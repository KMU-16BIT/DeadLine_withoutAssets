using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 튜토리얼에서 타겟을 가리키는 화살표를 제어하는 스크립트
 * 
 * */

public class TargetArrowController : MonoBehaviour
{
    int idx;
    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = Destroy();
        StartCoroutine(coroutine);
    }


    public void setIdx(int idx) => this.idx = idx;
    
    IEnumerator Destroy()
    {
        float timer = 0f;

        while(true)
        {
            timer += Time.deltaTime;

            yield return null;
        }


    }

    public void StopCoroutine()
    {
        StopCoroutine(coroutine);

        Destroy(gameObject);
    }
}
