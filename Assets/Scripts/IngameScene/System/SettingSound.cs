using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingSound : MonoBehaviour
{
    public static SettingSound SS;
    public AudioMixer audioMixer;

    public AudioSource BGMSound;
    public AudioSource[] TowerAttackSound;
    public AudioSource[] TowerSkillSound;
    public AudioSource[] PlayerSound;
    public AudioSource[] ObstaclesSound;

    public bool soundOff;
    private void Awake()
    {
        SS = this;
        soundOff = false;
    }

    void Start() => audioMixer.SetFloat("InGameBGM", soundValue.inGameSoundValue);


    // 타워 공격 사운드
    public void playTowerAttackSound(TowerType towerType)
    {
        if (soundOff) return;

        switch (towerType)
        { 
            case TowerType.Sharp:
                TowerAttackSound[0].Play();
                break;
            case TowerType.Pen:
                TowerAttackSound[1].Play();
                break;
            case TowerType.Knife:
                TowerAttackSound[2].Play();
                break;
            case TowerType.Eraser:
                TowerAttackSound[3].Play();
                break;
            default:
                break;
        }
    }

    // 타워 스킬 사운드
    public void playTowerSkillSound(TowerType towerType)
    {
        switch (towerType)
        {
            case TowerType.Sharp:
                TowerSkillSound[0].Play();
                break;
            case TowerType.Pen:
                TowerSkillSound[1].Play();
                break;
            case TowerType.Knife:
                TowerSkillSound[2].Play();
                break;
            case TowerType.Eraser:
                TowerSkillSound[3].Play();
                break;
            default:
                break;
        }
    }

    // 플레이어 공격, 스킬 사운드
    public void playPlayerSound(bool useSkill = false)
    {
        if (!useSkill) PlayerSound[0].Play();
        else PlayerSound[1].Play();
    }

    // 방해요소 먹었을때
    public void playObstaclesSound(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Soju:
                ObstaclesSound[0].Play();
                break;
            case EnemyType.Sleepy:
                if(!ObstaclesSound[1].isPlaying)
                ObstaclesSound[1].Play();
                break;
            case EnemyType.Phone:
                if (!ObstaclesSound[2].isPlaying)
                    ObstaclesSound[2].Play();
                break;
            default:
                break;
        }
    }

    // BGM 일시정지
    public void PauseAllSound(bool isPause)
    {
        //audioMixer.SetFloat("InGameBGM", soundValue.inGameSoundValue);
        if (isPause) BGMSound.Pause();
        else if (!isPause) BGMSound.UnPause();
    }

    // 모든 사운드 속도 조절
    public void SetPitchAllSound(float _pitch)
    {
        audioMixer.SetFloat("InGamePitch", _pitch);   
    }


}
