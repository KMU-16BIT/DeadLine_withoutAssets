using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneController : MonoBehaviour
{
    TutorialManager TM;

    

    private void Awake()
    {
        TM = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
    }

    // 씬 재생 종료
    public void DoneTuto()
    {
        // 스테이지 정상 시작을 위한 프로세스
        TM.EndTutorialScene();

        // 본인 객체 삭제
        Destroy(gameObject);
    }

    public void DoneEnding()
    {
        SceneManager.LoadSceneAsync("Mainmenu");
    }
}
