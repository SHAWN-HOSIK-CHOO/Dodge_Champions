using System;
using Game;
using GameInput;
using GameLobby;
using UnityEngine;

namespace Ball
{
   public class AutoBallScript : BallScript
   {
      public  GameObject pfBall;
      private GameObject _instantiatedBall;
      public  GameObject trackingPlayer;

      public Transform ballSpawnTransform;
      
      public  float ballShootFrequency = 3f;
      private float _currentTime       = 0f;

      public int damage = 5;

      private void Start()
      {
         ballDamage = damage;
         
         this.transform.parent = null;
         _canStart             = true;
         _currentTime          = ballShootFrequency;
      }

      public override void Initialize(Vector3 target, float speed, float height, bool horizontalThrow = false, bool isThisOwner = true)
      {
         base.Initialize(target, speed, height, horizontalThrow, isThisOwner);
         if (isThisOwner)
         {
            trackingPlayer = GameManager.Instance.enemyPlayer;
            Debug.Log("Tracking Enemy");
         }
         else
         {
            trackingPlayer = GameManager.Instance.localPlayer;
            Debug.Log("Tracking Local");
         }
      }

      protected override void BallUpdate()
      {
         if (_currentTime >= ballShootFrequency)
         {
            _currentTime = 0f;
            _instantiatedBall = Instantiate(pfBall, ballSpawnTransform.position,
                                            ballSpawnTransform.rotation);
            Vector3 targetVector = new Vector3(trackingPlayer.transform.position.x,
                                               trackingPlayer.transform.position.y - 0.3f,
                                               trackingPlayer.transform.position.z);
            _instantiatedBall.GetComponent<BallScript>().Initialize(targetVector, _throwSpeed, 0.2f);
            _instantiatedBall.GetComponent<BallScript>().SetBallDamage(damage);
            _instantiatedBall.GetComponent<BallScript>().StartCommand(ballSpawnTransform.position);
         }

         _currentTime += Time.deltaTime;
      }
   }
}