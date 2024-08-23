using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class BuildSelectionPanel : MonoBehaviour
{
    public GameObject buildSelectionPrefab;
    List<BuildObjectSelect> buildSelectionButtons = new();
    void Start()
    {
        BuilderManager.ins.RegisterOnChangeBuildCategoryCallback(OnChangeBuildCategory);
    }
    void OnChangeBuildCategory(GameObject[] buildObjectPrefabs)
    {
        //return all the buttons
        ReturnAllButtons();
        foreach(GameObject buildObject in buildObjectPrefabs)
        {
            GameObject poolObj = SharedGameObjectPool.Rent(buildSelectionPrefab);
            poolObj.transform.SetParent(transform, false);
            BuildObjectSelect buildObjectSelect = poolObj.GetComponent<BuildObjectSelect>();
            if(buildObjectSelect != null)
            {
                buildObjectSelect.SetUpPrefab(buildObject);
                buildObjectSelect.RegisterToOnClick(OnSelectButtonClickedHandler);
                buildSelectionButtons.Add(buildObjectSelect);
            }
        }
    }
    void ReturnAllButtons()
    {
        foreach(var button in buildSelectionButtons)
        {
            button.Return();
        }
        buildSelectionButtons.Clear();
    }
    void OnSelectButtonClickedHandler(BuildObjectSelect buttonClicked)
    {
        foreach (BuildObjectSelect buildSelectionButton in buildSelectionButtons)
        {
            buildSelectionButton.ToggleMask(buildSelectionButton != buttonClicked);
        }
    }
    public void OnBuildCategoryChanged()
    {
        foreach (BuildObjectSelect buildSelectionButton in buildSelectionButtons)
        {
            buildSelectionButton.ToggleMask(false);
        }
    }
}
