using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.IO;

public static class UserData
{
    // 플레이펩 로그인 후 고유 유저 아이디
    public static string UserPlayFabId;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public static bool loadPopUpflag;

    public Text userID;

    [HideInInspector]
    public bool useTestAccount = false;

    // 사운드 값 저장 경로
    string path;

    // 테스트 아이디 비번
    // 추후 구글과 연동하여 고유 아이디를 가지도록 해야함!
    string id;
    string pw;

    // 업그레이드 수치를 스트링으로 합치기 위함
    StringBuilder upgradeValue = new StringBuilder();

    [SerializeField] GameObject popUpScreen;


    // 버튼 시각적
    // 언젠간 삭제해ㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐ
    [HideIf("useTestAccount")]
    [Button("== 테스터 계정 비활성화 상태 ==", 90), GUIColor(0, 1, 0)]
    private void DisableTestAccount() { useTestAccount = true; }

    [ShowIf("useTestAccount")]
    [Button("★★ 테스터 계정 활성화 상태 ★★", 90), GUIColor(1, 0.1f, 0)]
    private void EnableTestAccount() { useTestAccount = false; }

    // **************************************************************
    private void Awake()
    {
        instance = this;
        loadPopUpflag = false;
        path = Application.persistentDataPath + "/setting.txt";

        if (useTestAccount)
        {
            id = "deadline@k.circle";
            pw = "deadline";

            Login();
        }
        else
        {
            Init();
        }

        loadSettingValue();
    }

    // 로그인 함수
    void Login()
    {
        var loginRequest = new LoginWithEmailAddressRequest { Email = id, Password = pw };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest,
            (result) =>
            {
                UserData.UserPlayFabId = result.PlayFabId;


                //save cur stage grades before load
                Init();
            },
            (error) => Register());
    }

    // 회원가입 함수
    void Register()
    {
        // id를 @앞까지만 따와서 유저이름으로 처리
        int cutId = id.IndexOf("@");
        string username = id.Substring(0, cutId);

        var registerRequest = new RegisterPlayFabUserRequest { Email = id, Password = pw, Username = username };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, (result) => Login(), (error) => Debug.Log("회원가입 실패!"));
    }

    void Init()
    {
        userID.text = "ID : " +UserData.UserPlayFabId;

        LoadPopUpScreen(true);

        if (GradeManager.isFirstAwake)
        {
            print("Login with Only Load");
            GradeManager.isFirstAwake = false;
            LoadData(); // 첫 시작에는 Load만 진행(Save할 값 x)
        }
        else
        {
            print("Login with Save");
            SaveData("grade"); // 이후(스테이지 진행 등)에는 정보를 바로 Save 후 Load
        }
    }

    public void SaveData(string saveName)
    {
        LoadPopUpScreen(true);
        print("call SaveData()");

        if (saveName == "upgrade")
        {
            #region "업그레이드 수치 저장"
            // key : upgrade
            // value : upgrade value | upgrade value 형식으로 저장

            // 업그레이드 벨류를 나열
            for (int i = 0; i < UpgradeManager.instance.towerSelect.Length; i++)
            {
                upgradeValue.Append(UpgradeManager.instance.towerSelect[i].getUpgrade());
                if (i < UpgradeManager.instance.towerSelect.Length - 1)
                    upgradeValue.Append("|");
            }

            // 데이터 저장
            var upgradeRequest = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Upgrade", upgradeValue.ToString() } } };
            PlayFabClientAPI.UpdateUserData(upgradeRequest,
                (result) =>
                {
                    Debug.Log("업그레이드 데이터 저장 완료");
                    LoadPopUpScreen(false);

                },
                (error) =>
                {
                    Error("ERROR-B-1");
                    LoadPopUpScreen(false);
                }
                );

            // 다음 저장을 위해 데이터 초기화
            upgradeValue.Clear();
            #endregion
        }
        else if (saveName == "grade")
        {
            #region "성적 저장"
            // key : upgrade
            // value : upgrade value | upgrade value 형식으로 저장

            // 업그레이드 벨류를 나열
            for (int i = 0; i < 17; i++)
            {
                upgradeValue.Append(GradeManager.instance.getGrade(i));

                if (i < 16)
                    upgradeValue.Append("|");
            }

            // 데이터 저장
            var upgradeRequest = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Grade", upgradeValue.ToString() } } };
            PlayFabClientAPI.UpdateUserData(upgradeRequest,
                (result) =>
                {
                    Debug.Log("성적 데이터 저장 완료");

                    // Save한 데이터 서버에서 Load
                    LoadData();

                },
                (error) =>
                {
                    Error("ERROR-B-2");
                    LoadPopUpScreen(false);
                }
                );

            upgradeValue.Clear();
            #endregion
        }
    }

    public void saveMoney(int money, bool isSub = false)
    {
        Debug.Log("saveMoeny callled");

        if (!isSub)
        {
            var moneyrequest = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "GD", Amount = money };
            PlayFabClientAPI.AddUserVirtualCurrency(moneyrequest, (resultmoney) => { MoneyManager.instance.setMoneyText(resultmoney.Balance); }, (errormoney) => Error("ERROR-A-2"));
        }
        else
        {
            loadPopUpflag = true;

            var moneycheckrequest = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "GD", Amount = 0 };
            PlayFabClientAPI.AddUserVirtualCurrency(moneycheckrequest, (resultmoney) =>
            {
                //// 구매 할수 있는지 체크
                //if (resultmoney.Balance - money < 0)
                //{
                //    Debug.Log("니 돈없음 ㅅㄱ 팝업창 띄워 주세요.");
                //    return;
                //}

                // 돈 사용
                var moneylessrequest = new SubtractUserVirtualCurrencyRequest() { VirtualCurrency = "GD", Amount = money };
                PlayFabClientAPI.SubtractUserVirtualCurrency(moneylessrequest, (resultmoney1) =>
                {
                    Debug.Log(money + "원 사용 완료 (1/2)");
                    loadPopUpflag = false;
                    MoneyManager.instance.setMoneyText(resultmoney1.Balance);
                    Debug.Log(money + "원 사용 완료 (2/2)");
                }, (errormoney1) => Error("ERROR-A-1"));
            }, (errormoney) => Error("ERROR-A-1"));
        }
        Debug.Log("saveMoeny done");
    }

    // 성적 및 스테이지 초기화
    public void DevelopMode_ResetGrade()
    {
        LoadPopUpScreen(true);

        #region "성적 저장"
        // key : upgrade
        // value : upgrade value | upgrade value 형식으로 저장

        // 업그레이드 벨류를 나열
        for (int i = 0; i < 17; i++)
        {
            // GradeManager.Grade.N
            upgradeValue.Append((int)Grade.N);

            if (i < 16)
                upgradeValue.Append("|");
        }

        // 데이터 저장
        var upgradeRequest = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Grade", upgradeValue.ToString() } } };
        PlayFabClientAPI.UpdateUserData(upgradeRequest,
            (result) =>
            {
                Debug.Log("이거슨 개발자 모드임 - 성적 데이터 저장 완료");

                saveMoney(MoneyManager.instance.getMoneyText(), true);

                LoadData();
            },
            (error) =>
            {
                LoadPopUpScreen(false);
                Debug.Log("이거슨 개발자 모드임 - 성적 데이터 저장 실패");
            }
            );

        upgradeValue.Clear();


        #endregion
    }

    // 성적 및 스테이지 모두 오픈
    public void DevelopMode_SaveGrade()
    {
        LoadPopUpScreen(true);

        #region "성적 저장"
        // key : upgrade
        // value : upgrade value | upgrade value 형식으로 저장

        // 업그레이드 벨류를 나열
        for (int i = 0; i < 17; i++)
        {
            // GradeManager.Grade.B
            upgradeValue.Append((int)Grade.B);

            if (i < 16)
                upgradeValue.Append("|");
        }

        // 데이터 저장
        var upgradeRequest = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Grade", upgradeValue.ToString() } } };
        PlayFabClientAPI.UpdateUserData(upgradeRequest,
            (result) =>
            {
                Debug.Log("이거슨 개발자 모드임 - 성적 데이터 저장 완료");
                saveMoney(9999, false);
                LoadData();
            },
            (error) =>
            {
                LoadPopUpScreen(false);
                Debug.Log("이거슨 개발자 모드임 - 성적 데이터 저장 실패");
            }
            );

        upgradeValue.Clear();


        #endregion
    }

    void LoadData()
    {
        print("LoadData call");
        print(UserData.UserPlayFabId);

        #region "로드"
        var upgraderequest = new GetUserDataRequest() { PlayFabId = UserData.UserPlayFabId };
        PlayFabClientAPI.GetUserData(upgraderequest,
            (result) =>
            {
                foreach (var item in result.Data)
                {
                    if (item.Key == "Upgrade")
                    {
                        string[] upgradeValue = item.Value.Value.Split('|');

                        for (int i = 0; i < upgradeValue.Length; i++)
                        {
                            UpgradeManager.instance.towerSelect[i].setUpgrade(int.Parse(upgradeValue[i]));
                            UpgradeManager.instance.upgrade();
                        }
                    }
                    else if (item.Key == "Grade")
                    {
                        string[] gradeValue = item.Value.Value.Split('|');

                        GradeManager.instance.loadGrade(gradeValue);
                    }
                }

                GradeManager.instance.initStage();

                saveMoney(0, false);
            },
            (error) =>
            {
                LoadPopUpScreen(false);
                print("로드실패");
               Error("ERROR-A-1");
            }
            );
        #endregion
        
    }

    public void InitData()
    {
        LoadPopUpScreen(true);

        #region "업그레이드 수치 초기화"
        var upgraderequest = new UpdateUserDataRequest() { KeysToRemove = new List<string>() { "Upgrade" } };
        PlayFabClientAPI.UpdateUserData(upgraderequest, (result) =>
         {
             for (int i = 0; i < UpgradeManager.instance.towerSelect.Length; i++)
             {
                 UpgradeManager.instance.towerSelect[i].setUpgrade(0);
                 UpgradeManager.instance.upgrade();
             }
             Debug.Log("업그레이드 수치 초기화 완료");
         }, (error) => Debug.Log("업그레이드 수치 초기화 실패"));
        #endregion

        //#region "성적 수치 초기화"
        //var graderequest = new UpdateUserDataRequest() { KeysToRemove = new List<string>() { "Grade" } };
        //PlayFabClientAPI.UpdateUserData(graderequest, (result) =>
        //{
        //    GradeManager.instance.loadGrade(true);
        //    Debug.Log("성적 수치 초기화 완료");

        //    LoadPopUpScreen(false);

        //}, (error) =>
        //{
        //    Debug.Log("성적 수치 초기화 실패");
        //    LoadPopUpScreen(false);

        //}
        //);
        //#endregion

        //saveMoney(MoneyManager.instance.getMoneyText(), true);

        if (MoneyManager.instance.getMoneyText() > 50)
            saveMoney(MoneyManager.instance.getMoneyText() - 50, true);
        else
            saveMoney(50 - MoneyManager.instance.getMoneyText(), false);

        AchieveManager.instance.AchievementClear(17);
    }

    public void LoadPopUpScreen(bool setActive)
    {
        Debug.Log("LoadPopup >> " + setActive);
        if (!setActive) if (loadPopUpflag) return;

        if (popUpScreen)
        {
            //popUpScreen.SetActive(setActive);
            LoadingManager.instance.LoadScreenActive(setActive);
        }

    }

    public void saveSettingValue(float BGM, float Effect)
    {
        print(path);
        // 기존 파일 있으면 삭제후 새로 만듦
        if (File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, BGM + "\n" + Effect);
    }

    public void loadSettingValue()
    {
        // 파일이 없으면 로드 안함
        if (!File.Exists(path))
        {
            soundValue.mainSoundValue = 1;
            soundValue.inGameSoundValue = 1;
            return;
        } 

            string[] loadData = File.ReadAllLines(path);

        soundValue.mainSoundValue = float.Parse(loadData[0]);
        soundValue.inGameSoundValue = float.Parse(loadData[1]);
    }

   public void Error(string errorCode)
    {
        ErrorCode.errorCode = errorCode;
        ErrorCode.onErrorUI = true;
        SceneManager.LoadSceneAsync("Intro");
    }
}
