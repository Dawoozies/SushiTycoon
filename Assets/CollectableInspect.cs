using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectableInspect : MonoBehaviour
{
    [SerializeField] Image collectableImage;
    [SerializeField] Image collectableDropShadowImage;
    [SerializeField] TMP_Text collectableName;
    [SerializeField] TMP_Text collectableRank;

    CanvasPanelMotion canvasPanelMotion;
    void Start()
    {
        canvasPanelMotion = GetComponent<CanvasPanelMotion>();
    }
    public void InspectionOpen(CollectableData collectableData)
    {
        collectableImage.sprite = collectableData.sprite;
        collectableDropShadowImage.sprite = collectableData.sprite;

        collectableName.text = collectableData.name;
        collectableRank.text = $"RANK {collectableData.rank}";

        if (canvasPanelMotion == null)
            canvasPanelMotion = GetComponent<CanvasPanelMotion>();
        canvasPanelMotion.DoOpenMotion();
    }
    public void InspectionClose()
    {
        canvasPanelMotion.DoCloseMotion();
    }
}
