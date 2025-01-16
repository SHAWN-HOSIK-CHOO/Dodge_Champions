using System;
using UnityEngine;

namespace Game
{
    public enum EGameMode
    {
        MULTIPLAER,
        SINGLEPLAYER,
        Count
    }
    
    public class GameMode : MonoBehaviour
    {
        private static GameMode _instance = null;
        public static  GameMode Instance => _instance == null ? null : _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public EGameMode CurrentGameMode
        {
            get;
            set;
        }
    }
}
