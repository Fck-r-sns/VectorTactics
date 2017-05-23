using UnityEngine;

using EventBus;

public class HealthPack : MonoBehaviour
{
    [SerializeField]
    private GameObject packObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Soldier")
        {
            CharacterState state = other.gameObject.GetComponent<CharacterState>();
            if (state.health < 100.0f)
            {
                state.Heal();
                Destroy(packObject);
                Dispatcher.GetInstance().SendEvent(new HealthPackCollected(state.side));
            }
        }
    }
}
