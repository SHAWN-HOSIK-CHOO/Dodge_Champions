using System;
using UnityEngine;

namespace Tests
{
    public class DancerTest : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _animator.SetBool("Start", true);
        }

        private void OnDestroy()
        {
            _animator.SetBool("Start", false);
        }
    }
}
