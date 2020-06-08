using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 오브젝트 풀링을 총괄하는 스크립트
 * 
 * */

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;

    Queue<GameObject>[] enemy;
    public List<GameObject> enemy_pref;

    // 총알 Prefab은 enum 순서대로 추가되어야 한다.
    // enum 번호가 pooling된 bullet 배열의 idx와 대응된다.
    // Sharp-Pen-Knife-Eraser-OneOff
    // OneOff : pool로 관리되지 않는, 일회성의 스킬들

    Queue<GameObject>[] bullet;
    public List<GameObject> bullet_pref;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        instance = this;

        initEnemy();
        initBullet();
    }
    
    void initEnemy()
    {
        // enemy들을 보관할 queue 공간 생성
        enemy = new Queue<GameObject>[enemy_pref.Count];

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i] = new Queue<GameObject>();
            //for (int j = 0; j < 30; j++)
            for (int j = 0; j < 50; j++)
            {
                GameObject obj = Instantiate(enemy_pref[i], this.transform);
                obj.SetActive(false);

                // 생성한 오브젝트 저장
                enemy[i].Enqueue(obj);
            }
        }   
    }
    public GameObject getEnemy(EnemyType enemyType)
    {
        GameObject obj = enemy[(int)enemyType].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    public void returnEnemy(GameObject obj, EnemyType enemyType)
    {
        obj.SetActive(false);
        enemy[(int)enemyType].Enqueue(obj);
    }
    
    void initBullet()
    {
        // bullet들을 보관할 queue 공간 생성
        bullet = new Queue<GameObject>[bullet_pref.Count];

        for (int i = 0; i < bullet.Length; i++)
        {
            bullet[i] = new Queue<GameObject>();
            for (int j = 0; j < 50; j++)
            {
                GameObject obj = Instantiate(bullet_pref[i], this.transform);

                // 지우개: 각 레벨별 총알 이미지가 모두 다름
                if(i == (int)TowerType.Eraser)
                {
                    int idx = UpgradeManager.instance.towerSelect[(int)TowerType.Eraser].getUpgrade();
                    obj.GetComponent<BulletController>().changeBulletImg(idx);
                }

                // 칼: 5레벨일때만 총알 이미지가 다름
                else if (i == (int)TowerType.Knife)
                {
                    if (UpgradeManager.instance.towerSelect[(int)TowerType.Knife].getUpgrade() == UpgradeManager.__UPGRADE_MAX__)
                        obj.GetComponent<BulletController>().changeBulletImg(1);
                }

                obj.SetActive(false);

                // 생성한 오브젝트 저장
                bullet[i].Enqueue(obj);
            }
        }
    }
    public GameObject getBullet(int idx)
    {
        GameObject obj = bullet[idx].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    public void returnBullet(GameObject obj, int idx)
    {
        obj.transform.position = transform.position;
        obj.transform.localScale = new Vector2(1, 1);

        obj.SetActive(false);
        bullet[idx].Enqueue(obj);
    }
}
