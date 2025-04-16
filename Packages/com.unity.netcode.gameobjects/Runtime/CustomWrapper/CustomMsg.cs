using Unity.Collections;

namespace Unity.Netcode
{
    public class CustomMsg : NetworkBehaviour
    {
        protected string MessageName = "TestPingPongMessage";
        public override void OnNetworkSpawn()
        {
            NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, ReceiveMessage);
        }
        private void SendMessage(ulong clientId)
        {
            var writer = new FastBufferWriter(sizeof(double), Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                //write value
                customMessagingManager.SendNamedMessage(MessageName, clientId, writer);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.CustomMessagingManager != null)
            {
                NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(MessageName);
            }
        }

        private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
        {
            //read value
        }

    }
}
