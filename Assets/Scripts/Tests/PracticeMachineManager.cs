using System;
using System.Collections;
using Ball;
using Game;
using UnityEngine;

namespace Tests
{
    public class PracticeMachineManager : MonoBehaviour
    {
        public GameObject pfBall;
        public GameObject trackingPlayer;
        public Transform  ballSpawnTransform;

        public bool  stopPractice       = false;
        public float ballSpawnFrequency = 3f;
        public float ballSpeed          = 10f;
        public float machineLastingTime = 10f;
        
        private GameObject _currentBall;
        private Transform  _machineSpawnTransform;

        public void Initialize(GameObject ball, Transform machineSpawnTransform, float ballSpawnFreq, float ballSpeedF, float machineTime)
        {
            pfBall                 = ball;
            _machineSpawnTransform = machineSpawnTransform;
            ballSpawnFrequency     = ballSpawnFreq;
            ballSpeed              = ballSpeedF;
            machineLastingTime     = machineTime;

            this.transform.position = _machineSpawnTransform.position;
            this.transform.rotation = _machineSpawnTransform.rotation;
        }
        
        public void StartPractice(GameObject target)
        {
            trackingPlayer = target;
            
            StartCoroutine(CoStartPractice());
            StartCoroutine(CoStopPracticeAfterTime(machineLastingTime)); 
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
                _currentBall.GetComponent<BallScript>().Initialize(targetVector, ballSpeed, 0.2f);
                _currentBall.GetComponent<BallScript>().StartCommand(ballSpawnTransform.position);
            }
        }

        private IEnumerator CoStopPracticeAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            stopPractice = true; 
        }
    }
}

