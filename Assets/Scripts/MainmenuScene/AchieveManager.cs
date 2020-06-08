using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    public static AchieveManager instance;
    private void Awake() => instance = this;

    // UI 사운드
    public AudioSource BtnSound;
    public void openUI()
    {
        BtnSound.Play();
        Social.ShowAchievementsUI();
    }

    public void AchievementClear(int number)
    {
        if(number == 2) Social.ReportProgress(GPGSIds.achievement_2, 100, (bool success) => { });
        else if (number == 3) Social.ReportProgress(GPGSIds.achievement_3, 100, (bool success) => { });
        else if (number == 4) Social.ReportProgress(GPGSIds.achievement_4, 100, (bool success) => { });
        else if (number == 5) Social.ReportProgress(GPGSIds.achievement_5, 100, (bool success) => { });
        else if (number == 6) Social.ReportProgress(GPGSIds.achievement_6, 100, (bool success) => { });
        else if (number == 7) Social.ReportProgress(GPGSIds.achievement_7, 100, (bool success) => { });
        else if (number == 8) Social.ReportProgress(GPGSIds.achievement_8, 100, (bool success) => { });
        else if (number == 9) Social.ReportProgress(GPGSIds.achievement_9, 100, (bool success) => { });
        else if (number == 10) Social.ReportProgress(GPGSIds.achievement_10, 100, (bool success) => { });
        else if (number == 11) Social.ReportProgress(GPGSIds.achievement_11, 100, (bool success) => { });
        else if (number == 12) Social.ReportProgress(GPGSIds.achievement_12, 100, (bool success) => { });
        else if (number == 13) Social.ReportProgress(GPGSIds.achievement_13, 100, (bool success) => { });
        else if (number == 14) Social.ReportProgress(GPGSIds.achievement_14, 100, (bool success) => { });
        else if (number == 15) Social.ReportProgress(GPGSIds.achievement_15, 100, (bool success) => { });
        else if (number == 16) Social.ReportProgress(GPGSIds.achievement_16, 100, (bool success) => { });
        else if (number == 17) Social.ReportProgress(GPGSIds.achievement_17, 100, (bool success) => { });
        else if (number == 18) Social.ReportProgress(GPGSIds.achievement_18, 100, (bool success) => { });
    }
}
