using Character;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace Ball
{
    public class BallLauncher : NetworkBehaviour
    {
       public GameObject pfCurrentBall = null;
       public GameObject instantiatedBall;

       public override void OnNetworkSpawn()
       {
          if (IsOwner)
          {
             pfCurrentBall = GameManager.Instance.localPlayerBall;
          }
          else
          {
             pfCurrentBall = GameManager.Instance.enemyPlayerBall;
          }
       }

       [ServerRpc]
       public void SpawnBallServerRPC()
       {
          SpawnBallClientRPC();
       }
       
       [ClientRpc]
       private void SpawnBallClientRPC()
       {
          Transform ballSpawnPosition = this.GetComponent<CharacterManager>().ballSpawnPosition.transform;

          instantiatedBall = Instantiate(pfCurrentBall, ballSpawnPosition.position, ballSpawnPosition.rotation);
          instantiatedBall.transform.SetParent(ballSpawnPosition);
       }
       
       [ServerRpc]
       public void ThrowBallServerRPC(Vector3 targetPosition, float speed = 1f, float height = 2f)
       {
          ThrowBallClientRPC(targetPosition, speed, height);
       }

       [ClientRpc]
       private void ThrowBallClientRPC(Vector3 targetPosition, float speed = 1f, float height = 2f)
       {
          LaunchBall(targetPosition,speed, height);
       }
       
       private void LaunchBall( Vector3 targetPosition, float speed = 1f, float height = 2f)
       {
          if (instantiatedBall != null)
          {
             instantiatedBall.GetComponent<BallScript>().Initialize(targetPosition,speed,height);
          }
       }

       public void CallBack_LaunchBallOnAnimation()
       {
          instantiatedBall.transform.parent = null;
          instantiatedBall.GetComponent<BallScript>().StartCommand(instantiatedBall.transform.position);
       }
    }
}
