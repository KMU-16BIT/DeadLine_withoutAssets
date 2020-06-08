using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;


public class SkillController : MonoBehaviour
{
    /**
     * 
     * 각 타워의 스킬과 관련된 기능을 제어하는 스크립트
     * 
     * */

    public enum skillGaugeType{charging, full, consume}


    // 충전 게이지
    public Image skillGauge;
    // 상태별 게이지 이미지
    [SerializeField] Sprite[] skillGaugeSprites;

    // 스킬 수치
    [InfoBox("Eraser Skill Damage 값 수정 경로:\n" +
        "아무 Enemy 프리팹 / EnemyController.cs / rangeAttack() / 수치 조정")]
    float initCoolTime; // 쿨타임
    float initDurationTime; // 지속 시간
    
    float curCoolTime;

    // 스킬 충전 완료 상태 플래그
    bool skillOn;


    //#################

    // 스킬 프리팹
    public GameObject skillEffectPref;

    //#################

    private void Start()
    {
        skillOn = false;
        StartCoroutine(chargeGauge());
    }

    public bool getSkillOn() => skillOn;

    // 쿨타임을 충전하는 코루틴 함수
    IEnumerator chargeGauge()
    {
        curCoolTime = 0f;
        skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.charging]; 

        while (true)
        {
            skillGauge.fillAmount = curCoolTime / initCoolTime;

            curCoolTime += Time.deltaTime;

            if (curCoolTime > initCoolTime)
            {
                // 스킬 온
                skillOn = true;
                skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.full];
                break;
            }

            yield return null;
        }
    }

    public void setSkillStatus(TowerType towerType, float _cooltime, float _durationTime, float _skillValue)
    {
        initCoolTime = _cooltime;
        initDurationTime = _durationTime;

        switch (towerType)
        {
            case TowerType.Sharp:
            case TowerType.Pen:
                // 스킬 시전 중 공격 속도 
                break;

            case TowerType.Eraser:
                skillEffectPref.GetComponent<SkillEraserController>().setDamage((int)_skillValue);
                break;
        }
    }

    public void SKILL()
    {
        if (skillOn)
        {
            skillOn = false;

            skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.consume];

            switch (GetComponent<TowerController>().towerType)
            {
                case TowerType.Sharp:
                    StartCoroutine(SKILL_SHARP());
                    break;
                case TowerType.Pen:
                    StartCoroutine(SKILL_PEN());
                    break;
                case TowerType.Eraser:
                    StartCoroutine(SKILL_ERASER());
                    break;

                case TowerType.Knife:
                    StartCoroutine(SKILL_KNIFE());
                    break;
            }

            SettingSound.SS.playTowerSkillSound(GetComponent<TowerController>().towerType);

            // 튜토리얼 화살표 삭제
            TutorialManager.instance.DestroyArrow((int)GetComponent<TowerController>().towerType);

        }
        else
        {
            print("아직 충전이 부족합니다");
        }
    }

//################ 각 타워별 스킬 ###################


    IEnumerator SKILL_SHARP()
    {
        // 스킬 시전이 종료됨을 알려주는 플래그
        float leftTime = initDurationTime;

        // 공격 속도
        GetComponent<TowerController>().BuffSkill(true);

        while (leftTime > 0f)
        {
            leftTime -= Time.deltaTime;

            skillGauge.fillAmount = leftTime / initDurationTime;

            yield return null;
        }

        // 공격 속도 초기화
        GetComponent<TowerController>().BuffSkill(false);

        StartCoroutine(chargeGauge());
    }

    IEnumerator SKILL_PEN()
    {
        // 스킬 시전이 종료된을 알려주는 플래그
        float skillTimer = 5.0f;
        float leftTime = skillTimer;

        // 총알 크기 x10
        GetComponent<TowerController>().isPenSkillMode = true;

        while (leftTime > 0f)
        {
            leftTime -= Time.deltaTime;

            skillGauge.fillAmount = leftTime / skillTimer;

            yield return null;
        }

        // 총알 크기 x1
        GetComponent<TowerController>().isPenSkillMode = false;

        StartCoroutine(chargeGauge());

    }

    IEnumerator SKILL_KNIFE()
    {
        Instantiate(skillEffectPref, Vector3.zero, Quaternion.identity);

        yield return null;

        StartCoroutine(chargeGauge());
    }

    IEnumerator SKILL_ERASER()
    {
        Instantiate(skillEffectPref, new Vector3(0, 1.5f), Quaternion.identity);

        yield return null;

        StartCoroutine(chargeGauge());
    }

    
}
