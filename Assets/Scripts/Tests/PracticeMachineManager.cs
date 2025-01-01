using System;
using System.Collections;
using Ball;
using Game;
using UnityEngine;

namespace Tests
{
    public class PracticeMachineManager : MonoBehaviour
    {
        private static PracticeMachineManager _instance = null;
        public static  PracticeMachineManager Instance => _instance == null ? null : _instance;

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

        public GameObject pfBall;
        public GameObject trackingPlayer;
        public Transform  ballSpawnTransform;

        public bool  stopPractice       = false;
        public float ballSpawnFrequency = 3f;
        public float ballSpeed          = 10f;

        private GameObject _currentBall;
        
        public void StartPractice(GameObject target)
        {
            trackingPlayer = target;

            StartCoroutine(CoStartPractice());
        }

        private IEnumerator CoStartPractice()
        {
            while (!stopPractice)
            {
                yield return new WaitForSeconds(ballSpawnFrequency);
                _currentBall = Instantiate(pfBall, ballSpawnTransform.position, ballSpawnTransform.rotation);
                Vector3 targetVector = new Vector3(trackingPlayer.transform.position.x,
                                                   trackingPlayer.transform.position.y - 0.3f,
                                                   trackingPlayer.transform.position.z);
                _currentBall.GetComponent<BallScript>().Initialize(targetVector,ballSpeed,0.2f);
                _currentBall.GetComponent<BallScript>().StartCommand(ballSpawnTransform.position);
            }
        }
    }
}
