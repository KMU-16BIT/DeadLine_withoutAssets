using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public class PlayerUpgrade
{
    [HorizontalGroup("Player Status", 75)]
    // 해당 업그레이드 스프라이트
    [PreviewField(75)]
    [HideLabel]
    public Sprite playerImage;

    // 데미지
    [VerticalGroup("Player Status/Stats")]
    [LabelWidth(100)]
    public int damage;

    // 스킬 게이지
    [VerticalGroup("Player Status/Stats")]
    [LabelWidth(100)]
    public int skillGauge;
}

[System.Serializable]
public class PlayerSelect
{
    public PlayerUpgrade[] playerUpgrade = new PlayerUpgrade[5];

    // 현재 업그레이드 수준
    // 스테이지 진행 정도에 따라 변화
    int upgrade;

    public void initUpgrade() => upgrade = 0;
    public void setUpgrade(int value) => upgrade = value;
    public int getUpgrade() => upgrade;


}

[System.Serializable]
public class TowerUpgrade
{
    [HideInInspector] public string Name;


    [HorizontalGroup("Tower Status", 75)]
    // 해당 업그레이드 스프라이트
    [PreviewField(75)]
    [HideLabel]
    public Sprite towerImage;


    // 공격 속도
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public float assaultSpeed;

    // 데미지
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public int damage;

    // 총알 속도
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public float bulletSpeed;

    // 스킬 쿨타임
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public float skillCoolTime;

    // 스킬 지속시간
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public float skillDurationTime;

    // 스킬 수치
    // 샤프 볼펜: 스킬 공격 속도
    // 지우개: 스킬 공격 데미지
    [VerticalGroup("Tower Status/Stats")]
    [LabelWidth(100)]
    public float skillValue;
}

[System.Serializable]
public class TowerSelect
{
    [HideInInspector] public string Name;

    // 각 타워 업그레이드별 스프라이트
    //public Sprite[] towerImages = new Sprite[5];

    // 각 업그레이드의 스탯
    public TowerUpgrade[] towerUpgrade = new TowerUpgrade[5];

    // 현재 업그레이드 수준 0(default)~4(max)
    int upgrade;
    
    public void initUpgrade() { upgrade = 0; }

    public bool addUpgrade() {
        if (upgrade < UpgradeManager.__UPGRADE_MAX__)
        {
            upgrade += 1;
            return true;
        }
        else
        {
            Debug.Log("최대 업그레이드 입니다.");
            return false;
        }
    }

    public void setUpgrade(int value) // setUpgrade
    {
        if (value <= UpgradeManager.__UPGRADE_MAX__)
        {
            upgrade = value;
        }
        else
        {
            Debug.Log("최대 업그레이드 입니다.");
        }
    }
    public int getUpgrade() { return upgrade; }

}



public class UpgradeManager : MonoBehaviour
{
    /**
     * 
     * 타워 업그레이드를 제어 및 관리하는 스크립트
     * 
     * 업그레이드: 1(default) - 2 - 3 - 4 - 5(max)
     * 
     * */
    public static UpgradeManager instance;

    public static int __UPGRADE_MAX__ = 4;

    public PlayerSelect playerSelect = new PlayerSelect();

    [InfoBox("sharp, pen, knife, eraser 순서를 반드시 따를 것")]
    public TowerSelect[] towerSelect = new TowerSelect[4];


    public GameObject[] Btns;
    public AudioSource upgradeBtnSound;

    // 업그레이드에 필요한 비용
    public int[] upgradeCosts = new int[4];
    // 버튼 사운드



    // 초기화 버튼
    public GameObject resetUpgradeBtn;

    private void Awake()
    {
        instance = this;
        
        //DontDestroyOnLoad(gameObject);

        initUpgrade();
    }
    

    public void initUpgrade()
    {
        // 타워 init
        for(int i = 0; i < towerSelect.Length; i++)
        {
            towerSelect[i].initUpgrade();
        }

        // 플레이어 init
        playerSelect.initUpgrade();
    }


    // 업그레이드 진행
    // (int)towerType == Sharp, Pen, Knife, Eraser, OneOff
    public void upgrade(int towerType)
    {
      
        // 사운드 플레이
        upgradeBtnSound.Play();





        // 재화가 충분하다면
        if (towerSelect[towerType].getUpgrade() < 4 &&
            MoneyManager.instance.getMoneyText() - upgradeCosts[towerSelect[towerType].getUpgrade()] >= 0 )
        {
            print($"{towerSelect[towerType].getUpgrade()} << 현재 업그레이드");


            // 업그레이드가 가능하다면(Max가 아니라면)
            if (towerSelect[towerType].addUpgrade())
            {
                // 최초 강화 업적
                AchieveManager.instance.AchievementClear(15);

                // 최종 강화 업적
                if (towerSelect[towerType].getUpgrade() == 4) AchieveManager.instance.AchievementClear(16);

                // 업그레이드. 돈 사용
                DataManager.instance.saveMoney(upgradeCosts[towerSelect[towerType].getUpgrade() - 1], true);


                // 기존 버튼 내에서 호출하던 함수 위치 이동
                DataManager.instance.SaveData("upgrade");
            }
        }



        // set tower sprite
        Btns[towerType].transform.GetChild(0).GetComponent<Image>().sprite =
         //   towerSelect[towerType].towerImages[towerSelect[towerType].getUpgrade()];
         towerSelect[towerType].towerUpgrade[towerSelect[towerType].getUpgrade()].towerImage;

        // set upgrade value
        string tmp = "LV.";
        tmp += (towerSelect[towerType].getUpgrade() + 1) == 5 ? "MAX" : ""+(towerSelect[towerType].getUpgrade() + 1);
        Btns[towerType].transform.GetChild(1).GetComponent<Text>().text = tmp;

        // set upgrade cost
        int cost;
        if (towerSelect[towerType].getUpgrade() == 4)
            cost = -1;
        else
            cost = upgradeCosts[towerSelect[towerType].getUpgrade()];
        Btns[towerType].transform.GetChild(3).GetComponent<Text>().text = (cost == -1 ? "M" : ""+cost);

        DataManager.instance.LoadPopUpScreen(false);

        Debug.Log("Upgrade 진행 완료");
    }

    // 업그레이드 수준에 맞게 스프라이트 교체
    public void upgrade()
    {
        int towerType;

        for (int i = 0; i < towerSelect.Length; i++)
        {
            towerType = i;

            //print("upgrade(" + tower + ") >> " + towerSelect[towerType].getUpgrade());

            // set tower sprite
            Btns[towerType].transform.GetChild(0).GetComponent<Image>().sprite =
             //   towerSelect[towerType].towerImages[towerSelect[towerType].getUpgrade()];
             towerSelect[towerType].towerUpgrade[towerSelect[towerType].getUpgrade()].towerImage;

            // set upgrade value
            string tmp = "LV.";
            if (towerSelect[towerType].getUpgrade() == 4) tmp = "MAX";
            else tmp += (towerSelect[towerType].getUpgrade() + 1);
            Btns[towerType].transform.GetChild(1).GetComponent<Text>().text = tmp;

            // set upgrade cost
            int cost;
            if (towerSelect[towerType].getUpgrade() == 4)
                cost = -1;
            else
                cost = upgradeCosts[towerSelect[towerType].getUpgrade()];
            Btns[towerType].transform.GetChild(3).GetComponent<Text>().text = (cost == -1 ? "M" : "" + cost);
        }
       
    }

}
