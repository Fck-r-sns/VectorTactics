using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRepresentation : MonoBehaviour
{

    private Material material;

    public void SetHeight(float height)
    {
        height = Mathf.Clamp(height, 0.01f, 1.0f);
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
