using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 개발자 모드 시작
 * 
 * */

public class DevMode : MonoBehaviour
{
    public GameObject DevBtn1;
    public GameObject DevBtn2;

    int flag = 0;

    private void OnEnable()
    {
        SetBtnActive(false);
    }

    private void OnDisable()
    {
        SetBtnActive(false);
    }
    
    /// <summary>
    /// 본 함수를 10회 연속 실행시키면 개발자 버튼 활성화
    /// </summary>
    public void DevModeStarter()
    {
        flag++;

        if (flag > 15)
        {
            SetBtnActive(true);
        }
    }

    // 버튼 활성화 or 비활성화
    public void SetBtnActive(bool active)
    {
        if (DevBtn1.activeSelf != active)
            DevBtn1.SetActive(active);
        if (DevBtn2.activeSelf != active)
            DevBtn2.SetActive(active);

        flag = 0;
    }
}
