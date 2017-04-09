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

    private bool blueSeesRed = false;
    private bool redSeesBlue = false;

    public SoldierController GetBlueSoldier()
    {
        return blueSoldier;
    }

    public SoldierController GetRedSoldier() {
        return redSoldier;
    }

    public bool IsEnemySpotted(Defines.Side targetEnemy)
    {
        return (targetEnemy == Defines.Side.Blue) ? redSeesBlue : blueSeesRed;
    }

    void Awake()
    {

    }

    void Update()
    {
        blueSeesRed = visibilityChecker.CheckVisibility(blueSoldier, redSoldier);
        redSeesBlue = visibilityChecker.CheckVisibility(redSoldier, blueSoldier);
    }
}
