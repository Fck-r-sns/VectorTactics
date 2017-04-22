using UnityEngine;

using EventBus;

[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(SoldierAnimation))]
public class SoldierController : MonoBehaviour, Controllable
{

    [SerializeField]
    private bool isInvulnerable = false;

    private CharacterState state;
    private SoldierAnimation animator;
    private Weapon weapon;

    private void Awake()
    {
        state = GetComponent<CharacterState>();
        animator = GetComponent<SoldierAnimation>();
        weapon = GetComponentInChildren<Weapon>();
    }

    public CharacterState GetState()
    {
        return state;
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0.0f;
        state.movementDirection = direction.normalized;
        transform.Translate(state.movementDirection * state.speed * Time.fixedDeltaTime, Space.World);
        animator.AnimateMoving(direction);
    }

    public void TurnToPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0.0f;
        if (!direction.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Shoot(Vector3 target)
    {
        if (weapon != null)
        {
            weapon.fire(target);
            animator.AnimateShooting();
        }
    }

    public void OnHit(float damage)
    {
        if (state.isDead || isInvulnerable)
        {
            return;
        }
        state.health -= damage;
        state.health = Mathf.Clamp(state.health, 0.0f, 100.0f);
        if (state.isDead)
        {
            animator.AnimateDeath();
            GetComponent<Collider>().enabled = false;
            weapon.gameObject.GetComponent<Collider>().enabled = false;
        }

        Dispatcher.SendEvent(new HealthChanged(state.health, state.side));
    }

}
