using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
public class CoroutineHandler : MonoBehaviour
{
    // �ڷ�ƾ ��ü�� DontDestroy ������Ʈ�� ���� �Ϸ��� �������� �ۼ�
    public Coroutine BeginCoroutine(Func<IEnumerator> getter)
    {
        return base.StartCoroutine(getter?.Invoke());
    }

    public void EndCoroutine(Coroutine corroutine)
    {
        base.StopCoroutine(corroutine);
    }

}
