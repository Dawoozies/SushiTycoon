using LitMotion;
using UnityEngine;
using System;
public class CanvasPanelMotion : MonoBehaviour
{
    [SerializeField] float timeDeltaMultiplier;
    [SerializeField] float panelOpenTime = 1f;
    [SerializeField] float panelCloseTime = 1f;
    [SerializeField] Ease openEasing;
    [SerializeField] Ease closeEasing;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform rectTransformStateClosed;
    [SerializeField] RectTransform rectTransformStateOpen;

    MotionHandle[] openHandles;
    bool openHandlesInitialized;
    MotionHandle[] closeHandles;
    bool closeHandlesInitialized;
    public bool doMotion;
    [Disable] public bool open;
    [Disable] public bool openMotionActive;
    [Disable] public bool closeMotionActive;
    void Start()
    {
    }
    public void DoOpenMotion()
    {
        openHandles = new MotionHandle[3];
        openHandlesInitialized = true;
        var openHandlePos = LMotion.Create(rectTransformStateClosed.localPosition, rectTransformStateOpen.localPosition, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.localPosition = x)
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var openHandleWidth = LMotion.Create(rectTransformStateClosed.rect.width, rectTransformStateOpen.rect.width, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var openHandleHeight = LMotion.Create(rectTransformStateClosed.rect.height, rectTransformStateOpen.rect.height, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);

        openHandles[0] = openHandlePos;
        openHandles[1] = openHandleWidth;
        openHandles[2] = openHandleHeight;

        open = true;
    }
    public void DoCloseMotion()
    {
        closeHandles = new MotionHandle[3];
        closeHandlesInitialized = true;
        var closeHandlePos = LMotion.Create(rectTransformStateOpen.localPosition, rectTransformStateClosed.localPosition, panelCloseTime)
            .WithEase(closeEasing)
            .Bind(x => rectTransform.localPosition = x)
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var closeHandleWidth = LMotion.Create(rectTransformStateOpen.rect.width, rectTransformStateClosed.rect.width, panelCloseTime)
            .WithEase(closeEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var closeHandleHeight = LMotion.Create(rectTransformStateOpen.rect.height, rectTransformStateClosed.rect.height, panelCloseTime)
            .WithEase(closeEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);

        closeHandles[0] = closeHandlePos;
        closeHandles[1] = closeHandleWidth;
        closeHandles[2] = closeHandleHeight;

        open = false;
    }
    public void DoCloseMotion(Action onCompleteCallback)
    {
        closeHandles = new MotionHandle[3];
        closeHandlesInitialized = true;
        var closeHandlePos = LMotion.Create(rectTransformStateOpen.localPosition, rectTransformStateClosed.localPosition, panelCloseTime)
            .WithEase(closeEasing)
            .WithOnComplete(onCompleteCallback)
            .Bind(x => rectTransform.localPosition = x)
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var closeHandleWidth = LMotion.Create(rectTransformStateOpen.rect.width, rectTransformStateClosed.rect.width, panelCloseTime)
            .WithEase(closeEasing)   
            .WithOnComplete(onCompleteCallback)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);
        var closeHandleHeight = LMotion.Create(rectTransformStateOpen.rect.height, rectTransformStateClosed.rect.height, panelCloseTime)
            .WithEase(closeEasing)
            .WithOnComplete(onCompleteCallback)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x))
            .AddTo(gameObject, LinkBehaviour.CancelOnDisable);

        closeHandles[0] = closeHandlePos;
        closeHandles[1] = closeHandleWidth;
        closeHandles[2] = closeHandleHeight;

        open = false;
    }
    void Update()
    {
        openMotionActive = false;
        if(openHandlesInitialized)
        {
            foreach (var handle in openHandles)
            {
                if (handle.IsActive())
                {
                    openMotionActive = true;
                    break;
                }
            }
        }
        closeMotionActive = false;
        if(closeHandlesInitialized)
        {
            foreach (var handle in closeHandles)
            {
                if (handle.IsActive())
                {
                    closeMotionActive = true;
                    break;
                }
            }
        }
        if (doMotion && !openMotionActive && !closeMotionActive)
        {
            if(!open)
            {
                DoOpenMotion();
            }
            else
            {
                DoCloseMotion();
            }
            doMotion = false;
        }
    }
    public void SetRectangleSize(Vector2 openSize, Vector2 closeSize)
    {
        rectTransformStateOpen.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, openSize.x);
        rectTransformStateOpen.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, openSize.y);

        rectTransformStateClosed.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, closeSize.x);
        rectTransformStateClosed.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, closeSize.y);
    }
}
