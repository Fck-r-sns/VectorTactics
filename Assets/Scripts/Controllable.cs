using UnityEngine;

public interface Controllable  {
    void Move(Vector3 direction);
    void Turn(Vector3 direction);
    void Shoot();
}
