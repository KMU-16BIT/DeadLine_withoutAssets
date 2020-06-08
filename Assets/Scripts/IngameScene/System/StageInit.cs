using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class StageInit : MonoBehaviour
{
    /**
     * 
     * 스테이지 시작시 필요한 셋팅들?
     * 
     * */

    [SerializeField] Image stageBackground;

    // Start is called before the first frame update
    void Start()
    {
        // 스테이지 배경 이미지 교체
        stageBackground.sprite = StageManager.instance.
            Level[StageManager.instance.getCurLevel()].levelBackgroundImg;

    }
    
}
