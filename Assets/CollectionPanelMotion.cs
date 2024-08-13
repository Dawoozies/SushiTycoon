using LitMotion;
using UnityEngine;
public class CollectionPanelMotion : MonoBehaviour
{
    [SerializeField] float timeDeltaMultiplier;
    [SerializeField] float panelOpenTime = 1f;
    [SerializeField] float panelCloseTime = 1f;
    [SerializeField] Ease openEasing;
    [SerializeField] Ease closeEasing;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform rectTransformStateClosed;
    [SerializeField] RectTransform rectTransformStateOpen;

    CompositeMotionHandle openHandles = new CompositeMotionHandle();
    CompositeMotionHandle closeHandles = new CompositeMotionHandle();
    public bool doMotion;
    [Disable] public bool open;
    [Disable] public bool openMotionActive;
    [Disable] public bool closeMotionActive;
    void DoOpenMotion()
    {
        var openHandlePos = LMotion.Create(rectTransformStateClosed.localPosition, rectTransformStateOpen.localPosition, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.localPosition = x)
            .AddTo(openHandles);
        var openHandleWidth = LMotion.Create(rectTransformStateClosed.rect.width, rectTransformStateOpen.rect.width, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
            .AddTo(openHandles);
        var openHandleHeight = LMotion.Create(rectTransformStateClosed.rect.height, rectTransformStateOpen.rect.height, panelOpenTime)
            .WithEase(openEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x))
            .AddTo(openHandles);

        open = true;
    }
    void DoCloseMotion()
    {
        var closeHandlePos = LMotion.Create(rectTransformStateOpen.localPosition, rectTransformStateClosed.localPosition, panelCloseTime)
            .WithEase(closeEasing)
            .Bind(x => rectTransform.localPosition = x)
            .AddTo(closeHandles);
        var closeHandleWidth = LMotion.Create(rectTransformStateOpen.rect.width, rectTransformStateClosed.rect.width, panelCloseTime)
            .WithEase(closeEasing)   
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
            .AddTo(closeHandles);
        var closeHandleHeight = LMotion.Create(rectTransformStateOpen.rect.height, rectTransformStateClosed.rect.height, panelCloseTime)
            .WithEase(closeEasing)
            .Bind(x => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x))
            .AddTo(closeHandles);

        open = false;
    }
    void Update()
    {
        openMotionActive = false;
        foreach (var handle in openHandles)
        {
            if(handle.IsActive())
            {
                openMotionActive = true;
                break;
            }
        }
        closeMotionActive = false;
        foreach (var handle in closeHandles)
        {
            if(handle.IsActive())
            {
                closeMotionActive = true;
                break;
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
}
