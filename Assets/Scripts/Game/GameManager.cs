using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance = null;
        public static  GameManager Instance => _instance == null ? null : _instance;

        public GameObject cinemachineCamera;
        public GameObject mainCamera;

        public bool isGameReadyToStart = false;

        public GameObject localPlayerBall;
        public GameObject enemyPlayerBall;
        public Transform  localPlayerBallSpawnPosition;

        public Transform[] spawnTransforms = new Transform[2];

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

        public int localClientID;
        public int enemyClientID;

        public GameObject localPlayer = null;
        public GameObject enemyPlayer = null;
    }
}
