using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{

    [SerializeField]
    private SoldierController blueSoldierController;

    [SerializeField]
    private SoldierController redSoldierController;

    [SerializeField]
    private VisibilityChecker visibilityChecker;

    private bool enemySpotted = false;

    public SoldierController blueSoldier {
        get {
            return blueSoldierController;
        }
    }

    public SoldierController redSoldier {
        get {
            return redSoldierController;
        }
    }

    void Awake()
    {

    }

    void Update()
    {
        enemySpotted = visibilityChecker.IsVisible();
    }
}
