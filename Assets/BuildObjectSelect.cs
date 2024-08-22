using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using uPools;

public class BuildObjectSelect : MonoBehaviour
{
    GameObject buildObjectPrefab;
    public TMP_Text itemName;
    public Image itemImage;
    public TMP_Text itemCost;
    public void OnClick()
    {
        BuilderManager.ins.ChangePrefabToBuild(buildObjectPrefab);
    }
    public void Return()
    {
        SharedGameObjectPool.Return(gameObject);
    }
    public void SetUpPrefab(GameObject prefab)
    {
        buildObjectPrefab = prefab;
        IBuiltObject builtObject = prefab.GetComponent<IBuiltObject>();
        itemName.text = builtObject.itemName;
        itemImage.sprite = builtObject.sprite;
        itemCost.text = string.Format("{0:C2}", builtObject.itemCost);
    }
}
