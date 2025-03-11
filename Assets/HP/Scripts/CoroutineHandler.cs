using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
public class CoroutineHandler : MonoBehaviour
{
    // 코루틴 주체를 DontDestroy 오브젝트에 위임 하려는 목적으로 작성
    public Coroutine BeginCoroutine(Func<IEnumerator> getter)
    {
        return base.StartCoroutine(getter?.Invoke());
    }

    public void EndCoroutine(Coroutine corroutine)
    {
        base.StopCoroutine(corroutine);
    }

}
