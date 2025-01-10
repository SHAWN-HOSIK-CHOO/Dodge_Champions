using System;
using System.Collections;
using System.Globalization;
using Game;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameUI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static  UIManager Instance => _instance == null ? null : _instance;

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
            startPanel.SetActive(false);
        }
        
        [Header("Game Start Info Panel")] 
        public GameObject startPanel;
        public TMP_Text startText;

        [Header("Dodge")] public GameObject dodgeText;

        [Header("State, 0 for attack 1 for defense")]
        public Image[] statesUIImages = new Image[2];

        [Header("Players' Hp bar")] 
        public Image playerFill;
        public Image enemyFill;

        private void Start()
        {
            dodgeText.SetActive(false);
            ResetFill(playerFill);
            ResetFill(enemyFill);
        }

        private void ResetFill(Image fillImage)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = 1.0f; 
            }
        }

        public void ChangeFillWithRatio(float fillRatio, bool isThisPlayer)
        {
            if (isThisPlayer)
            {
                // 만약 플레이어가 맞은거면
                playerFill.fillAmount = fillRatio;
            }
            else
            {
                // 상대가 맞은거라면
                enemyFill.fillAmount = fillRatio;
            }
        }
        
        public void StartGameCountDown(float time = 5f)
        {
            StartCoroutine(CoStartCountDown(5f));
        }
        
        private IEnumerator CoStartCountDown(float time)
        {
            startPanel.SetActive(true);
            while (time > 0)
            {
                startText.text = time.ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(1f);
                time--;
            }

            startText.text = "Game Start!";
            yield return new WaitForSeconds(1f);
            startPanel.SetActive(false);

            GameManager.Instance.isGameReadyToStart = true;
        }

        public void GameOverUI(int winnerID)
        {
            startPanel.SetActive(true);
            if (GameManager.Instance.localClientID == winnerID)
            {
                startText.text = "You Win!";
            }
            else
            {
                startText.text = "You Lose!";
            }
        }
    }
}
