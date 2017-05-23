using UnityEngine;
using EventBus;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(Statistics))]
[RequireComponent(typeof(WorldState))]
public class GameFlowManager : MonoBehaviour, IEventSubscriber
{
    public const string GAMES_COUNTER_KEY = "gamesCounter";
    private const float MAX_GAME_DURATION = 60.0f;

    [SerializeField]
    private bool clearCounter;

    private Statistics statistics;
    private WorldState world;
    private int gamesCounter;
    private bool finished = false;
    private float startTime;

    public void OnReceived(EBEvent e)
    {
        if (finished)
        {
            return;
        }
        if (e.type == EBEventType.HealthChanged)
        {
            if ((e as HealthChanged).value <= 0.0f)
            {
                finished = true;
                StartCoroutine(FinishAndRestart());
            }
        }
    }

    private void Awake()
    {
        statistics = GetComponent<Statistics>();
        world = GetComponent<WorldState>();
        if (clearCounter)
        {
            PlayerPrefs.SetInt(GAMES_COUNTER_KEY, 0);
        }
        gamesCounter = PlayerPrefs.GetInt(GAMES_COUNTER_KEY);
        LoadScenario();
        startTime = Time.time;
    }

    private void Update()
    {
        if (!finished && Time.time - startTime > MAX_GAME_DURATION)
        {
            finished = true;
            StartCoroutine(FinishAndRestart());
        }
    }

    private void LoadScenario()
    {
        using (StreamReader file = new StreamReader(File.OpenRead("./Files/configurations")))
        {
            int battlesPerConfiguration = int.Parse(file.ReadLine());
            List<string> configs = new List<string>();
            string line;
            while ((line = file.ReadLine()) != null)
            {
                configs.Add(line);
            }
            int index = gamesCounter / battlesPerConfiguration;
            if (index < configs.Count)
            {
                string cfg = configs[index];
                string[] split = cfg.Split(':');
                ActivateSoldierController(world.GetCharacterState(GameDefines.Side.Blue).gameObject, split[0]);
                ActivateSoldierController(world.GetCharacterState(GameDefines.Side.Red).gameObject, split[1]);
            }
            else
            {
                Application.Quit();
            }
        }
    }

    private IEnumerator FinishAndRestart()
    {
        yield return new WaitForSeconds(2.0f);
        statistics.WriteToFile();
        ++gamesCounter;
        PlayerPrefs.SetInt(GAMES_COUNTER_KEY, gamesCounter);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    private void ActivateSoldierController(GameObject soldier, string controllerKey)
    {
        switch (controllerKey)
        {
            case "fsm":
                soldier.GetComponent<Ai.Fsm.FsmController>().enabled = true;
                break;
            case "bt":
                soldier.GetComponent<Ai.Bt.BtController>().enabled = true;
                break;
            case "fl":
                soldier.GetComponent<Ai.Fl.FuzzyController>().enabled = true;
                break;
            case "nn":
                soldier.GetComponent<Ai.Nn.NeuralController>().enabled = true;
                break;
        }
    }
}
