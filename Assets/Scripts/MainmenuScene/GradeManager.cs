using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/**
 * 
 * 각 스테이지 성적을 관리 및 제어하는 스크립트
 * 
 * */


public enum Grade {N, Aplus, A, B, C, F };



public class GradeManager : MonoBehaviour
{
    public static GradeManager instance;



    // A+ A B C F N(투명이미지) 각 성적 이미지
    public Sprite[] gradeImages;
    
    

    // 각 스테이지의 성적을 저장
    static Grade[] stageGrade;



    public static bool isFirstAwake = true;

    private void Awake()
    {
        instance = this;


        // init Grades
        if (isFirstAwake)
        {
            Debug.Log("GradeManager awake");
            
            //isFirstAwake = false;
            stageGrade = new Grade[17]; // 스테이지 크기, 초기값인 0으로 셋팅된다.
            for (int i = 0; i < stageGrade.Length; i++)
            {
                stageGrade[i] = Grade.N;
            }
            print("stageGrade.Length >> " + stageGrade.Length);
        }
    }
    

    // load한 정보를 바탕으로 각 스테이지의 버튼 (비)활성화 함수
    public void initStage()
    {
        Debug.Log("initStage 호출");

        int idx = 0;

        // 전부 비활성화(false)
        for (; idx < stageGrade.Length; idx++)
        {
            StageSelector.instance.setStageBtnStatus(idx,
                    // 저장된 성적을 기반으로 해당 리소스 이미지 추출 및 전달
                    gradeImages[(int)stageGrade[idx]]
                );
        }

        // 진행 가능한 스테이지들은 활성화
        for(idx = 0; idx < stageGrade.Length; idx++)
        {
            if(stageGrade[idx] != Grade.N)
            {
                StageSelector.instance.setStageBtnStatus(idx,
                    // 저장된 성적을 기반으로 해당 리소스 이미지 추출 및 전달
                    gradeImages[(int)stageGrade[idx]],
                    true,
                    stageGrade[idx] != Grade.F ? true : false
                    );

                // F성적이였다면 더 이상의 스테이지를 활성화하지 않음
                if (stageGrade[idx] == Grade.F) break;

                // 최종 스테이지를 '클리어' 했다면
                // : 최종 스테이지가 F나 N 성적이 아니라면 업그레이드 초기화 기능 지원
                if(idx == 16)
                {
                    UpgradeManager.instance.resetUpgradeBtn.SetActive(true);
                }
            }

            // 아직 진행하지 않은 스테이지라면
            else
            {
                // 그 중 첫번째 스테이지라면 활성화
                StageSelector.instance.setStageBtnStatus(idx,
                    // 저장된 성적을 기반으로 해당 리소스 이미지 추출 및 전달
                    gradeImages[(int)stageGrade[idx]],
                    true
                    );

                // 스테이지 진행 정도에 따라 플레이어 업그레이드 수준 결정 후 루프 종료
                UpgradeManager.instance.playerSelect.setUpgrade(idx / 4);

                break;
            }
        }
    }

    // 게임 시작시 StageSelector.selectStage() 에서 호출
    // 현재 시작된 스테이지의 idx 값을 전달받아서 저장
    // 스테이지 종료시 해당 스테이지(idx)에 성적 정보를 저장하기위해 필요한 함수
    //public void setStartStageIdx(int n)
    //{
    //    startStageIdx = n;
    //}

    // 해당 idx에 성적을 적용하는 함수
    // 서버로부터 성적을 load할 때 호출
    public void loadGrade(string[] grades) {

        print("loadGrade call");
        print("grades(param).Length >> " + grades.Length);

        int tmp;

        for (int i = 0; i < stageGrade.Length; i++)
        {
            tmp = int.Parse(grades[i]);
            // int -> enum casting
            stageGrade[i] = (Grade)tmp;
        }

        initStage();
    }

    // 성적 초기화 함수
    public void loadGrade(bool isReset)
    {
        print("성적 초기화 호출");
        print("stageGrade.Length >> " + stageGrade.Length);

        for (int i = 0; i < stageGrade.Length; i++)
        {
            //stageGrade[i] = (Grade)0;
            stageGrade[i] = Grade.N;
        }

        initStage();
    }

    // save
    public int getGrade(int idx, bool isInt = true )
    {
        return (int)stageGrade[idx];
    }

    // 플레이를 마친 스테이지에 성적 입력
    public void setGrade(Grade grade)
    {
        Debug.Log("setGrade 호출");
        Debug.Log("" + (StageSelector.instance.getTargetIndex() + 1) + "스테이지 성적 >> "+grade.ToString());
        stageGrade[StageSelector.instance.getTargetIndex()] = grade;
    }


    // 성적 3번 체크
    public bool getGradeCount(Grade grade)
    {
        int count = 0;

        for (int i = 0; i < stageGrade.Length; i++)
        {
            if (stageGrade[i] == grade) count++;
            if (count == 3) return true;
        }
        return false;
    }
}
