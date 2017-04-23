using UnityEngine;

using EventBus;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Soldier")
        {
            CharacterState state = other.gameObject.GetComponent<CharacterState>();
            if (state.health < 100.0f)
            {
                state.Heal();
                Destroy(gameObject);
                Dispatcher.SendEvent(new EBEvent() { type = EBEventType.HealthPackCollected });
            }
        }
    }
}
