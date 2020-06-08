using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public static TimeScaleManager instance;
    public AudioSource TimeScaleBtnSound;

    [SerializeField] GameObject speedNormal, speedUp;

    private void Awake()
    {
        instance = this;

        setSpeed(1);
        btnChanger(true);
    }

    public void setSpeed(float speed)
    {
        TimeScaleBtnSound.Play();
        Time.timeScale = speed;


        btnChanger();
    }

    public void setSpeedTutorialMask()
    {
        TimeScaleBtnSound.Play();
        Time.timeScale = 1.0f;
        
        speedNormal.SetActive(true);
        speedUp.SetActive(false);
    }

    

    public void PauseSpeed()
    {
        // 이미 Pause 였다면 되돌린다.
        if(Time.timeScale == 0)
        {
            if (speedNormal.activeSelf == true) Time.timeScale = 1f;
            else Time.timeScale = 2f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    public void btnChanger(bool reset = false)
    {
        if (reset)
        {
            speedNormal.SetActive(true);
            speedUp.SetActive(false);
        }
        
        else if (speedNormal.activeSelf)
        {
            speedNormal.SetActive(false);
            speedUp.SetActive(true);
        }
        else
        {
            speedNormal.SetActive(true);
            speedUp.SetActive(false);
        }
    }
}
