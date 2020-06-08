using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

// 한 웨이브
[System.Serializable]
public struct Wave
{
    public EnemyType enemyType;
    public int enemyCount;
    public float TermEnemies;
    public float TermNextWave;

    Wave(EnemyType enemyType, int enemyCount, float TermEnemies, float TermNextWave)
    {
        this.enemyType = enemyType;
        this.enemyCount = enemyCount;
        this.TermEnemies = TermEnemies;
        this.TermNextWave = TermNextWave;
    }
}

[System.Serializable]
public class Stage
{
    [HideInInspector] public string Name;

    // 해당 스테이지의 각 웨이브 정보
    // 적, 마리수, 적들 사이 간격, 종료 후 다음 스테이지 시작까지의 간격
    [TitleGroup("스테이지")]
    public Wave[] wave;

    // 보상
    [SerializeField] int reward;

    public int getReward() => reward;
}

[System.Serializable]
public class Level
{
    [HideInInspector] public string Name;

    // 해당 레벨의 스테이지
    [TitleGroup("레벨")]
    public Stage[] stage;
    // 각 레벨별 배경 이미지
    public Sprite levelBackgroundImg;
}

public class StageManager : MonoBehaviour
{
    /**
     * 
     * 각 스테이지를 제어하는 스크립트
     * 
     * */

    public static StageManager instance;

    // 각 레벨
    public Level[] Level = new Level[5];

    // 작동될 스테이지
    // 스테이지 시작 버튼 클릭시 설정된다
    // Ingame 씬의 EnemySpawner에서 본 변수에 접근한다
    Stage workingStage;


    public Stage getWorkingStage()
    {
        return workingStage;
    }

    public void setWorkingStage(Stage s)
    {
        workingStage = s;
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        instance = this;
    }

    private void Update()
    {
    }

    public void SetStage()
    {

        //EventSystem.current.currentSelectedGameObject
        GameObject obj = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        string tmp = obj.name; // ex) Stage1-2

        int level = int.Parse(tmp.Substring(tmp.Length - 3, 1));
        int stage = int.Parse(tmp.Substring(tmp.Length - 1));

        workingStage = Level[level - 1].stage[stage - 1];
    }

    // 현재 튜토리얼 스테이지 인지를 확인하는 함수
    public bool IsTutorialStage()
    {
        if (workingStage == Level[0].stage[0])
            return true;
        else return false;
    }


    /// <summary>
    /// 현재 스테이지의 레벨을 반환
    /// </summary>
    /// <returns>0~4</returns>
    public int getCurLevel()
    {
        for (int i = 0; i < Level.Length; i++)
        {
            for (int j = 0; j < Level[i].stage.Length; j++)
            {
                if (Level[i].stage[j] == workingStage)
                    return i;
            }
        }
        return -1;
    }
    public bool getCurStageInfo(int level, int stage)
    {
        if (Level[level - 1].stage[stage - 1] == workingStage)
            return true;
        return false;
    }

    public GameObject GetStageBtnObj()
    {
        //EventSystem.current.currentSelectedGameObject
        GameObject obj = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        return obj;
    }
}
