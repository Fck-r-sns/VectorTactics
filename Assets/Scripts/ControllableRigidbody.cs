using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllableRigidbody : MonoBehaviour, Controllable
{

    [SerializeField]
    private float speed = 6.0f;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0.0f;
        rigidbody.MovePosition(transform.position + direction.normalized * speed * Time.fixedDeltaTime);
    }

    public void Turn(Vector3 direction)
    {
        direction.y = 0.0f;
        rigidbody.MoveRotation(Quaternion.LookRotation(direction));
    }

    public void Shoot()
    {
        throw new NotImplementedException();
    }

}
