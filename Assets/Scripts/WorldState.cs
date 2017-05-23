using EventBus;
using UnityEngine;

using EventBus;
using System;

public class WorldState : MonoBehaviour, IEventSubscriber
{

    [SerializeField]
    private CharacterState blueSoldierState;

    [SerializeField]
    private CharacterState redSoldierState;

    [SerializeField]
    private VisibilityChecker visibilityChecker;

    private static WorldState instance;
    private Dispatcher dispatcher;
    private int address;
    private int healthPacks = 2;

    public static WorldState GetInstance()
    {
        return instance;
    }

    public int healthPacksAvailable {
        get {
            return healthPacks;
        }
    }

    public CharacterState GetCharacterState(GameDefines.Side side)
    {
        switch (side)
        {
            case GameDefines.Side.Blue:
                return blueSoldierState;
            case GameDefines.Side.Red:
                return redSoldierState;
            default:
                return null;
        }
    }

    public void OnReceived(EBEvent e)
    {
        if (e.type == EBEventType.HealthPackCollected)
        {
            --healthPacks;
        }
    }

    void Awake()
    {
        instance = this;

        blueSoldierState.enemyState = redSoldierState;
        redSoldierState.enemyState = blueSoldierState;

        // set last known positions to initial spawning points
        blueSoldierState.lastEnemyPosition = redSoldierState.position;
        redSoldierState.lastEnemyPosition = blueSoldierState.position;
    }

    void Start()
    {
        dispatcher = Dispatcher.GetInstance();
        address = dispatcher.GetFreeAddress();
        dispatcher.SendEvent(new GameStarted(PlayerPrefs.GetInt(GameFlowManager.GAMES_COUNTER_KEY), (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
        dispatcher.Subscribe(EBEventType.HealthPackCollected, address, gameObject);
    }

    void OnDestroy()
    {
        dispatcher.Unsubscribe(EBEventType.HealthPackCollected, address);
    }

    void Update()
    {
        UpdateCharacterState(blueSoldierState);
        UpdateCharacterState(redSoldierState);
    }

    private void UpdateCharacterState(CharacterState state)
    {
        state.isEnemyVisible = visibilityChecker.CheckVisibility(state.transform, state.enemyState.transform);
        if (state.isEnemyVisible)
        {
            state.lastEnemyPosition = state.enemyState.position;
            state.distanceToEnemy = Vector3.Distance(state.position, state.lastEnemyPosition);
        }
    }
}
