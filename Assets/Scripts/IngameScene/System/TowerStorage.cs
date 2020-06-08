using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStorage : MonoBehaviour
{
    public static TowerStorage instance;

    public GameObject[] towers;

    private void Awake()
    {
        instance = this;
    }
}
