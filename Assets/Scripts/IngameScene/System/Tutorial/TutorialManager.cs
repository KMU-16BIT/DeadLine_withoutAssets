using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    // 대상을 가르켜주는 화살표 오브젝트
    public GameObject targetArrow;

    // 플레이어 스킬 gauge에 접근
    public PlayerSkillController PSK;
    // 타워 스킬 gauge에 접근
    public SkillController[] SC;


    // 튜토리얼 씬 플레이어
    public GameObject tutorialScenePlayer;

    // 튜토리얼 마스크
    public GameObject tutorialMask_tower;
    public GameObject tutorialMask_obstacle;
    public GameObject tutorialMask_playerSkill;
    int onceFlag; // 0: default. Tower skill mask 대기
                  // 1: Tower skill mask done. Player skill mask 대기
                  // 2: Player skill maks done.
    public GameObject curTutorialMask_tower;
    public GameObject curTutorialMask_obstacle;
    public GameObject curTutorialMask_playerSkill;


    // 0~3 : SC 각 인덱스
    // 4   : PSK
    // 호출된 arrow를 보관하는 변수
    GameObject[] arrow = new GameObject[5];

    private void Awake()
    {
        instance = this;
        onceFlag = 0;
    }

    private void Start()
    {
    }

    public void callArrow(Vector3 pos, int idx)
    {
        GameObject obj = Instantiate(targetArrow, pos, Quaternion.identity);
        arrow[idx] = obj;

        // 튜토리얼 마스크 최초 한번만
        if (onceFlag== 0)
        {
            onceFlag += 1;
            curTutorialMask_tower = Instantiate(tutorialMask_tower, Vector3.zero, Quaternion.identity);
            TimeScaleManager.instance.PauseSpeed();
        }
    }
    
    // 특정 시점에 화살표를 삭제하기 위한 함수
    public void DestroyArrow(int idx)
    {
        arrow[idx]?.GetComponent<TargetArrowController>().StopCoroutine();
        arrow[idx] = null;
    }

    // 플레이어 머리 위에 화살표를 띄우는 함수
    // 방해요소 첫 등장시 호출된다.
    public void callArrowOnPlayer()
    {
        if (!arrow[4])
        {
            callArrow(PSK.transform.position, 4);

            // 튜토리얼 마스크 최초 한번만
            if (onceFlag == 1)
            {
                onceFlag += 1;
                curTutorialMask_obstacle = Instantiate(tutorialMask_obstacle, Vector3.zero, Quaternion.identity);
                TimeScaleManager.instance.PauseSpeed();
            }
        }
    }

    /// <summary>
    /// 플레이어 스킬 시전 튜토리얼을 호출하는 함수
    /// </summary>
    public void callTutorialMask_playerSkill()
    {
        // 튜토리얼 마스크 최초 한번만
        if (onceFlag == 2)
        {
            onceFlag += 1;
            curTutorialMask_playerSkill= Instantiate(tutorialMask_playerSkill, Vector3.zero, Quaternion.identity);
            TimeScaleManager.instance.PauseSpeed();
        }

        // n초 뒤 마스크 삭제
    }
    

    public void OffTutorialMask_tower()
    {
        if (curTutorialMask_tower == null) return;
        curTutorialMask_tower?.GetComponent<TutorialMaskController>().MaskOff();
        curTutorialMask_tower = null;
    }
    public void OffTutorialMask_obstacle()
    {
        if (curTutorialMask_obstacle == null) return;
        curTutorialMask_obstacle?.GetComponent<TutorialMaskController>().MaskOff();
        curTutorialMask_obstacle = null;
    }
    public void OffTutorialMask_playerSkill()
    {
        if (curTutorialMask_playerSkill == null) return;
        curTutorialMask_playerSkill?.GetComponent<TutorialMaskController>().MaskOff();
        curTutorialMask_playerSkill= null;
    }



    public void StartTutorialCoroutine()
    {
        StartCoroutine(GaugeChecker());
    }

    public void PlayTutorialScene()
    {
        Instantiate(tutorialScenePlayer, new Vector3(10, 0, 0), Quaternion.identity);

        Camera.main.transform.Translate(new Vector3(10, 0, 0));
    }

    /// <summary>
    /// 튜토리얼 씬 재생 종료
    /// </summary>
    public void EndTutorialScene()
    {
        // Enemy Spawner 동작 시작 및
        // Gauge Checker 동작 시작
        EnemySpawner.instance.StartEnemySpawner();

        // 카메라 복귀
        Camera.main.transform.Translate(new Vector3(-10, 0, 0));
    }

    IEnumerator GaugeChecker()
    {
        while (true)
        {
            for(int i = 0; i < SC.Length; i++)
            {
                // 스킬이 충전되었고, 현재 화살표로 가리키고 있지 않다면
                if (SC[i].getSkillOn() && !arrow[i])
                {
                    callArrow(SC[i].transform.position, i);
                }
            }


            yield return null;
        }
    }
}
