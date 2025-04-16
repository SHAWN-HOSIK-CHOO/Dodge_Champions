namespace Unity.Netcode
{
    internal struct TimeMessage : INetworkMessage, INetworkSerializeByMemcpy
    {
        public int Version => 0;

        public ulong ClientId;
        public double Time;
        public double SendDelay;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            BytePacker.WriteValuePacked(writer, ClientId);
            BytePacker.WriteValuePacked(writer, Time);
            BytePacker.WriteValuePacked(writer, SendDelay);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
        {
            var networkManager = (NetworkManager)context.SystemOwner;
            ByteUnpacker.ReadValuePacked(reader, out ClientId);
            ByteUnpacker.ReadValuePacked(reader, out Time);
            ByteUnpacker.ReadValuePacked(reader, out SendDelay);
            return true;
        }

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (NetworkManager)context.SystemOwner;
            if (networkManager.ConnectionManager.ConnectedClients.TryGetValue(ClientId, out var client))
            {
                client.ReceiveDelay = networkManager.NetworkTimeSystem.m_TimeSec - Time;
                client.SendDelay = SendDelay;

                if (client.Rtt > 0.25)
                {
                    UnityEngine.Debug.LogWarning($"{client.ClientId} Rtt High ! {client.Rtt}");
                }
            }
        }
    }
}
