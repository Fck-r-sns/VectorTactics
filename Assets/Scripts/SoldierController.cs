using UnityEngine;

using EventBus;

[RequireComponent(typeof(SoldierAnimation))]
public class SoldierController : MonoBehaviour, Controllable
{

    [SerializeField]
    private global::Defines.Side side = Defines.Side.Blue;

    [SerializeField]
    private float speed = 6.0f;

    [SerializeField]
    private float health = 100.0f;

    private SoldierAnimation animator;
    private Weapon weapon;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<SoldierAnimation>();
        weapon = GetComponentInChildren<Weapon>();
    }

    public Defines.Side GetSide()
    {
        return side;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return isDead;
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
        health -= damage;
        if (health <= 0.0f && !isDead)
        {
            animator.AnimateDeath();
            isDead = true;
            GetComponent<Collider>().enabled = false;
            weapon.gameObject.GetComponent<Collider>().enabled = false;
        }

        Dispatcher.SendEvent(new HealthChanged(health, side));
    }

}
