using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRepresentation : MonoBehaviour
{

    private GameObject model;
    private Material material;

    public void SetHeight(float height)
    {
        height = Mathf.Clamp(height, 0.01f, 1.0f);
        transform.localScale = new Vector3(1, height, 1);
    }

    public void SetCellSize(float size)
    {
        model.transform.localScale = new Vector3(size, 1, size);
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

	void Awake () {
        model = transform.GetChild(0).gameObject;
        material = model.GetComponent<Renderer>().material;
	}

}
