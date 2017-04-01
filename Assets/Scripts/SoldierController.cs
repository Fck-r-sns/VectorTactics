using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoldierAnimation))]
public class SoldierController : MonoBehaviour, Controllable
{

    [SerializeField]
    private float speed = 6.0f;

    private SoldierAnimation animator;
    private Weapon weapon;

    private void Awake()
    {
        animator = GetComponent<SoldierAnimation>();
        weapon = GetComponentInChildren<Weapon>();
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0.0f;
        transform.Translate(direction.normalized * speed * Time.fixedDeltaTime, Space.World);
        animator.AnimateMoving(direction);
    }

    public void TurnToPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Shoot(Vector3 direction)
    {
        if (weapon != null)
        {
            weapon.fire(direction);
            animator.AnimateShooting();
        }
    }

}
