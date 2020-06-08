using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Threading;

public static class soundValue
{
    public static float mainSoundValue;
    public static float inGameSoundValue;
}

public class SettingManager : MonoBehaviour
{
    public GameObject SettingPanel;
    public GameObject CreditPanel;

    public bool isIngameScene;

    public Text versionText;

    // 뒤로가기 누른 수
    int pressCount;

    [Header("Sound")]
    public AudioMixer audioMixer;   // 오디오 관리자
    public AudioSource MainBGM;
    public AudioSource settingBtnSound;
    public Slider BGMSlider;        // 브금 슬라이더 
    public Slider EffectSlider;     // 이펙트 슬라이더
    bool isChange;                  // 사운드값이 바뀐지 여부

    // Start is called before the first frame update
    void Start()
    {
        pressCount = 0;

        isChange = false;
        setBGMSound(true);

        SettingPanel.gameObject.SetActive(false);

        versionText.text = "version : v" + Application.version;
        MainBGM.Play();
    }

    private void Update()
    {
        // 모바일 환경에서 뒤로가기 입력이 이루어졌을 경우
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyUp(KeyCode.Escape)) 
        {
            if (isIngameScene)
            {
                if (SettingPanel.gameObject.activeSelf) PanelOff();
                else PanelOn();
            }
            else
            {
                if (SettingPanel.gameObject.activeSelf) PanelOff();
                else
                {
                    pressCount++;
                    AndroidSet.instance.ShowToast("'뒤로'버튼 한번 더 누르면 종료됩니다.");
                    if (!IsInvoking("DoubleClick"))
                        Invoke("DoubleClick", 2.0f);
                }
            }

            if (pressCount == 2)
            {
                CancelInvoke("DoubleClick");
                Application.Quit();
            }
        }
    }

    void DoubleClick() => pressCount = 0;

    // 메인 브금 사운드 조절
    public void setBGMSound(bool init = false)
    {
        if (SceneManager.GetActiveScene().name == "Ingame")
            return;

        if (!init)
        {
            audioMixer.SetFloat("MainBGM", Mathf.Log(BGMSlider.value) * 10);
            isChange = true;
        }
        else
        {
            BGMSlider.value = soundValue.mainSoundValue;
            EffectSlider.value = soundValue.inGameSoundValue;
            audioMixer.SetFloat("MainBGM", Mathf.Log(soundValue.mainSoundValue) * 10);
        }
    }

    // 인게임 브금 사운드 조절
    public void setInGameSound()
    {
        audioMixer.SetFloat("InGameBGM", Mathf.Log(EffectSlider.value) * 10);
        soundValue.inGameSoundValue = Mathf.Log(EffectSlider.value) * 10;
        isChange = true;
    }

    public void PanelOn()
    {
        settingBtnSound.Play();
        if (SettingPanel.gameObject.activeSelf)
        {
            if (isIngameScene)
            {
                TimeScaleManager.instance.PauseSpeed();
                SettingSound.SS.PauseAllSound(false);
            }

            SettingPanel.gameObject.SetActive(false);
        }
        else
        {
            if (isIngameScene)
            {
                TimeScaleManager.instance.PauseSpeed();
                SettingSound.SS.PauseAllSound(true);
            }

            SettingPanel.gameObject.SetActive(true);
        }
    }

    public void PanelOff()
    {
        settingBtnSound.Play();
        if (isIngameScene)
        {
            TimeScaleManager.instance.PauseSpeed();
            SettingSound.SS.PauseAllSound(false);
        }

        if (isChange)
        {
            DataManager.instance.saveSettingValue(BGMSlider.value, EffectSlider.value);
            isChange = false;
        }
        
        SettingPanel.gameObject.SetActive(false);
    }

    public void CreditOn()
    {
        settingBtnSound.Play();
        AchieveManager.instance.AchievementClear(18);
        CreditPanel.SetActive(true);
    }

    public void CreditOff()
    {
        settingBtnSound.Play();
        CreditPanel.SetActive(false);
    }

    // 아이디 복사
    public void copyID()
    {
        print("copy button");
        StartCoroutine("copyIDAndMessage");
    }

    IEnumerator copyIDAndMessage()
    {
        UniClipboard.SetText(UserData.UserPlayFabId);
        AndroidSet.instance.ShowToast("ID 복사 완료");

        yield return new WaitForSeconds(2f);

        AndroidSet.instance.CancelToast();
    }
}
