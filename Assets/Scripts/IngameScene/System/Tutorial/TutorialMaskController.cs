using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMaskController : MonoBehaviour
{
    
    
    // 마스크 종료.
    // 플레이어 스킬 사용시 호출
    public void MaskOff()
    {
        if (!StageManager.instance.IsTutorialStage()) return;

        // 시간 되돌리기
        TimeScaleManager.instance.setSpeedTutorialMask();
        

        // 마스크 삭제
        Destroy(gameObject);
    }

    // 재귀호출
    public void CallOffTutorialMask_playerSkill()
    {
        TutorialManager.instance.OffTutorialMask_playerSkill();
    }

}
