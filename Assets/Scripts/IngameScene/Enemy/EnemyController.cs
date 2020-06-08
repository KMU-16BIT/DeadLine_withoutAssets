using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * Enemy를 제어하는 스크립트
 * 
 * */

// Level [value] Enemy [value]
public enum EnemyType {
    // Normal Enemies
    L1E1, L1E2, L1E3, L1Boss,
    L2E1, L2E2, L2E3, L2Boss,
    L3E1, L3E2, L3E3, L3Boss,
    L4E1, L4E2, L4E3, L4Boss,
    L5E1, L5E2, L5E3, L5Boss,

    // Obstacle Enemies
    Soju, Sleepy, Phone
}

[System.Serializable]
// init status
class EnemyStatus
{
    // 체력
    public int hp;
    // 공격력
    public int damage;
    // 이동 속도
    public float speed;

    // 게이지 충전량
    public int gaugeValue;
}

// 현재 status. 체력 등이 깎이는 경우를 이곳에서 구현
class curStatus
{
    // 체력
    public int hp;
    // 공격력
    public int damage;
    // 이동 속도
    public float speed;
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    EnemyStatus initialStatus;

    curStatus status = new curStatus();
    
    // LEVEL-1 >> PoolingManager에서 각 Queue 배열의 인덱스와 대응
    [SerializeField] EnemyType enemyType;

    [SerializeField]GameObject ObstacleEffPref;

    // ### UI ###
    public Image hpBar; // 체력 바
    public Image hpBarBack; // 체력 바 배경



    // ### Flags ###
    bool moveFlag;

    private void Awake()
    {
        //hpBar = transform.Find("hpBar").GetComponent<Image>();
    }

    private void OnEnable()
    {
        initStatus();

        SkillKnifeController.DoEnemiesKnockBack += knockBack;
        //SkillEraserController.DoEnemiesAttack += rangeAttack;

        // 방해요소 몬스터 셋팅
        if (enemyType >= EnemyType.Soju)
        {
            // 지우개 스킬 공격에 피해를 입지 않는다

            // 방해요소 특수 이펙트
            //ObstacleEff(true);
            MaterialContainer.instance.MaterialChanger(gameObject, MaterialType.OBSTACLE);
        }
        else
        {
            // 지우개 스킬 공격에 피해를 입는다
            SkillEraserController.DoEnemiesAttack += attacked;
        }

        SkillPlayerController.DoEnemiesAttack += attacked;

        StartCoroutine(move_c());
    }
    

    private void OnDisable()
    {
        SkillKnifeController.DoEnemiesKnockBack -= knockBack;
        //SkillEraserController.DoEnemiesAttack -= rangeAttack;
        SkillEraserController.DoEnemiesAttack -= attacked;
        SkillPlayerController.DoEnemiesAttack -= attacked;
    }

    void initStatus()
    {
        status.hp = initialStatus.hp;
        status.speed = initialStatus.speed;
        status.damage = initialStatus.damage;

        // 피격 이전에는 체력 바가 보이지 않음
        //hpBar = transform.Find("hpBar").GetComponent<Image>();
        if (hpBar)
        {
            hpBar.fillAmount = status.hp / (float)initialStatus.hp;
            hpBar.enabled = false;
            hpBarBack.enabled = false;
        }
    }

    // 아래로 이동
    IEnumerator move_c()
    {
        moveFlag = true;
        while (moveFlag)
        {
            transform.Translate(Vector3.down * status.speed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }

    IEnumerator knockBack_c()
    {

        float timer = 0f;
        Vector3 originPos = transform.position;

        Vector3 knockBackRange = (enemyType == EnemyType.L1Boss || enemyType == EnemyType.L2Boss || enemyType == EnemyType.L3Boss || enemyType == EnemyType.L4Boss || enemyType == EnemyType.L5Boss) ? new Vector3(0, 0.666f) : new Vector3(0, 2);

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(originPos, originPos + knockBackRange, timer * 2);
            
            yield return null;
        }
    }

    void knockBack()
    {
        // 화면 일정 위치 아래일 때에만 넉백 실행
        if (transform.position.y < 1)
        {
            StartCoroutine(knockBack_c());
        }
    }


    // hp 감소
    public void attacked(int damage)
    {
        // 지우개 스킬은 101부터 데미지 시작
        // 일부 범위
        // 플레이어 스킬은 100으로 4회
        if(damage > 100)
        {
            if (transform.position.y < 0) return;
        }
        status.hp -= damage;
        hpBar.enabled = true;
        hpBarBack.enabled = true;
        hpBar.fillAmount = status.hp / (float)initialStatus.hp;
        

        //if (status.hp <= 0) die();
        if (status.hp <= 0) StartCoroutine("die_c");
    }

    //// 방해요소 특수효과를 활성화하는 함수
    //public void ObstacleEff(bool isOn)
    //{
    //    if (isOn)
    //    {
    //        GetComponent<SpriteRenderer>().material = MaterialContainer.instance.materialList._obstacleEff;
    //    }
    //    else
    //    {
    //        GetComponent<SpriteRenderer>().material = MaterialContainer.instance.materialList._emptyShader;
    //    }
    //}

    IEnumerator die_c()
    {
        hpBarBack.enabled = false;

        if (moveFlag)
        {
            // 제자리에 멈추도록 이동 코루틴 종료
            moveFlag = false;

            // 몬스터가 살아있는 경우와 죽은 경우를 구분하기위한 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("Dying");


            // Obstacle 방해 요소라면
            if (enemyType >= EnemyType.Soju)
            {
                PlayerController.instance.chargeGauge(initialStatus.gaugeValue);

                // 특수 효과 이펙트(셰이더) 종료
                //ObstacleEff(false);
            }


            //material.EnableKeyword("FADE_ON");
            MaterialContainer.instance.MaterialChanger(gameObject, MaterialType.DIE);

            // 사망 셰이더 적용
            Material material = GetComponent<SpriteRenderer>().material;

            float timer = 0f;

            while (timer < 1.0f)
            {
                timer += Time.deltaTime;

                material.SetFloat("_FadeAmount", Mathf.Lerp(0, 1, timer));

                yield return null;
            }


            // Material 초기화
            //material.SetFloat("_FadeAmount", 0f);
            //material.DisableKeyword("FADE_ON");
            MaterialContainer.instance.MaterialChanger(gameObject,
                enemyType >= EnemyType.Soju ? MaterialType.OBSTACLE : MaterialType.EMPTY
                );

            // 풀 반환
            die();
        }
    }


    // 죽었을 경우
    void die()
    {
        // 레이어 초기화
        // 일반 몬스터
        if (enemyType <= EnemyType.L5Boss)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        // 방해 요소
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Obstacle");
        }

        // 비활성화 한 요소 초기화
        hpBarBack.enabled = true;

        PoolingManager.instance.returnEnemy(gameObject, enemyType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // DestroyArea에 닿았을 경우. Pool 반환
        if (collision.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            PoolingManager.instance.returnEnemy(gameObject, enemyType);

            // 플레이어 hp 감소
            if (collision.CompareTag("bottomDestroyArea"))
            {
                // 진동
                VibrationManager.instance.onVibration();

                // 방해 요소 특수 효과
                switch (enemyType)
                {
                    case EnemyType.Soju:
                        // 타워 하나가 술에 취해 작동을 중지
                        int stopTower = Random.Range(0, 4);
                        TowerStorage.instance.towers[stopTower].GetComponent<TowerController>().AssultStop(ObstacleEffPref, 3);                       
                        break;


                    case EnemyType.Sleepy:
                        Instantiate(ObstacleEffPref);
                        break;


                    case EnemyType.Phone:
                        for(int i = 0; i < TowerStorage.instance.towers.Length; i++)
                        {
                            TowerStorage.instance.towers[i].GetComponent<TowerController>().AssultSpeedDown(ObstacleEffPref, 3);
                        }
                        break;
                }
                SettingSound.SS.playObstaclesSound(enemyType);
                PlayerController.instance.attacked(status.damage);
            }
        }

        // 피격시
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            BulletController BC = collision.gameObject.GetComponent<BulletController>();

            // 총알 반환
            // TowerType.Pen 의 경우 관통 구현을 위해 총알을 반환하지 않음
            // TowerType.Eraser 의 경우 포격 구현을 위해 총알을 반환하지 않음
            if (BC.towerType != TowerType.Pen && BC.towerType != TowerType.Eraser && BC.towerType != TowerType.OneOff)
                PoolingManager.instance.returnBullet(collision.gameObject,
                    (int)BC.towerType);

            // hp 감소
            attacked(BC.status.damage);
        }
        
        
    }
}
