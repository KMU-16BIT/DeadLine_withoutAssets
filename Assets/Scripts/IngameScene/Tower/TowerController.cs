using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

/**
 * 
 * 각 타워를 제어하는 스크립트
 * 
 * 수정 사항>
 * 총알 풀에서 관리하게끔 할 것
 * 총알 속도를 포탑쪽에서 설정할 수 있게 할 것
 * 
 * */

// 총알 Prefab은 enum 순서대로 추가되어야 한다.
// enum 번호가 pooling된 bullet 배열의 idx와 대응된다.
public enum TowerType { Sharp, Pen, Knife, Eraser, OneOff }
// OneOff : pool로 관리되지 않는, 일회성의 스킬들


enum DebuffType { Soju, Sleepy, Phone }

[System.Serializable]
public class TowerStatus
{
    // 업그레이드 상태
    public int upgrade;

    // 초당 공격 속도
    // ex. 1.25 => 초당 1.25회 공격
    public float speed;

}

public class TowerController : MonoBehaviour
{
    public TowerType towerType;

    TowerStatus status = new TowerStatus();

    Animator animator;

    public BulletController bullet;

    Vector2 originPosition;

    // 원래 공격 속도
    float originSpeed;

    // Pen Tower의 Skill 사용 여부에 따라 총알에 변화를 주기 위한 플래그
    [HideInInspector] public bool isPenSkillMode;

    // 타워 스프라이트의 교체가 필요한 경우
    // 특정 레벨의 발사 직후에 사용
    [SerializeField] SpriteRenderer SR;

    [SerializeField] Sprite[] towerSprite;

    bool isAssaultNow;

    private void Awake()
    {
        // upgrade, status 등 load
        setStatus();
        animator = GetComponent<Animator>();

        

        // Upgrade가 max라면 타워가 커져서 스킬 게이지에 가리게 된다.
        // 이에 맞추어 스킬 게이지를 조금 아래로 내려주는 구문
        if (status.upgrade == 4) { // max
            transform.GetChild(0).GetChild(0).Translate(new Vector3(0, -0.5f));
        }


        // init
        isPenSkillMode = false;
        originPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 공격 시작
        AssultStart();
    }


    // 업그레이드 수준에 따른 status 초기화
    void setStatus()
    {
        // 업그레이드 수준 load
        status.upgrade = UpgradeManager.instance.towerSelect[(int)towerType].getUpgrade();

        // status 초기화
        bullet.status.speed = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].bulletSpeed;
        bullet.status.damage = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].damage;
        status.speed = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].assaultSpeed;
        originSpeed = status.speed;

        // change tower sprite
        gameObject.GetComponent<SpriteRenderer>().sprite =
            UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].towerImage;

        GetComponent<SkillController>().setSkillStatus(towerType,
            UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].skillCoolTime,
            UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].skillDurationTime,
            UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].skillValue
            );
    }

    // 자동 공격
    IEnumerator assault_c()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if(timer > status.speed)
            {
                timer = 0f;

                // 포탑이 뒤로 밀리는 현상 제거
                transform.position = originPosition;

                // 공격 명령
                if (TargetSelector.instance.target != null)
                    assault(TargetSelector.instance.target);
            }

            yield return null;
        }
    }

    IEnumerator lookTarget_c()
    {
        while (true)
        {
            if (TargetSelector.instance.target != null)
            {
                Vector3 dir = TargetSelector.instance.target.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }

            yield return null;
        }
    }

    // 작동 정지
    public void AssultStop(GameObject drunkEff, float timeLimit)
    {
        isAssaultNow = false;
        StopCoroutine("assault_c");
        StopCoroutine("lookTarget_c");

        StartCoroutine(DebuffRecoverer(DebuffType.Soju, drunkEff, timeLimit));
    }

    // 작동 시작
    public void AssultStart()
    {
        if (!isAssaultNow)
        {
            isAssaultNow = true;
            StartCoroutine("assault_c");
            StartCoroutine("lookTarget_c");
        }
    }
    
    IEnumerator DebuffRecoverer(DebuffType type, GameObject DebuffEff, float timeLimit, float originSpeed = -1)
    {
        // 방해효과 이펙트 표시
        GameObject obj = Instantiate(DebuffEff, transform.position, Quaternion.identity);

        float timer = 0f;
        while(timer < timeLimit)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 이펙트 삭제
        Destroy(obj);

        switch (type)
        {
            case DebuffType.Soju:
                AssultStart();
                break;

            case DebuffType.Phone:
                status.speed = originSpeed;
                break;
        }
        
    }

    public void AssultSpeedDown(GameObject debuffEff, float timeLimit)
    {
        //originSpeed = status.speed;

        status.speed = originSpeed * 2f;

        StartCoroutine(DebuffRecoverer(DebuffType.Phone, debuffEff, timeLimit, originSpeed));
    }
    

    void assault(GameObject target = null)
    {
        // anim
        animator.SetTrigger("shot");

        // target에 대해 공격 진행   
        GameObject obj = PoolingManager.instance.getBullet((int)towerType);
        obj.transform.eulerAngles = transform.eulerAngles;
        obj.transform.position = transform.position;

        // Pen이 스킬을 사용중인 경우
        if (isPenSkillMode)
        {
            obj.GetComponent<BulletController>().changeBulletImg(1);
            obj.transform.localScale = new Vector2(5, 5);
        }

        // Eraser의 경우, 투사체가 떠올랐다 떨어지는 듯한 효과를 주는 코루틴을 실행
        if (towerType == TowerType.Eraser)
        {
            obj.GetComponent<BulletController>().callJump_c();
        }

        // TowerType.Knife는 2발의 투사체를 추가로 발사
        else if (towerType == TowerType.Knife)
        {
            // target에 대해 공격 진행
            obj = PoolingManager.instance.getBullet((int)towerType);
            obj.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 30);
            obj.transform.position = transform.position;

            // target에 대해 공격 진행
            obj = PoolingManager.instance.getBullet((int)towerType);
            obj.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, -30);
            obj.transform.position = transform.position;
        }

        // 사운드 재생
        SettingSound.SS.playTowerAttackSound(towerType);
    }



    // 공격 속도에 변화를 부는 함수
    // SharpSkill
    public void BuffSkill(bool skillActive)
    {
        switch (towerType)
        {
            case TowerType.Sharp:
                if (skillActive)
                    status.speed = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].skillValue;
                else
                    status.speed = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].assaultSpeed;
                break;

            case TowerType.Pen:
                //if (skillActive)
                //    bullet.status.damage = 300;
                //else
                //    bullet.status.damage = UpgradeManager.instance.towerSelect[(int)towerType].towerUpgrade[status.upgrade].damage;


                break;
        }

    }

    // shoot의 과거분사 ㅎㅎ
    // 총알이 발사되었을 때 타워를 '총알이 없는 스프라이트'로 교체
    // 해당 타워의 공격 속도를 받아와서 1/2 를 통과하는 지점에 다시 총알이 장전된 스프라이트로 교체
    IEnumerator shot_c(float waitTime)
    {
        changeTowerSprite(1);
        yield return new WaitForSeconds(waitTime);
        changeTowerSprite(0);
    }

    // 코루틴 시작 함수. Animation Event를 통해 총알 발사 시점에 호출
    void shot()
    {
        // 타워 공격 속도의 1/2가 지난 시점에 총알 장전한 스프라이트로 교체
        StartCoroutine("shot_c", status.speed / 2);
    }


    void changeTowerSprite( int idx)
    {
        int curUpgrade = UpgradeManager.instance.towerSelect[(int)towerType].getUpgrade();
        idx += 2 * curUpgrade; 

        // Full Upgrade 되어있는 Knife, Eraser 타워에 대해서만 실행
        switch (towerType)
        {
            case TowerType.Eraser:
                SR.sprite = towerSprite[idx];
                break;

            case TowerType.Knife:
                
                SR.sprite = towerSprite[idx];
                break;
        }
    }

}
