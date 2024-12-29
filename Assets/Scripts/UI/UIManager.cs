using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using TMPro;

namespace UI
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
        }
        
        [Header("Game Start Info Panel")] 
        public GameObject startPanel;
        public TMP_Text startText;

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
    }
}
