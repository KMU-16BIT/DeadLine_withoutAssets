using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKnifeController : MonoBehaviour
{
    public delegate void KnockBackHandler();

    public static event KnockBackHandler DoEnemiesKnockBack;
    
    public void destroySelf(float delayTime = 0f)
    {
        StartCoroutine(timeWaiter(delayTime));
    }

    IEnumerator timeWaiter(float timeLimit)
    {
        float timer = 0f;
        while(timer < timeLimit)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void knockBack()
    {
        if (DoEnemiesKnockBack != null)
            DoEnemiesKnockBack();
    }
    
}
