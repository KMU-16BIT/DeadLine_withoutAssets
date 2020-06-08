using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * 
 * 게임 종료 이후 모든 기능 및 프로세스를 제어하는 스크립트
 * 애니메이션, 점수 출력, 성적 저장 등 프로세스 처리
 * 
 * */


public class GameOverChecker : MonoBehaviour
{
    public static GameOverChecker instance;

    // 게임 종료 체크 플래그
    bool isGameOver;

    // 결과 창 팝업
    [SerializeField] GameObject resultPopUp;

    // 애니메이션
    // 0: F 애니메이션
    // 1: Ending 애니메이션
    [SerializeField] GameObject[] animScene;

    // 성적 출력 슬롯
    [SerializeField] Image gradeSlot;

    // 게임 오버 사운드
    public AudioSource gameoverSound;

    private void Awake()
    {
        instance = this;
        resultPopUp.SetActive(false);
    }

    public void GameOver(int leftHP)
    {
        // 종료가 반복되는 현상 방지
        if (isGameOver) return;

        isGameOver = true;

        SettingSound.SS.PauseAllSound(true);

        // 적 스폰 종료
        EnemySpawner.instance.StopEnemySpawner();

        // 게임 속도 정상화
        TimeScaleManager.instance.setSpeed(1.0f);

        // 게임 진행 중지
        //TimeScaleManager.instance.PauseSpeed();

        // 남은 hp로 성적 계산 및 저장
        Grade calcGrade;
        if (leftHP == 100) calcGrade = Grade.Aplus;
        else if (leftHP > 75) calcGrade = Grade.A;
        else if (leftHP > 50) calcGrade = Grade.B;
        else if (leftHP > 25) calcGrade = Grade.C;
        else calcGrade = Grade.F;

        // 더 높은 성적을 기록했을 경우 저장하기 위해 현재 성적 반환
        int tmp = GradeManager.instance.getGrade(StageSelector.instance.getTargetIndex());

        // 성적 값이 없거나, 더 높은 성적을 기록했다면 성적 저장
        if ((Grade)tmp == Grade.N || calcGrade < (Grade)tmp)
        {
            print("성적 갱신! 저장합니다.");
            GradeManager.instance.setGrade(calcGrade);
        }
        else
        {
            print("성적이 더 낮아서 저장되지 않습니다.");
        }

        // 기존 재화를 획득하지 못한 상태였다면
        if ((Grade)tmp == Grade.N || (Grade)tmp == Grade.F)
        {
            // 이번에는 재화를 획득할 수 있는 성적이라면
            if (calcGrade != Grade.F)
                // 재화 획득
                DataManager.instance.saveMoney(StageManager.instance.getWorkingStage().getReward(), false);
        }

        // 사운드 정지
        SettingSound.SS.soundOff = true;

        // 모든 웨이브 종료 : 스테이지 종료       
        CallResultPopUp(calcGrade);


    }

    void CallResultPopUp(Grade grade)
    {
        if (grade == Grade.F)
        {
            if (grade == Grade.F) AchieveManager.instance.AchievementClear(12);
            gameoverSound.Play();

            Camera.main.transform.Translate(new Vector3(10, 0));
            Instantiate(animScene[0], Camera.main.transform.position + new Vector3(0, 0, 10), Quaternion.identity);
        }
        else
        {
            // 업적 체크(스테이지 클리어 여부)
            if (StageManager.instance.IsTutorialStage()) AchieveManager.instance.AchievementClear(2);
            else if (StageManager.instance.getCurStageInfo(1, 4)) AchieveManager.instance.AchievementClear(3);
            else if (StageManager.instance.getCurStageInfo(2, 4)) AchieveManager.instance.AchievementClear(4);
            else if (StageManager.instance.getCurStageInfo(3, 4)) AchieveManager.instance.AchievementClear(5);
            else if (StageManager.instance.getCurStageInfo(4, 4)) AchieveManager.instance.AchievementClear(6);

            // 업적 체크(각 성적 최초 획득)
            if (grade == Grade.Aplus) AchieveManager.instance.AchievementClear(8);
            else if (grade == Grade.A) AchieveManager.instance.AchievementClear(9);
            else if (grade == Grade.B) AchieveManager.instance.AchievementClear(10);
            else if (grade == Grade.C) AchieveManager.instance.AchievementClear(11);

            // 업적 체크(A Plus , F가 3번인가 여부)
            if(GradeManager.instance.getGradeCount(Grade.Aplus)) AchieveManager.instance.AchievementClear(13);
            else if(GradeManager.instance.getGradeCount(Grade.F)) AchieveManager.instance.AchievementClear(14);

            // 마지막 스테이지에서 F가 아닌 성적을 기록했다면.
            if (StageManager.instance.getCurLevel() == 4)
            {
                if (StageManager.instance.getCurStageInfo(5, 1)) AchieveManager.instance.AchievementClear(7);

                Camera.main.transform.Translate(new Vector3(10, 0));
                Instantiate(animScene[1], Camera.main.transform.position + new Vector3(0, 0, 10), Quaternion.identity);
            }
            else
            {
                // Result popup에 성적 기록
                gradeSlot.sprite = GradeManager.instance.gradeImages[(int)grade];
                resultPopUp.SetActive(true);
            }
        }
    }

    // restart btn
    public void Restart()
    { 
        // 사운드 플래그 정상화
        SettingSound.SS.soundOff = false;

        SceneManager.LoadSceneAsync("Ingame");
    }

    // home btn, go to mainmenu
    public void Home()
    {
        // 사운드 플래그 정상화
        SettingSound.SS.soundOff = false;

        // go to Mainmenu Scene
        TimeScaleManager.instance.setSpeed(1.0f);
        SceneManager.LoadSceneAsync("Mainmenu");
    }

    // next btn
    public void Next()
    {
        // 사운드 플래그 정상화
        SettingSound.SS.soundOff = false;

        int targetIdx = StageSelector.instance.getTargetIndex();

        // max stage
        if (targetIdx < 16)
            targetIdx += 1; // go to next stage
        StageSelector.instance.setTargetIndex(targetIdx);

        int level = targetIdx / 4;
        int stage = targetIdx % 4;

        print("targetIdx >> " + targetIdx);
        print(level + "level, " + stage + "stage");

        StageManager.instance.setWorkingStage(StageManager.instance.Level[level].stage[stage]);
        SceneManager.LoadSceneAsync("Ingame");

        print("next");
    }
}
