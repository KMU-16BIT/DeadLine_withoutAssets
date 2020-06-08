using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 투사체의 공격 속도를 설정하는 스크립트
 * 
 * */

[System.Serializable]
public class BulletStatus
{
    // 투사체 속도
    public float speed;
    // 공격력
    public int damage;
}

public class BulletController : MonoBehaviour
{
    public TowerType towerType;

    [HideInInspector]
    public BulletStatus status;

    [SerializeField] GameObject bomb;

    // 총알의 스프라이트 변경이 필요한 경우
    public Sprite []bulletImg;
    
    // 공격력
    int damage;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        if (towerType != TowerType.OneOff)
            StartCoroutine("shot_c");
    }

    private void OnDisable()
    {
        // 총알 스프라이트 교체가 필요한 경우
        switch (towerType)
        {
            case TowerType.Pen:
                changeBulletImg(0);
                break;
        }
    }

    public void changeBulletImg(int idx)
    {
        GetComponent<SpriteRenderer>().sprite = bulletImg[idx];
    }

    IEnumerator shot_c()
    {
        switch (towerType)
        {
            // 지우개의 경우, 해당 위치를 포격
            case TowerType.Eraser:

                while (true)
                {
                    // 직선방향으로 발사
                    transform.Translate(Vector3.up * status.speed * Time.deltaTime, Space.Self);

                    yield return null;
                }
                break;

            // 그 외의 경우 발사
            default:
                while (true)
                {
                    // 직선방향으로 발사
                    transform.Translate(Vector3.up * status.speed * Time.deltaTime, Space.Self);

                    yield return null;
                }
            break;
        }

    }

    // ############# Eraser bullet special effect ################

    // 포격 위치로 날아가며 0~0.5 까지는 포탄 크기 커짐
    // 0.5~1까지는 포탄 크기 작아짐
    public void callJump_c()
    { StartCoroutine("jump_c"); }
    IEnumerator jump_c()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = TargetSelector.instance.target.transform.position;


        float middleDist = Vector2.Distance(startPos, endPos) / 2f;
        float movedDist = 0f;

        while(movedDist < middleDist){
            Debug.DrawLine(startPos, endPos, Color.red);

            movedDist = Vector2.Distance(startPos, transform.position);

            transform.localScale = new Vector2(
                Mathf.Lerp(1, 2, movedDist / middleDist),
                Mathf.Lerp(1, 2, movedDist / middleDist)
                );

            yield return null;
        }

        startPos = transform.position;
        movedDist = 0f;
        

        while(movedDist < middleDist){
            Debug.DrawLine(startPos, endPos, Color.blue);

            movedDist = Vector2.Distance(startPos, transform.position);

            transform.localScale = new Vector2(
                Mathf.Lerp(2, 1, movedDist/ middleDist),
                Mathf.Lerp(2, 1, movedDist/ middleDist)
                );
            
            yield return null;
        }

        // 폭발 이펙트
        Instantiate(bomb, transform.position, Quaternion.identity);

        PoolingManager.instance.returnBullet(gameObject, (int)TowerType.Eraser);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // DestroyArea에 닿았을 경우. Pool 반환
        if (collision.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            if (!collision.CompareTag("bottomDestroyArea"))
            {
                PoolingManager.instance.returnBullet(gameObject, (int)towerType);
            }
        }
    }

    void DestroyCollider()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    // TowerType.OneOff 로써 일회성으로 사용되는 총알 등의 경우
    // Anim Event에서 스스로 Destroy 하는 함수를 호출한다
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
