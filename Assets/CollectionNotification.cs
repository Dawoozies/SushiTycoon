using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using uPools;

public class CollectionNotification : MonoBehaviour
{
    [SerializeField] float displayTime;
    float _displayTime;
    [SerializeField] Image collectableImage;
    [SerializeField] Image collectableDropShadowImage;
    [SerializeField] TMP_Text collectableName;
    [SerializeField] TMP_Text collectableRank;

    CanvasPanelMotion canvasPanelMotion;
    bool notificationOpen => canvasPanelMotion.open;
    bool returnToPool => !canvasPanelMotion.open && !canvasPanelMotion.closeMotionActive;
    void Start()
    {
        canvasPanelMotion = GetComponent<CanvasPanelMotion>();
    }
    public void TriggerNotification(CollectableData collected)
    {
        collectableImage.sprite = collected.sprite;
        collectableDropShadowImage.sprite = collected.sprite;

        collectableName.text = collected.name;
        collectableRank.text = $"RANK {collected.rank}";

        _displayTime = displayTime;

        if (canvasPanelMotion == null)
            canvasPanelMotion = GetComponent<CanvasPanelMotion>();
        canvasPanelMotion.DoOpenMotion();
    }
    void Update()
    {
        if(notificationOpen)
        {
            if (_displayTime > 0)
            {
                _displayTime -= Time.deltaTime;
            }
            else
            {
                canvasPanelMotion.DoCloseMotion(ReturnToPool);
            }
        }
    }
    void ReturnToPool()
    {
        SharedGameObjectPool.Return(gameObject);
    }
}