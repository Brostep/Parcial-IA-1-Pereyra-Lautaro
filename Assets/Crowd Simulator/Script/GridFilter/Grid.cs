using UnityEngine;
using System;
using System.Collections.Generic;
using IA2;
using IA2.FP;
using System.Linq;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
	public float x;
	public float z;
	public float cellWidth;
	public float cellHeight;
	public int width;
	public int height;
	void Update()
	{
		MaxPairs = 0;
	}

	static IEnumerable<Transform> RecursiveWalker(Transform parent)
	{
		foreach (Transform child in parent)
		{
			foreach (Transform grandchild in RecursiveWalker(child))
				yield return grandchild;
			yield return child;
		}
	}

	static IEnumerable<Tuple<int, int, T>> LazyMatrix<T>(T[,] matrix)
	{
		for (int i = 0; i < matrix.GetLength(0); i++)
			for (int j = 0; j < matrix.GetLength(1); j++)
				yield return Tuple.New(i, j, matrix[i, j]);
		//yield return new { x = i, y = j, value =  matrix[i, j]};

	}

	Dictionary<GridEntity, Tuple<int, int>> lastPositions;
	HashSet<GridEntity>[,] entities;

	readonly public Tuple<int, int> Outside = Tuple.New(-1, -1);
	readonly public GridEntity[] Empty = new GridEntity[0];

	public void UpdateEntity(GridEntity entity)
	{
		var prevPos = lastPositions.ContainsKey(entity) ? lastPositions[entity] : Outside;
		var currPos = PositionInGrid(entity.transform.position);

		//Same pos, no update needed
		if (prevPos.Equals(currPos))
			return;

		//Entity was previously inside the grid and it will move from there
		if (InsideGrid(prevPos))
		{
			entities[prevPos.First, prevPos.Second].Remove(entity);
		}

		//Entity is now inside the grid, and just moved from prev cell, add it to the new cell
		if (InsideGrid(currPos))
		{
			entities[currPos.First, currPos.Second].Add(entity);
			lastPositions[entity] = currPos;
		}
		else
		{
			lastPositions.Remove(entity);
		}
	}

	public IEnumerable<GridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition)
	{
		var from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), 0, Mathf.Min(aabbFrom.z, aabbTo.z));
		var to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), 0, Mathf.Max(aabbFrom.z, aabbTo.z));

		var fromGridPosition = PositionInGrid(from);
		var toGridPosition = PositionInGrid(to);

		if (!InsideGrid(fromGridPosition) && !InsideGrid(toGridPosition))
			return Empty;

		// Creamos tuplas de cada celda
		var cols = Generators.Generate(fromGridPosition.First, x => x + 1)
			.TakeWhile(x => x < width && x <= toGridPosition.First);

		var rows = Generators.Generate(fromGridPosition.Second, y => y + 1)
			.TakeWhile(y => y < height && y <= toGridPosition.Second);

		var cells = cols.SelectMany(
			col => rows.Select(
				row => Tuple.New(col, row)
			)
		);

		// Iteramos las que queden dentro del criterio
		return cells
			.SelectMany(cell => entities[cell.First, cell.Second])
			.Where(e =>
				from.x <= e.transform.position.x && e.transform.position.x <= to.x &&
				from.z <= e.transform.position.z && e.transform.position.z <= to.z
			).Where(x => filterByPosition(x.transform.position));


	}

	bool InsideGrid(Tuple<int, int> position)
	{
		return 0 <= position.First && position.First < width &&
			0 <= position.Second && position.Second < height;
	}


	void OnDestroy()
	{
		var ents = RecursiveWalker(transform).Select(x => x.GetComponent<GridEntity>()).Where(x => x != null);
		foreach (var e in ents)
		{
			e.OnMove -= OnEntityMove;
		}
	}

	void Awake()
	{
		lastPositions = new Dictionary<GridEntity, Tuple<int, int>>();
		entities = new HashSet<GridEntity>[width, height];

		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				entities[i, j] = new HashSet<GridEntity>();

		var ents = RecursiveWalker(transform)
			.Select(x => x.GetComponent<GridEntity>())
			.Where(x => x != null);
		foreach (var e in ents)
		{
			e.OnMove += UpdateEntity;
			UpdateEntity(e);
		}
		//		var seq = Generators.Generate(0, x => x + 1);
		//		var itrolu = seq.Take(width).SelectMany(
		//			x => seq.Take(height).Select(y => Tuple.New(x, y))
		//		);
		//
		//		foreach (var lol in itrolu) {
		//			entities[lol.First, lol.Second] = new HashSet<GridEntity>();
		//		}
	}

	void OnEntityMove(GridEntity ent)
	{
		UpdateEntity(ent);
	}

	int MaxPairs = 0;


	void OnDrawGizmos()
	{
		var cols = Generators.Generate(x, curr => curr + cellWidth)
			.Select(col => Tuple.New(
				new Vector3(col, 0, z),
				new Vector3(col, 0, z + cellHeight * height)
				)
		);

		var rows = Generators.Generate(z, curr => curr + cellHeight)
			.Select(row => Tuple.New(new Vector3(x, 0, row), new Vector3(x + cellWidth * width, 0, row)));

		foreach (var line in cols.Take(width + 1).Concat(rows.Take(height + 1)))
			Gizmos.DrawLine(line.First, line.Second);


		if (entities != null)
		{
			var toDraw = LazyMatrix(entities)
				.Where(t => t.Third.Count > 0)
				.ToList();

			//Flatten the sphere we're going to draw
			Gizmos.matrix *= Matrix4x4.Scale(Vector3.forward + Vector3.right);
			foreach (var t in toDraw)
			{
				var ix = t.First;
				var iy = t.Second;
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(
					new Vector3(x, 0, z) +
					new Vector3(
						(ix + 0.5f) * cellWidth,
						0,
						(iy + 0.5f) * cellHeight),
					Mathf.Min(cellWidth, cellHeight) / 2
				);
			}

			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.identity;
			foreach (var t in toDraw.Where(t => t.Third.Count > 1))
			{
				var pairs =
					t.Third.SelectMany(
						e1 => t.Third.Select(
							e2 => Tuple.New(e1, e2)
						)
					)
					;
				//Debug.Log("PAIRS "+pairs.Count());
				MaxPairs += pairs.Count(); //Math.Max(pairs.Count(), MaxPairs);

				var offset = Vector3.up * 3.0f;
				foreach (var te in pairs)
				{
					Gizmos.DrawLine(te.First.transform.position + offset, te.Second.transform.position + offset);
					Gizmos.DrawCube(te.First.transform.position + offset, Vector3.one);
				}
			}
		}
	}

	Tuple<int, int> PositionInGrid(Vector3 position)
	{
		return Tuple.New(
			(int)Mathf.Floor((position.x - x) / cellWidth),
			(int)Mathf.Floor((position.z - z) / cellHeight)
		);
	}

}