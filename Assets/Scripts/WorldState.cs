using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{

    [SerializeField]
    private SoldierController blueSoldier;

    [SerializeField]
    private SoldierController redSoldier;

    [SerializeField]
    private VisibilityChecker visibilityChecker;

    private bool enemySpotted = false;

    public SoldierController GetBlueSoldier()
    {
        return blueSoldier;
    }

    public SoldierController GetRedSoldier() {
        return redSoldier;
    }

    public bool IsEnemySpotted()
    {
        return enemySpotted;
    }

    void Awake()
    {

    }

    void Update()
    {
        enemySpotted = visibilityChecker.IsVisible();
    }
}
