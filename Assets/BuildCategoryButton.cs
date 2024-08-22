using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCategoryButton : MonoBehaviour
{
    public BuilderManager.BuildCategory category;
    public void OnClick()
    {
        BuilderManager.ins.ChangeBuildCategory(category);
    }
}
