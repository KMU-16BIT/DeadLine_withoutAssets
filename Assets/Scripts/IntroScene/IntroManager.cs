using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using PlayFab;
using PlayFab.ClientModels;
using System;

public static class ErrorCode
{
    // ERROR-A-1 : 데이터 로드 실패 
    // ERROR-A-2 : 유저 머니 로드 실패 
    // ERROR-B-1 : 업그레이드 수치 저장 실패
    // ERROR-B-2 : 성정 저장 실패
    // ERROR-C-1 : 재화 사용 실패
    public static string errorCode;
    public static bool onErrorUI;
}

public class IntroManager : MonoBehaviour
{
    public Text Intro_Message;
    bool connected;


    [Header("ERROR")]
    public Text errorCodeText;
    public GameObject ErrorUI;

    private void Awake()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        // 인터넷 연결 여부 검사
        // 인터넷에 연결이 되어 있지 않으면
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            connected = false;
            Intro_Message.text = "인터넷 연결을 해주세요!";
        }
        else // 연결 되어 있으면
        {
            // 구글 로그인 시도
             GoogleLogin();
           
        }
    }

    // 구글 로그인 함수
    void GoogleLogin()
    {
        Social.localUser.Authenticate((success) =>
        {
            Intro_Message.text = "구글 로그인 시도";
            if (success) PlayFabLogin();
            else Intro_Message.text = "구글 로그인 실패!";
        });
    }

    // 구글 로그인 성공하면 유저 이메일로 플레이팹 로그인 시도
    // 실패시 플레이팹 회원가입 함수 호출
    // 성공시 연결 플래그 트루
    void PlayFabLogin()
    {
        Intro_Message.text = "서버 연결 시도";
       var LoginRequest = new LoginWithEmailAddressRequest { Email = Social.localUser.id + "@16bit.com", Password = Social.localUser.id};
        PlayFabClientAPI.LoginWithEmailAddress(LoginRequest, (success) =>
        {
            UserData.UserPlayFabId = success.PlayFabId;
            connected = true;
            Intro_Message.text = "화면을 터치하세요";
        }, (faill) => PlayFabRegister());
    }

    // 플레이팹 회원 가입
    // 구글 유저 닉네임으로 회원 가입 시도 후 정상이면 로그인, 실패면 실패사유 출력
    void PlayFabRegister()
    {

        var RegisterRequest = new RegisterPlayFabUserRequest { Email = Social.localUser.id + "@16bit.com", Password = Social.localUser.id, Username = RandomUserName() };
        PlayFabClientAPI.RegisterPlayFabUser(RegisterRequest, (success) => PlayFabLogin(), (faill) => Intro_Message.text = faill.ToString());
    }

     string RandomUserName()
    {
        string strPool = "abcdefghijklamopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //문자 생성 풀
        char[] chRandom = new char[15];
        System.Random random = new System.Random();

        for (int i = 0; i < 15; i++)
        {
            chRandom[i] = strPool[random.Next(strPool.Length)];
        }
        string strRet = new String(chRandom);  // char to string

        return strRet;
    }

    private void FixedUpdate()
    {
        if ( connected && Input.GetMouseButtonDown(0))
        {
            Social.ReportProgress(GPGSIds.achievement_1, 100, (bool success) => { });
            SceneManager.LoadSceneAsync(1);
        }

    }

    // 에러 UI 셋팅
    public void onErrorUI()
    {
        ErrorUI.SetActive(true);
        errorCodeText.text = "< " + ErrorCode.errorCode + " >";
    }

    // 에러 UI 닫음
    public void offErrorUI()
    {
        ErrorCode.onErrorUI = false;
        ErrorUI.SetActive(false);
    }
}
