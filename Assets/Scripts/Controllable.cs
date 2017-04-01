using UnityEngine;

public interface Controllable  {
    void Move(Vector3 direction);
    void TurnToPoint(Vector3 point);
    void Shoot(Vector3 target);
}
