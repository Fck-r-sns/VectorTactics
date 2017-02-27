using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private SoldierController controllable;

    private int floorMask;

    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        controllable.Move(new Vector3(h, 0.0f, v));
    }

    private void Turn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            controllable.Turn(hit.point - transform.position);
        }
    }

}
