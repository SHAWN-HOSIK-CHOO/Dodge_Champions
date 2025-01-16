using System;
using Game;
using GameInput;
using GameLobby;
using SinglePlayer;
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

      public int damage = 2;

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

         if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
         {
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
         else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
         {
            trackingPlayer = SinglePlayerGM.Instance.enemyNpc;
         }
      }

      protected override void BallUpdate()
      {
         if (trackingPlayer == null)
         {
            Destroy(this.gameObject);
         }
         
         if (_currentTime >= ballShootFrequency)
         {
            _currentTime = 0f;
            _instantiatedBall = Instantiate(pfBall, ballSpawnTransform.position,
                                            ballSpawnTransform.rotation);
            Vector3 targetVector = new Vector3(trackingPlayer.transform.position.x,
                                               trackingPlayer.transform.position.y - 0.3f,
                                               trackingPlayer.transform.position.z);
            _instantiatedBall.GetComponent<BallScript>().Initialize(targetVector, ballLaunchSpeed, 0.2f);
            _instantiatedBall.GetComponent<BallScript>().SetBallDamage(damage);
            _instantiatedBall.GetComponent<BallScript>().StartCommand(ballSpawnTransform.position);
         }

         _currentTime += Time.deltaTime;
      }
   }
}
