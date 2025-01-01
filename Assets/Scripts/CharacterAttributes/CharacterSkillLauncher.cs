using System.Collections;
using Skill;
using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
    public class CharacterSkillLauncher : NetworkBehaviour
    {
        private CharacterController _characterController;
        private Coroutine           _skillCoroutine;

        public SkillBase currentSkill = null;
        
        public bool isSkillActivated = false;
        
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
            _skillCoroutine  = null;
        }
    }
}
