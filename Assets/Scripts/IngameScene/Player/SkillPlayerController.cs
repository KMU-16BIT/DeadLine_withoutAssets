using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPlayerController : MonoBehaviour
{

    public delegate void RangeAttackHandler(int damage);

    public static event RangeAttackHandler DoEnemiesAttack;

    int damage;

    public void setDamage(int _damage) => damage = _damage;

    public void destroySelf()
    {
        PlayerController.instance.PSK.emptyGauge();
        Destroy(gameObject);
    }

    // call by animation event
    public void rangeAttack()
    {
        if (DoEnemiesAttack != null)
        {
            print("RangeAttack!! : " + damage);

            //DoEnemiesAttack(damage);
            DoEnemiesAttack(100);
        }
    }

}
