using System.Collections.Generic;
using UnityEngine;
using System;

class Utility
{
    public static System.Random random = new System.Random();
    //Trunca un vector a un largo máximo
    public static Vector3 Truncate(Vector3 vec, float maxMag)
    {
        var mag = vec.magnitude;
        if (mag < float.Epsilon) return vec;
        else return vec * Mathf.Min(1f, maxMag / mag);
    }

    //Baraja de elementos de un array dinámico
    public static void KnuthShuffle<T>(List<T> array)
    {
        for (int i = 0; i < array.Count - 1; i++)
        {
            var j = random.Next(i, array.Count);
            if (i != j)
            {
                var temp = array[j];
                array[j] = array[i];
                array[i] = temp;
            }
        }
    }
    //Dibuja una flecha gizmo con dirección (en vez de solo una linea)
    public static void GizmoArrow(Vector3 from, Vector3 to, float scale = 0.25f, float gap = 0.15f)
    {
        var dir = to - from;
        to -= dir.normalized * gap;
        var offset = Vector3.Cross(dir.normalized, Vector3.up) * scale;
        var arrowLeft = to - dir.normalized * scale + offset;
        var arrowRight = to - dir.normalized * scale - offset;

        Gizmos.DrawLine(from, to);
        Gizmos.DrawLine(to, arrowLeft);
        Gizmos.DrawLine(to, arrowRight);
    }

    public static LayerMask LayerNumberToMask(int layerNum)
    {
        return 1 << layerNum;
    }
    public static int LayerMaskToInt(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask.value, 2);
    }
    //http://mathworld.wolfram.com/SpherePointPicking.html
    public static Vector3 RandomDirection()
    {
        var theta = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        var phi = UnityEngine.Random.Range(0f, Mathf.PI);
        var u = Mathf.Cos(phi);
        return new Vector3(Mathf.Sqrt(1 - u * u) * Mathf.Cos(theta), Mathf.Sqrt(1 - u * u) * Mathf.Sin(theta), u);
    }
}
