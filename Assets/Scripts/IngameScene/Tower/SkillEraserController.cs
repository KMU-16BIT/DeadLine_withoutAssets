using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEraserController : MonoBehaviour
{

    public delegate void RangeAttackHandler(int damage);

    public static event RangeAttackHandler DoEnemiesAttack;

    [SerializeField] int damage;
    

    public void setDamage(int _damage)
    {
        print("setDamage : " + _damage);
        damage = _damage;
    }

    public void destroySelf()
    {
        Destroy(gameObject);
    }

    // call by animation event
    public void rangeAttack()
    {
        if (DoEnemiesAttack != null)
        {
            print("지우개 공격, 데미지 : " + damage);
            DoEnemiesAttack(damage);
        }
    }

}
