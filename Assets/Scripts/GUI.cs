using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EventBus;

public class GUI : MonoBehaviour, IEventSubscriber
{

    private Slider blueSoldierHealthSlider;
    private Text blueSoldierHealthText;
    private Slider redSoldierHealthSlider;
    private Text redSoldierHealthText;
    private Text logicFps;
    private Text physicsFps;

    private int address = AddressProvider.GetFreeAddress();

    public void OnReceived(EBEvent e)
    {
        switch (e.type)
        {
            case EBEventType.HealthChanged:
                HealthChanged hce = (e as HealthChanged);
                switch (hce.side)
                {
                    case GameDefines.Side.Blue:
                        SetBlueSoldierHealth(hce.value);
                        break;
                    case GameDefines.Side.Red:
                        SetRedSoldierHealth(hce.value);
                        break;
                }
                break;
        }
    }

    void Start () {
        blueSoldierHealthSlider = transform.Find("Canvas/BlueSoldier/BlueSoldierHealthSlider").gameObject.GetComponent<Slider>();
        blueSoldierHealthText = transform.Find("Canvas/BlueSoldier/BlueSoldierHealthText").gameObject.GetComponent<Text>();
        redSoldierHealthSlider = transform.Find("Canvas/RedSoldier/RedSoldierHealthSlider").gameObject.GetComponent<Slider>();
        redSoldierHealthText = transform.Find("Canvas/RedSoldier/RedSoldierHealthText").gameObject.GetComponent<Text>();
        logicFps = transform.Find("Canvas/Fps/LogicFps").gameObject.GetComponent<Text>();
        physicsFps = transform.Find("Canvas/Fps/PhysicsFps").gameObject.GetComponent<Text>();

        Dispatcher.Subscribe(EBEventType.HealthChanged, address, gameObject);
    }

    void OnDestroy()
    {
        Dispatcher.Unsubscribe(EBEventType.HealthChanged, address);
    }

    void Update ()
    {
        logicFps.text = "" + 1.0f / Time.deltaTime;
        physicsFps.text = "" + 1.0f / Time.fixedDeltaTime;
    }

    private void SetBlueSoldierHealth(float value)
    {
        blueSoldierHealthSlider.value = value;
        blueSoldierHealthText.text = "" + (int)value;
    }

    private void SetRedSoldierHealth(float value)
    {
        redSoldierHealthSlider.value = value;
        redSoldierHealthText.text = "" + (int)value;
    }
}
