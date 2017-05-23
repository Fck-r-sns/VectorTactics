using System.Collections.Generic;
using UnityEngine;

using EventBus;
using System.IO;
using System.Text;

public class Statistics : MonoBehaviour, IEventSubscriber
{

    class PlayerStats
    {
        public long timestamp;
        public GameDefines.ControllerType controllerType;
        public bool dead;
        public float damageGiven;
        public float damageTaken;
        public bool healthPackCollected;
        public float frameTime;
        public int framesCount;

        public void WriteToFile(StreamWriter file)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(timestamp).Append(";")
                .Append(controllerType).Append(";")
                .Append(dead).Append(";")
                .Append(damageGiven).Append(";")
                .Append(damageTaken).Append(";")
                .Append(healthPackCollected).Append(";")
                .Append(frameTime / framesCount).Append(";");
            file.WriteLine(builder.ToString());
        }
    }

    private Dictionary<GameDefines.Side, PlayerStats> stats = new Dictionary<GameDefines.Side, PlayerStats>();
    private Dispatcher dispatcher;
    private int address;

    public void OnReceived(EBEvent e)
    {
        switch (e.type)
        {
            case EBEventType.GameStarted:
                var gse = e as GameStarted;
                stats[GameDefines.Side.Blue].timestamp = gse.timestamp;
                stats[GameDefines.Side.Red].timestamp = gse.timestamp;
                break;

            case EBEventType.ControllerInited:
                var cie = e as ControllerInited;
                stats[cie.side].controllerType = cie.controllerType;
                break;

            case EBEventType.NewFrame:
                var nfe = e as NewFrame;
                stats[nfe.side].frameTime += nfe.frameTime;
                ++stats[nfe.side].framesCount;
                break;

            case EBEventType.HealthChanged:
                var hce = e as HealthChanged;
                if (!hce.isHeal)
                {
                    stats[hce.side].damageTaken += hce.diff;
                    stats[GameDefines.OpposingTo(hce.side)].damageGiven += hce.diff;
                    if (hce.value <= 0.0f)
                    {
                        stats[hce.side].dead = true;
                    }
                }
                break;

            case EBEventType.HealthPackCollected:
                var hpce = e as HealthPackCollected;
                stats[hpce.side].healthPackCollected = true;
                break;
        }
    }

    public void WriteToFile()
    {
        using (StreamWriter file = File.AppendText("./Files/statistics.csv"))
        {
            stats[GameDefines.Side.Blue].WriteToFile(file);
            stats[GameDefines.Side.Red].WriteToFile(file);
        }
    }

    private void Start()
    {
        stats.Add(GameDefines.Side.Blue, new PlayerStats());
        stats.Add(GameDefines.Side.Red, new PlayerStats());

        dispatcher = Dispatcher.GetInstance();
        address = dispatcher.GetFreeAddress();
        dispatcher.Subscribe(EBEventType.GameStarted, address, gameObject);
        dispatcher.Subscribe(EBEventType.ControllerInited, address, gameObject);
        dispatcher.Subscribe(EBEventType.NewFrame, address, gameObject);
        dispatcher.Subscribe(EBEventType.HealthChanged, address, gameObject);
        dispatcher.Subscribe(EBEventType.HealthPackCollected, address, gameObject);
    }

    private void OnDestroy()
    {
        dispatcher.Unsubscribe(EBEventType.GameStarted, address);
        dispatcher.Unsubscribe(EBEventType.ControllerInited, address);
        dispatcher.Unsubscribe(EBEventType.NewFrame, address);
        dispatcher.Unsubscribe(EBEventType.HealthChanged, address);
        dispatcher.Unsubscribe(EBEventType.HealthPackCollected, address);
    }
}
