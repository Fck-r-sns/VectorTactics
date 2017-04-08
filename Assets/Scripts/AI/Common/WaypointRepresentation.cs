using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRepresentation : MonoBehaviour
{

    private Material material;

    public void SetHeight(float height)
    {
        transform.localScale = new Vector3(1, height, 1);
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

	void Start () {
        material = transform.GetChild(0).GetComponent<Renderer>().material;
	}

}
