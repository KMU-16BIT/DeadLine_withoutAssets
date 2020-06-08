using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;


public class PlayerSkillController : MonoBehaviour
{
    /**
     * 
     * 각 타워의 스킬과 관련된 기능을 제어하는 스크립트
     * 
     * */

    public enum skillGaugeType { charging, full, consume }


    // 충전 게이지
    public Image skillGauge;
    public Image skillChangeGauge;

    //사운드
    public AudioSource skillSound;

    bool isCharging;

    // 상태별 게이지 이미지
    [SerializeField] Sprite[] skillGaugeSprites;

    // 스킬 수치
    [InfoBox("Eraser Skill Damage 값 수정 경로:\n" +
        "아무 Enemy 프리팹 / EnemyController.cs / rangeAttack() / 수치 조정")]
    int initGauge;

    int curGauge;

    // 스킬 충전 완료 상태 플래그
    bool skillOn;


    //#################

    // 스킬 프리팹
    public GameObject skillEffectPref;

    //#################

    private void Start()
    {
        skillOn = false;
        curGauge = 0;
        chargeGauge(0);
    }

    public bool getSkillOn() => skillOn;

    public void setSkillStatus(int _initGauge, int _skillDamage)
    {
        initGauge = _initGauge;

        skillEffectPref.GetComponent<SkillPlayerController>().setDamage(_skillDamage);
    }

    public void chargeGauge(int value)
    {
        
        curGauge += value;

        if (curGauge >= initGauge) {
            skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.full];
            skillOn = true;
            curGauge = initGauge;

            TutorialManager.instance.callTutorialMask_playerSkill();

        }

        skillGauge.fillAmount = curGauge / (float)initGauge;
    }

    public void emptyGauge()
    {
        curGauge = 0;
        skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.charging];
        skillGauge.fillAmount = curGauge / (float)initGauge;
    }


    //public bool SKILL()
    public void SKILL()
    {
            print("플레이어 스킬 사용 !!");

        if (skillOn)
        {
            skillOn = false;
            // 스킬 사운드 출력
            skillSound.Play();

            // 스킬 시전 애니메이션 출력
            PlayerController.instance.animator.SetTrigger("isSkill");

            skillGauge.sprite = skillGaugeSprites[(int)skillGaugeType.charging];

            // 스킬 사용과 동시에 curGauge가 충전되어
            // 연속으로 스킬을 사용하는 상황을 막기 위함
            // 스킬 시전이 종료되면 0으로 초기화되는 과정을 거침
            curGauge = -100;

            Instantiate(skillEffectPref, Vector3.zero, Quaternion.identity);

            // 사운드 재생 - 소리 없어서 주석
            //SettingSound.SS.playPlayerSound(true);

            // 튜토리얼 화살표 삭제
            TutorialManager.instance.DestroyArrow(4);


            return;
        }
        else
        { return; }
    }

    IEnumerator SkillCharging()
    {
        float chargeGauge = 0f;

        while(isCharging && chargeGauge < 0.7f)
        {
            chargeGauge += Time.deltaTime;

            skillChangeGauge.fillAmount = chargeGauge / 0.7f;

            yield return null;
        }

        // 스킬 사용
        if (isCharging)
            SKILL();
        isCharging = false;

        skillChangeGauge.fillAmount = 0;
    }

    public void SkillOn()
    {
        if (skillOn)
        {

            isCharging = true;
            StartCoroutine(SkillCharging());
        }
    }

    public void SkillCancel()
    {
        if (isCharging)
        {
            print("SKILLCANCEL!");


            isCharging = false;
            StopCoroutine(SkillCharging());
        }
    }
    

}
