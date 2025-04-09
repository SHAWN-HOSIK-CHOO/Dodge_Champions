using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
   public class FlashBangProjectile : MonoBehaviour
   {
      private Vector3   _targetPosition;
      private float     _moveSpeed    = 15f;
      private float     _explodeDelay = 2f;
      private float     _flashRadius;
      private float     _blindDuration;
      private LayerMask _enemyLayer;
      private bool      _exploded = false;

      public void Initialize(Vector3 target, float radius, float blindTime, LayerMask layer)
      {
         _targetPosition = target;
         _flashRadius    = radius;
         _blindDuration  = blindTime;
         _enemyLayer     = layer;

         StartCoroutine(AutoExplode());
      }

      private void Update()
      {
         if (_exploded) return;

         // 타겟을 향해 이동
         Vector3 dir = (_targetPosition - transform.position).normalized;
         transform.position += dir * (_moveSpeed * Time.deltaTime);

         // 목표 근처 도달하면 바로 터지기
         if (Vector3.Distance(transform.position, _targetPosition) < 0.5f)
         {
            Explode();
         }
      }

      private void OnTriggerEnter(Collider other)
      {
         if (_exploded) return;

         // 적이 아니어도 뭐든 부딪히면 터지게
         Explode();
      }

      private IEnumerator AutoExplode()
      {
         yield return new WaitForSeconds(_explodeDelay);

         if (!_exploded)
         {
            Explode();
         }
      }

      private void Explode()
      {
         _exploded = true;

         // 여기서 주변 적 감지
         Collider[]            hitColliders   = Physics.OverlapSphere(transform.position, _flashRadius, _enemyLayer);
         List<IFlashBlindable> enemiesToBlind = new();

         foreach (var hitCollider in hitColliders)
         {
            if (hitCollider.TryGetComponent<IFlashBlindable>(out var blindable))
            {
               enemiesToBlind.Add(blindable);
            }
         }

         foreach (var enemy in enemiesToBlind)
         {
            enemy.ApplyBlind(_blindDuration);
         }
         
         Destroy(gameObject);
      }
   }
}