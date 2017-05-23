using UnityEngine;
using EventBus;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Statistics))]
public class GameFlowManager : MonoBehaviour, IEventSubscriber
{
    private Statistics statistics;

    public void OnReceived(EBEvent e)
    {
        if (e.type == EBEventType.HealthChanged)
        {
            if ((e as HealthChanged).value <= 0.0f)
            {
                StartCoroutine(FinishAndRestart());
            }
        }
    }

    void Awake()
    {
        statistics = GetComponent<Statistics>();
    }

    IEnumerator FinishAndRestart()
    {
        statistics.WriteToFile();
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
