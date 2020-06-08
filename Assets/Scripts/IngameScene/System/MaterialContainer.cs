using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 게임 진행에 필요한 Material들을 보관하는 싱글턴 스크립트
 * 
 * */

    public enum MaterialType { EMPTY, OBSTACLE, DIE, }

[System.Serializable]
public class MaterialList
{
    #region lists
    public Material _emptyShader;
    public Material _enemyDieEff;
    public Material _obstacleEff;
    #endregion
    
    public Dictionary<MaterialType, Material> MaterialDic;

    public void MaterialGenerator()
    {
        MaterialDic = new Dictionary<MaterialType, Material>();

        MaterialDic.Add(MaterialType.EMPTY, _emptyShader);
        MaterialDic.Add(MaterialType.OBSTACLE, _obstacleEff);
        MaterialDic.Add(MaterialType.DIE, _enemyDieEff);
    }
}


public class MaterialContainer : MonoBehaviour
{
    public static MaterialContainer instance;

    public MaterialList materialList;
    
    private void Awake()
    {
        instance = this;

        materialList.MaterialGenerator();
    }

    public void MaterialChanger(GameObject obj, MaterialType type)
    {
        obj.GetComponent<SpriteRenderer>().material = materialList.MaterialDic[type];
    }

}
