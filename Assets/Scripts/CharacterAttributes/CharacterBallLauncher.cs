using Ball;
using Game;
using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
    public class CharacterBallLauncher : NetworkBehaviour
    {
       public GameObject pfCurrentBall = null;
       public GameObject instantiatedBall;

       public override void OnNetworkSpawn()
       {
          if (IsOwner)
          {
             pfCurrentBall       = GameManager.Instance.localPlayerBall;
             //Debug.Log("Owner tag and layer : " +pfCurrentBall.tag + " : " + LayerMask.LayerToName(pfCurrentBall.layer));
          }
          else
          {
             pfCurrentBall       = GameManager.Instance.enemyPlayerBall;
             //.Log("Enemy tag and layer : " +pfCurrentBall.tag + " : " + LayerMask.LayerToName(pfCurrentBall.layer));
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
          //Debug.Log("Instantiated tag and layer : " + instantiatedBall.tag + " : " + LayerMask.LayerToName(instantiatedBall.layer));
       }
       
       [ServerRpc]
       public void ThrowBallServerRPC(Vector3 targetPosition)
       {
          ThrowBallClientRPC(targetPosition);
       }

       [ClientRpc]
       private void ThrowBallClientRPC(Vector3 targetPosition)
       {
          if (instantiatedBall != null)
          {
             float speed  = instantiatedBall.GetComponent<BallScript>().GetCalculatedBallSpeed();
             float height = instantiatedBall.GetComponent<BallScript>().GetCalculatedBallHeight();
             LaunchBall(targetPosition,speed, height);
          }
       }
       
       private void LaunchBall( Vector3 targetPosition, float speed = 1f, float height = 2f)
       {
          if (instantiatedBall != null)
          {
             instantiatedBall.GetComponent<BallScript>().Initialize(targetPosition,speed,height,false,IsOwner);
          }
       }

       public void CallBack_LaunchBallOnAnimation()
       {
          if (instantiatedBall != null)
          {
             instantiatedBall.transform.parent = null;
             instantiatedBall.GetComponent<BallScript>().StartCommand(instantiatedBall.transform.position);
          }
       }
    }
}
