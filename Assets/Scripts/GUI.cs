using UnityEngine;
using UnityEngine.UI;

using EventBus;

public class GUI : MonoBehaviour, IEventSubscriber
{

    private Slider blueSoldierHealthSlider;
    private Text blueSoldierHealthText;
    private Text blueSoldierControllerTypeText;
    private Slider redSoldierHealthSlider;
    private Text redSoldierHealthText;
    private Text redSoldierControllerTypeText;
    private Text logicFps;
    private Text physicsFps;
    private Text gameIndex;

    private Dispatcher dispatcher;
    private int address;

    public void OnReceived(EBEvent e)
    {
        switch (e.type)
        {
            case EBEventType.GameStarted:
                GameStarted gse = (e as GameStarted);
                gameIndex.text = "" + gse.gameIndex;
                break;

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

            case EBEventType.ControllerInited:
                ControllerInited cie = (e as ControllerInited);
                switch (cie.side)
                {
                    case GameDefines.Side.Blue:
                        blueSoldierControllerTypeText.text = cie.controllerType.ToString();
                        break;
                    case GameDefines.Side.Red:
                        redSoldierControllerTypeText.text = cie.controllerType.ToString();
                        break;
                }
                break;
        }
    }

    void Start () {
        blueSoldierHealthSlider = transform.Find("Canvas/BlueSoldier/BlueSoldierHealthSlider").gameObject.GetComponent<Slider>();
        blueSoldierHealthText = transform.Find("Canvas/BlueSoldier/BlueSoldierHealthText").gameObject.GetComponent<Text>();
        blueSoldierControllerTypeText = transform.Find("Canvas/BlueSoldier/BlueSoldierControllerType").gameObject.GetComponent<Text>();
        redSoldierHealthSlider = transform.Find("Canvas/RedSoldier/RedSoldierHealthSlider").gameObject.GetComponent<Slider>();
        redSoldierHealthText = transform.Find("Canvas/RedSoldier/RedSoldierHealthText").gameObject.GetComponent<Text>();
        redSoldierControllerTypeText = transform.Find("Canvas/RedSoldier/RedSoldierControllerType").gameObject.GetComponent<Text>();
        logicFps = transform.Find("Canvas/Fps/LogicFps").gameObject.GetComponent<Text>();
        physicsFps = transform.Find("Canvas/Fps/PhysicsFps").gameObject.GetComponent<Text>();
        gameIndex = transform.Find("Canvas/GameIndexValue").gameObject.GetComponent<Text>();

        dispatcher = Dispatcher.GetInstance();
        address = dispatcher.GetFreeAddress();
        dispatcher.Subscribe(EBEventType.GameStarted, address, gameObject);
        dispatcher.Subscribe(EBEventType.HealthChanged, address, gameObject);
        dispatcher.Subscribe(EBEventType.ControllerInited, address, gameObject);
    }

    void OnDestroy()
    {
        dispatcher.Unsubscribe(EBEventType.GameStarted, address);
        dispatcher.Unsubscribe(EBEventType.HealthChanged, address);
        dispatcher.Unsubscribe(EBEventType.ControllerInited, address);
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
