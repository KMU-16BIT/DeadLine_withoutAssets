using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    /**
     * 
     * 가장 가까이 접근해있는 적을 찾아 타겟으로 지정하는 스크립트
     * 
     * */

    public static TargetSelector instance;

    public GameObject target;

    [SerializeField] Transform startPos, endPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 시작 위치
        transform.position = startPos.position;

        StartCoroutine("selector");
    }
    int checkCount = 1;

    IEnumerator selector()
    {

        while(true)
        {
            if (transform.position.y > endPos.position.y)
            {
                if (checkCount > 0)
                {
                    checkCount -= 1;
                }
                else
                {
                    // 3회 체크해도 없을 시 정말 타겟이 없다고 판단.
                    target = null;
                }
                initSensor();
            }

            transform.Translate(Vector3.up * 20 *  Time.deltaTime);
            yield return null;
        }
    }
    
    void initSensor()
    {
        transform.position = startPos.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Enemy를 탐지했을 경우
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            checkCount = 1;

            //센서 위치 초기화
            initSensor();

            // 타겟 설정
            target = collision.gameObject;
        }
    }
}
