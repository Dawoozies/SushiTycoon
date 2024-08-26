using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using uPools;
using UnityEngine.UI;
public class AnimatedImage : CanvasObject
{
    public Image image;
    [SerializeField] RectTransform closedRect;
    [SerializeField] RectTransform openRect;
    public LMotionAnim mainAnimData;
    float _displayTime;
    Color imageColor,fadeColor;
    public void Initialize(Sprite sprite, float displayTime)
    {
        if(image == null)
            image = GetComponent<Image>();
        image.sprite = sprite;
        _displayTime = displayTime;
        imageColor = image.color;
        fadeColor = image.color;
        fadeColor.a = 0;
        DoAnim(mainAnimData);
    }
    private void Update()
    {
        if(_displayTime > 0)
        {
            _displayTime -= Time.deltaTime;
            image.color = Color.Lerp(fadeColor, imageColor, _displayTime);
        }
        else
        {
            SharedGameObjectPool.Return(gameObject);
        }
    }
    void DoAnim(LMotionAnim animData)
    {
        LMotion.Create(closedRect.localPosition, openRect.localPosition, animData.motionTime)
            .WithEase(animData.easing)
            .Bind(x => image.rectTransform.localPosition = x);
        LMotion.Create(closedRect.rect.width, openRect.rect.width, animData.motionTime)
            .WithEase(animData.easing)
            .Bind(x => image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x));
        LMotion.Create(closedRect.rect.height, openRect.rect.height, animData.motionTime)
            .WithEase(animData.easing)
            .Bind(x => image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x));
    }
    public override void OnReturn()
    {
        //dispose of all motions
    }
}
