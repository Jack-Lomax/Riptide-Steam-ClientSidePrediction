using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using System.Linq;

public class SyncedBody : GenericBody
{
    [SerializeField]
    private int id;

    public static List<SyncedBody> List { get; private set; } = new List<SyncedBody>();
    private List<NetworkedBodyState> correctedStates = new List<NetworkedBodyState>();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        List[id] = this;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        List.Remove(this);
    }

    protected override void TickLoop()
    {
        while (correctedStates.Count > 0)
        {
            const float maxPositionDelta = 0.0000001f;

            NetworkedBodyState state = correctedStates[0];
            correctedStates.RemoveAt(0);

            Vector3 positionDelta = state.position - stateBuffer[state.tick % ServerSettings.BUFFER_SIZE].position;

            if (positionDelta.sqrMagnitude > maxPositionDelta)
            {
                stateBuffer[state.tick % ServerSettings.BUFFER_SIZE] = state;
                Reconcile((localTick - state.tick) % ServerSettings.BUFFER_SIZE);
            }
        }

        base.TickLoop();

        if (NetworkManager.Singleton.Server.IsRunning)
        {
            NetworkedBodyState state = GetBodyState();

            Message message = Message.Create(MessageSendMode.unreliable, ServerToClient.syncBody);

            message.AddInt(List.IndexOf(this));
            message.AddUInt(state.tick);
            message.AddVector3(state.position);
            message.AddVector3(state.velocity);
            message.AddQuaternion(state.rotation);
            message.AddVector3(state.angularVelocity);

            NetworkManager.Singleton.Server.SendToAll(message, NetworkManager.Singleton.Client.Id);
        }
    }

    [MessageHandler((ushort)ServerToClient.syncBody)]
    private static void ReceiveCorrectedState(Message message)
    {
        int index = message.GetInt();
        NetworkedBodyState state = new NetworkedBodyState
        {
            tick = message.GetUInt(),
            position = message.GetVector3(),
            velocity = message.GetVector3(),
            rotation = message.GetQuaternion(),
            angularVelocity = message.GetVector3()
        };

        if (List.ElementAtOrDefault(index) != null)
            List[index].correctedStates.Add(state);
    }
}
