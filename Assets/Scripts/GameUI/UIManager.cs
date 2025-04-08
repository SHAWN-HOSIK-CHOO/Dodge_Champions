using Game;
using GameLobby;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance => _instance == null ? null : _instance;
        
        public bool IsInitialized { get; private set; } = false;

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

        [Header("Game Start Info Panel")]
        public TMP_Text startText;

        [Header("Throw CoolDown")]
        public Image coolDownImage;

        [Header("Skill CoolDown")]
        public Image skillCoolDownImage;

        [Header("Turn CoolDown")]
        public Image turnCoolDownImage;

        [Header("Debug Area")]
        public TMP_Text playerBallSkillIndex;
        public TMP_Text enemyBallSkillIndex;

        [Header("Canvas")] 
        public Canvas canvas;

        [Header("HealthBar")] public GameObject pfHealthBar;
        
        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            //Debug
            playerBallSkillIndex.text = "Player Character No.  " + PlayerSelectionManager.Instance.GetLocalPlayerSelection();
            enemyBallSkillIndex.text = "Enemy Character No.  " + PlayerSelectionManager.Instance.GetEnemySelection();
            
            IsInitialized = true;
        }

        public void SetActiveAllObjects(bool flag)
        {
            startText.gameObject.SetActive(flag);
            coolDownImage.gameObject.SetActive(flag);
            skillCoolDownImage.gameObject.SetActive(flag);
            turnCoolDownImage.gameObject.SetActive(flag);
        }

        public void StartGameCountDown(float time = 5f)
        {
            StartCoroutine(CoStartCountDown(time));
        }

        private IEnumerator CoStartCountDown(float time)
        {
            startText.gameObject.SetActive(true);
            while (time > 0)
            {
                startText.text = time.ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(1f);
                time--;
            }

            startText.text = "Game Start!";
            yield return new WaitForSeconds(1f);

            startText.gameObject.SetActive(false);
            GameManager.Instance.isGameReadyToStart = true;
        }

        public void GameOverUI(int loserID)
        {
            if (GameManager.Instance.localClientID == loserID)
            {
                startText.text = "You Lose!";
            }
            else
            {
                startText.text = "You Win!";
            }
        }
    }
}
