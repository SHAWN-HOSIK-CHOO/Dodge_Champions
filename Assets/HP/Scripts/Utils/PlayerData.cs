using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputControlScheme;

public class PlayerData : MonoBehaviour
{
    [System.Serializable]
    public class PlayRecord
    {
        public uint win;
        public uint lose;
    }
}
