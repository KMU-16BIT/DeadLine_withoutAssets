using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class PlayerStatus
{
    // 공격력
    public int damage;

    // 스킬 충전에 필요한 게이지
    public int skillGauge;
    
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    PlayerStatus status = new PlayerStatus();

    public PlayerSkillController PSK;

    // 기본 공격 이팩트
    public GameObject assaultInkEff;

    public Animator animator;
    public Image hpBar;
    public Sprite[] brainIcons;
    public Image brainIcon;
    public int initHP;
    int curHP;

    IEnumerator coroutine;

    private void Awake()
    {
        instance = this;

        setStatus();

        coroutine = attackedEff();
    }

    private void Start()
    {
        curHP = initHP;
        hpBar.color = new Color(0.1f, 0.7f, 0.2f); // green
    }

    void setStatus()
    {
        // 스테이지 진행 정도를 토대로 현 업그레이드 수준 로드
        int curUpgrade = UpgradeManager.instance.playerSelect.getUpgrade();

        // 플레이어 레벨 전달(1~5)
        // 업그레이드 정도를 토대로 플레이어 외형 및 애니메이션 적용
        animator.SetInteger("playerLevel", curUpgrade);

        // 스텟 적용
        status.damage = UpgradeManager.instance.playerSelect.playerUpgrade[curUpgrade].damage;
        status.skillGauge = UpgradeManager.instance.playerSelect.playerUpgrade[curUpgrade].skillGauge;

        // skill status
        PSK.setSkillStatus(status.skillGauge, 9999);
    }

    // hp 감소
    public void attacked(int hp)
    {
        //StopCoroutine(coroutine);
        //StartCoroutine(coroutine);
        StartCoroutine(attackedEff());

        curHP -= hp;
        hpBar.fillAmount = (float)curHP / initHP;

        if (100 > curHP && curHP > 75) brainIcon.sprite = brainIcons[1]; 
        if (75 >= curHP && curHP > 50) brainIcon.sprite = brainIcons[2]; 
        if (50 >= curHP && curHP > 25) brainIcon.sprite = brainIcons[3]; 
        if (25 >= curHP && curHP > 0) brainIcon.sprite = brainIcons[4];

        // 종료
        if (curHP <= 0) GameOverChecker.instance.GameOver(curHP);
        
    }

    // 공격
    public void assault()
    {
        print("call assault");

        TutorialManager.instance.DestroyArrow(4);

        

        if (ObstacleSelector.instance.target != null)
        {
            animator.SetTrigger("isAttack");


            EnemyController EC = ObstacleSelector.instance.target.GetComponent<EnemyController>();

            print("플레이어 기본공격 : " + status.damage);
            EC.attacked(status.damage);

            // 사운드 재생
            SettingSound.SS.playPlayerSound();

            Instantiate(assaultInkEff, EC.transform.position, Quaternion.identity);
        }
        else
        {
            print("공격 대상이 없습니다");
        }
        
    }

    IEnumerator attackedEff()
    {
        hpBar.color = new Color(.8f, 0f, 0f); // red

        yield return new WaitForSeconds(0.5f);

        hpBar.color = new Color(0.1f, 0.7f, 0.2f); // green


        yield return null;
    }

    public void chargeGauge(int value)
    {
        PSK.chargeGauge(value);
    }

    public int getHP() => curHP;


    
}
