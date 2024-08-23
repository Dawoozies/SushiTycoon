using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCategoryButton : MonoBehaviour
{
    public BuilderManager.BuildCategory category;
    BuildSelectionPanel buildSelectionPanel;
    void Start()
    {
        buildSelectionPanel = FindAnyObjectByType<BuildSelectionPanel>();
    }
    public void OnClick()
    {
        BuilderManager.ins.ChangeBuildCategory(category);
        buildSelectionPanel.OnBuildCategoryChanged();
    }
}
