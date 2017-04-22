using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{

    [SerializeField]
    private CharacterState blueSoldierState;

    [SerializeField]
    private CharacterState redSoldierState;

    [SerializeField]
    private VisibilityChecker visibilityChecker;

    public CharacterState GetCharacterState(Defines.Side side)
    {
        switch (side)
        {
            case Defines.Side.Blue:
                return blueSoldierState;
            case Defines.Side.Red:
                return redSoldierState;
            default:
                return null;
        }
    }

    void Awake()
    {
        blueSoldierState.enemyState = redSoldierState;
        redSoldierState.enemyState = blueSoldierState;
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
