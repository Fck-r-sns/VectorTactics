using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 6.0f;

    private Vector3 movement;
    private Rigidbody rigidbody;
    private Animator animator;
    private int floorMask;

    private void Awake()
    {
        movement = new Vector3();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        floorMask = LayerMask.GetMask("Floor");
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turn();
        UpdateAnimation(h, v);
    }

    private void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(transform.position + movement);
    }

    private void Turn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            Vector3 viewVector = hit.point - transform.position;
            viewVector.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(viewVector);
            rigidbody.MoveRotation(rotation);
        }
    }

    private void UpdateAnimation(float h, float v)
    {
        animator.SetFloat("FrontMovement", v);
        animator.SetFloat("SideMovement", h);
    }

}
