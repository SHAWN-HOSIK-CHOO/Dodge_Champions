using UnityEngine;
using Unity.Netcode;

namespace CharacterAttributes
{
    public class CharacterStatus : NetworkBehaviour
    {
        public NetworkVariable<int> playerHealth = new NetworkVariable<int>(10);
    }
}
