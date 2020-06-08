using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * 
 * F 성적을 기록한 후 출력되는 animation의 콤포넌트
 * 화면 터치시 홈 화면으로 돌아가는 기능을 제공한다.
 * 
 * */

public class FailAnimController : MonoBehaviour
{
    public void Home()
    {
        SceneManager.LoadSceneAsync("Mainmenu");
    }

}
