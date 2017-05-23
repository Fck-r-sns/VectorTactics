using UnityEngine;
using EventBus;

public class CharacterState : MonoBehaviour
{

    public GameDefines.Side side = GameDefines.Side.Blue;
    public float health = GameDefines.FULL_HP;
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
            Vector3 pos = transform.position;
            pos.y = 0.0f;
            return pos;
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
        health = Mathf.Clamp(health, 0.0f, GameDefines.FULL_HP);
        Dispatcher.GetInstance().SendEvent(new HealthChanged(side, health, damage, false));
    }

    public void Heal()
    {
        float healthBefore = health;
        health = GameDefines.FULL_HP;
        Dispatcher.GetInstance().SendEvent(new HealthChanged(side, health, health - healthBefore, true));
    }

}