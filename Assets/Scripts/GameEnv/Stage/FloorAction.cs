using System;
using CharacterAttributes;
using UnityEngine;

namespace GameEnv.Stage
{
    public class FloorAction : MonoBehaviour
    {
        public Transform[] spawnTransforms = new Transform[2];

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Trigger Floor");
                other.gameObject.GetComponent<CharacterMovement>().HandleFloorFall(spawnTransforms);
            }
        }
    }
}
