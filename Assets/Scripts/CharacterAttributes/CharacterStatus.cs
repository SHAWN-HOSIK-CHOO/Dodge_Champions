using System;
using Game;
using GameUI;
using SinglePlayer;
using UnityEngine;
using Unity.Netcode;

namespace CharacterAttributes
{
   public class CharacterStatus : NetworkBehaviour
   {
      public int maxHp = 200;

      [Header("공에서 사용")]
      public int justDodgeSuccessCounts = 0;
      public int hitCounts              = 0;

      // 기본적으로 서버에서만 쓰기 가능하도록 설정
      public NetworkVariable<int> playerHealth = new NetworkVariable<int>(
                                                                          200, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
                                                                         );

      public bool isThisPlayer = true;

      public NetworkVariable<bool> isHpZero = new NetworkVariable<bool>(
                                                                         false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
                                                                        );
      
      public override void OnNetworkSpawn()
      {
         // 소유 여부를 확인
         isThisPlayer = IsOwner;

         // 서버에서만 초기화
         if (IsServer)
         {
            playerHealth.Value = maxHp;
         }

         if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
         {
            playerHealth.OnValueChanged += OnHealthChanged;
            isHpZero.OnValueChanged     += OnIsHpZeroChanged;
         }
         else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
         {
            playerHealth.OnValueChanged += OnHealthChangedSinglePlayer;
            isHpZero.OnValueChanged     += OnIsHpZeroChangedSinglePlayer;
         }
         
         // playerHealth.OnValueChanged += OnHealthChanged;
         // isHpZero.OnValueChanged     += OnIsHpZeroChanged;
      }

      public override void OnDestroy()
      {
         if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
         {
            playerHealth.OnValueChanged -= OnHealthChanged;
            isHpZero.OnValueChanged     -= OnIsHpZeroChanged;
         }
         else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
         {
            playerHealth.OnValueChanged -= OnHealthChangedSinglePlayer;
            isHpZero.OnValueChanged     -= OnIsHpZeroChangedSinglePlayer;
         }
         base.OnDestroy();
      }

      private void OnHealthChanged(int previousValue, int newValue)
      {
         float ratio = (float)newValue / maxHp;
         UIManager.Instance.ChangeFillWithRatio(ratio, isThisPlayer);
         
         // 서버에서 HP가 0이 되었는지 확인
         if (IsServer && newValue == 0 && !isHpZero.Value)
         {
            isHpZero.Value = true;
         }
      }

      private void OnHealthChangedSinglePlayer(int previousValue, int newValue)
      {
         float ratio = (float)newValue / maxHp;
         SinglePlayerGM.Instance.SetPlayerFill(ratio);
         
         // 서버에서 HP가 0이 되었는지 확인
         if (IsServer && newValue == 0 && !isHpZero.Value)
         {
            isHpZero.Value = true;
         }
      }
      
      public void HandleHit(int damage)
      {
         if (IsOwner)
         {
            TakeDamageServerRpc(damage);
         }
      }
      
      [ServerRpc]
      private void TakeDamageServerRpc(int damage)
      {
         playerHealth.Value = Mathf.Max(0, playerHealth.Value - damage);
      }
      
      private void OnIsHpZeroChanged(bool previousValue, bool newValue)
      {
         if (newValue)
         {
            int loserID = (int)OwnerClientId;
            GameManager.Instance.EndGameServerRPC(loserID);
            Debug.Log("loser is : " +loserID);
         }
      }

      private void OnIsHpZeroChangedSinglePlayer(bool previousValue, bool newValue)
      {
         if (newValue)
         {
            SinglePlayerGM.Instance.EndGame(false);
         }
      }
      
      public void IncreaseDodgeSuccessCount()
      {
         justDodgeSuccessCounts++;
      }

      public void IncreaseHitCount()
      {
         hitCounts++;
      }

      public int GetHitHp()
      {
         return maxHp - playerHealth.Value;
      }
   }
}