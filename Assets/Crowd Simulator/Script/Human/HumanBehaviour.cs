using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehaviour : MonoBehaviour {

	public enum humanInput
	{
		Wander,
		RunAway,
	}

	EventFSM<humanInput> _fsm;
	public bool walking;
	public bool isRunning = true;
	public Vector3 target;
	public float panicTime;
	float tick;
	Material currentMaterial;

	void Awake()
	{
		currentMaterial = GetComponent<Renderer>().material;
	}

	void Start()
	{
		var walking = new State<humanInput>("walking");
		var runningAway = new State<humanInput>("runningAway");

		StateConfigurer.Create(walking)
			.SetTransition(humanInput.Wander, runningAway);

		StateConfigurer.Create(runningAway)
			.SetTransition(humanInput.RunAway, walking);
		
	    runningAway.OnEnter += OnUpdateRunning;
	    walking.OnEnter += OnUpdateWalking;

		_fsm = new EventFSM<humanInput>(walking);
	}

	void Update()
	{
		tick += Time.deltaTime;
		if (tick > panicTime)
			walkAgain();
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				target = hit.point;
			}
		}

		if (isRunning)
		{
			_fsm.Feed(humanInput.Wander);
		}	
		else
		{
			_fsm.Feed(humanInput.RunAway);
		}	

		_fsm.Update();
	}

	public void changeColor()
	{
		currentMaterial.color = Color.red;
	}

	void walkAgain()
	{
		isRunning = false;
		tick = 0f;
		currentMaterial.color = Color.yellow;
	}
	void OnUpdateRunning()
	{
		walking = false;
	}
	void OnUpdateWalking()
	{
		walking = true;
	}
}
