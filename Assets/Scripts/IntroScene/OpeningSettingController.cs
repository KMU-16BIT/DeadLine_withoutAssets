using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSettingController : MonoBehaviour
{
    public GameObject SettingPanel;

    // Start is called before the first frame update
    void Start()
    {
        SettingPanel.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {

    }

    public void PanelOn()
    {
        SettingPanel.gameObject.SetActive(true);
    }

    public void PanelOff()
    {
        SettingPanel.gameObject.SetActive(false);
    }
}
