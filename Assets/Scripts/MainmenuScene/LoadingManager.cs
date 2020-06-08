using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * 로딩 화면을 제어/관리하는 스크립트
 * 
 * */

// 로딩화면에 출력되는 팁을 관리하는 클래스
[System.Serializable]
public class Tip
{
    //public AnimationClip anim;
    //public int numb;
    public string content;
}

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    public GameObject popUpScreen;

    /// <summary>
    /// 로딩이 시작되었는지 체크하는 플레그
    /// 로딩이 두 차례 중첩되는 버그를 방지하기위함
    /// </summary>
    bool isLoadingNow;
    /// <summary>
    /// 로딩이 완료되었는지 체크하는 플레그
    /// </summary>
    bool isLoadDone;

    //IEnumerator coroutine;

    [Header("Tips")]
    public Tip[] tip;
    public Animator tipPlayPoint;
    //public Text tipNumb;
    public Text tipText;
    

    private void Awake()
    {
        instance = this;

        isLoadingNow = false;
        isLoadDone = false;
        //coroutine = LoadScreenCloseChecker();
    }
    
    /// <summary>
    /// 로딩 스크린을 제어
    /// </summary>
    /// <param name="isOn">Active status</param>
    public void LoadScreenActive(bool isOn)
    {
        
        // 로딩 시작시 설명 및 아이콘을 선택 및 출력하는
        if (isOn)
        {
            if (isLoadingNow) return;
            popUpScreen.SetActive(true);

            // 랜덤 tip 선택
            int randIdx = Random.Range(0, tip.Length);

            tipPlayPoint.SetInteger("idx", randIdx);

            // 설명 문구 출력
            //tipNumb.text = ""+ tip[randIdx].numb + "/" + tip.Length;
            tipText.text = "tip : "+ tip[randIdx].content;

            //StopCoroutine(coroutine);
            //StartCoroutine(coroutine);
            StartCoroutine(LoadScreenCloseChecker());
        }
        else
        {
            isLoadDone = true;
        }
    }

    /// <summary>
    /// 로딩 팝업이 꺼질 수 있는지 체크하는 코루틴
    /// </summary>
    public IEnumerator LoadScreenCloseChecker()
    {
        float timeLimit = 2f;
        float timer = 0f;

        isLoadingNow = true;

        // 데이터 로드가 완료되고, 일정 시간이 지나야 로딩 스크린이 꺼진다.
        while(timer < timeLimit || !isLoadDone)
        {
            timer += Time.deltaTime;

          //  print("timer : " + timer);
          //  print("loadDoneFlag : " + isLoadDone);

            yield return null;
        }

        isLoadingNow = false;

        popUpScreen.SetActive(false);
    }
    
}
