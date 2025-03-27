using Game;
using GameUI;
using Skill;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
    public class CharacterSkillLauncher : NetworkBehaviour
    {
        private CharacterController _characterController;
        private Coroutine _skillCoroutine;
        private Coroutine _skillCoolDownCoroutine;

        public SkillBase currentSkill = null;
        public SkillBase ultraSkill = null;

        public bool isSkillActivated = false;

        public float skillCoolDown = 0.5f;
        public bool canUseSkill = true;

        public override void OnNetworkSpawn()
        {
            _characterController = this.GetComponent<CharacterController>();
        }

        public void StartSkill(ISkillInput input)
        {
            if (_skillCoroutine != null)
            {
                //Debug.Log("Current skill is still active");
            }
            else
            {
                _skillCoroutine = StartCoroutine(SkillCoroutine(input));
            }
        }

        private IEnumerator SkillCoroutine(ISkillInput input)
        {
            isSkillActivated = true;
            yield return currentSkill.Activate(_characterController, input);

            //Debug.Log("Skill coroutine ended");
            isSkillActivated = false;
            _skillCoroutine = null;
        }

        public void StartSkillCoolDown()
        {
            _skillCoolDownCoroutine = StartCoroutine(CoSkillCoolDown());
        }

        private IEnumerator CoSkillCoolDown()
        {
            canUseSkill = false;
            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                UIManager.Instance.skillCoolDownImage.fillAmount = 0f;
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {

            }

            float elapsedTime = 0f;

            while (elapsedTime <= skillCoolDown)
            {
                elapsedTime += Time.deltaTime;

                float fillAmount = elapsedTime / skillCoolDown;

                if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                {
                    UIManager.Instance.skillCoolDownImage.fillAmount = fillAmount;
                }
                else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
                {

                }

                yield return null;
            }

            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                UIManager.Instance.skillCoolDownImage.fillAmount = 1f;
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {

            }

            canUseSkill = true;
        }
    }
}
