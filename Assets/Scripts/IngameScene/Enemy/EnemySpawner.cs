using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * Enemy를 Spawn시키는 스크립트
 * 
 * */
 
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public AudioSource BossBGMSound;

    IEnumerator coroutine;

    // 현재 진행중인 스테이지가 튜토리얼 스테이지인지 여부를 확인하기 위한 플래그
    // 튜토리얼이라면, 플레이어 및 타워에 진행 안내 화살표가 표시된다
    bool isTutorialStage;

    // 현재 웨이브 진행 정도를 보여주는 ui
    [SerializeField] Text curWave;
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        coroutine = spawner_c(StageManager.instance.getWorkingStage());

        if (StageManager.instance.IsTutorialStage())
        {
            isTutorialStage = true;
            // StartEnemySpawner();

            TutorialManager.instance.PlayTutorialScene();
        }
        else
        {
            isTutorialStage = false;
            // 몬스터 스폰 시작
            StartEnemySpawner();
        }



    }

    // 해당 스테이지의 웨이브 정보를 전달받아 스테이지 진행
    IEnumerator spawner_c(Stage stageInfo)
    {
        // 남은 Enemy 수
        int leftEnemyCount;

        // 각 웨이브 순차 진행
        for (int i = 0; i < stageInfo.wave.Length; i++)
        {
            //print("웨이브 출발");
            print($"{i+1} 번째 웨이브 출발");
            curWave.text = $"{i+1}/{stageInfo.wave.Length}";

            leftEnemyCount = stageInfo.wave[i].enemyCount;

            //while (leftEnemyCount > 0)
            while(leftEnemyCount > 0)
            {
                yield return new WaitForSeconds(stageInfo.wave[i].TermEnemies);

                // 적 등장 범위
                float xRange = Random.Range(-2f, 2);
                Vector3 spawnPoint = transform.position;
                spawnPoint.x = xRange;
                    
                GameObject obj =
                    PoolingManager.instance.getEnemy(stageInfo.wave[i].enemyType);
                obj.transform.position = spawnPoint;
                                
                if (stageInfo.wave[i].enemyType == EnemyType.L1Boss ||
                    stageInfo.wave[i].enemyType == EnemyType.L1Boss ||
                    stageInfo.wave[i].enemyType == EnemyType.L1Boss ||
                    stageInfo.wave[i].enemyType == EnemyType.L1Boss)
                {
                    SettingSound.SS.BGMSound.Stop();
                    BossBGMSound.Play();
                }
                   

                // 생성 완료
                leftEnemyCount--;

                if (isTutorialStage && stageInfo.wave[i].enemyType >= EnemyType.Soju)
                {
                    TutorialManager.instance.callArrowOnPlayer();
                }


                yield return null;
            }
            
            // 다음 웨이브를 위한 시간 지연
            yield return new WaitForSeconds(stageInfo.wave[i].TermNextWave);
        }

        // Enemy가 모두 처치될때까지 대기
        while (true)
        {
            // 타겟이 없다면
            if (TargetSelector.instance.target == null)
            {

                yield return new WaitForSeconds(3);

                break;
            }
            yield return null;
        }

        // 모든 웨이브 종료 : 스테이지 종료
        // go to Mainmenu Scene
        // SceneManager.LoadSceneAsync(1);        
        GameOverChecker.instance.GameOver(PlayerController.instance.getHP());
    }
    public void StartEnemySpawner()
    {
        if (isTutorialStage)
        {
            // 튜토리얼 한정 UI 재생 시작(상호작용 객체에 대해 화살표로 표시)
            TutorialManager.instance.StartTutorialCoroutine();
        }

        // 몬스터 스폰 시작
        StartCoroutine(coroutine);
    }


    public void StopEnemySpawner()
    {
        StopCoroutine(coroutine);
    }
    

}
