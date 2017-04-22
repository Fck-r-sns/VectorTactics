using UnityEngine;

public class CharacterState : MonoBehaviour
{

    public Defines.Side side = Defines.Side.Blue;
    public float health = 100.0f;
    public float speed = 5.0f;

    public Vector3 movementDirection;
    public bool isInCover = false;
    public bool isNearCover = false;
    public bool isEnemyVisible = false;
    public float distanceToEnemy = 0.0f;
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

}