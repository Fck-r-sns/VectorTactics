using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SoldierAnimation))]
public class SoldierController : MonoBehaviour, Controllable
{

    [SerializeField]
    private float speed = 6.0f;

    private Rigidbody rigidbody;
    private SoldierAnimation animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<SoldierAnimation>();
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0.0f;
        rigidbody.MovePosition(transform.position + direction.normalized * speed * Time.fixedDeltaTime);
        animator.AnimateMoving(direction);
    }

    public void Turn(Vector3 direction)
    {
        direction.y = 0.0f;
        rigidbody.MoveRotation(Quaternion.LookRotation(direction));
    }

    public void Shoot()
    {
        // TODO
        animator.AnimateShooting();
    }

}
