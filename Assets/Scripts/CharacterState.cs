using UnityEngine;

public class CharacterState : MonoBehaviour
{

    public Defines.Side side = Defines.Side.Blue;
    public float health = 100.0f;
    public float speed = 5.0f;

    public Vector3 movementDirection;
    public int numberOfCovers;
    public bool isNearCover = false;
    public bool isEnemyVisible = false;
    public float distanceToEnemy = 0.0f;
    public Vector3 lastEnemyPosition;
    public CharacterState enemyState = null;

    public bool isDead {
        get {
            return health <= 0.0f;
        }
    }

    public Vector3 position {
        get {
            return transform.position;
        }
    }

    public bool isInCover {
        get {
            return numberOfCovers > 0;
        }
    }

    public void DoDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0.0f, 100.0f);
        EventBus.Dispatcher.SendEvent(new HealthChanged(health, side));
    }

    public void Heal()
    {
        health = 100.0f;
        EventBus.Dispatcher.SendEvent(new HealthChanged(health, side));
    }

}