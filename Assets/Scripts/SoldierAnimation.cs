using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SoldierAnimation : MonoBehaviour
{

    private Animator animator;

	void Awake () {
        animator = GetComponent<Animator>();
	}

    public void AnimateMoving(Vector3 direction)
    {
        float angle_rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle_rad);
        float sin = Mathf.Sin(angle_rad);

        // transform movement direction to object coordinate system
        float frontMovement = direction.x * sin + direction.z * cos;
        float sideMovement = direction.x * cos - direction.z * sin;

        animator.SetFloat("FrontMovement", frontMovement);
        animator.SetFloat("SideMovement", sideMovement);
    }

    public void AnimateShooting()
    {
        // TODO
    }
}
