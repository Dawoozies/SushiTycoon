using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using uPools;

public class AnimatedText : CanvasObject
{
    TMP_Text text;
    public LMotionTextAnim mainAnimData;
    public float displayTime;
    public void Initialize(string textInput)
    {
        if(text == null)
            text = GetComponent<TMP_Text>();

        text.text = textInput;
        displayTime = 0f;

        DoTextAnim(mainAnimData);
    }
    private void Update()
    {
        if (displayTime < RestaurantParameters.ins.CashDisplayTime)
        {
            displayTime += Time.deltaTime;
        }
        else
        {
            SharedGameObjectPool.Return(gameObject);
        }
    }
    void DoTextAnim(LMotionTextAnim textAnimData)
    {
        for (int i = 0; i < text.text.ToCharArray().Length; i++)
        {
            LMotion.Create(textAnimData.colorPair.A, textAnimData.colorPair.B, textAnimData.motionTime)
                .WithDelay(i * textAnimData.delay)
                .WithEase(textAnimData.easing)
                .BindToTMPCharColor(text, i);
            if (textAnimData.usePunch)
            {
                LMotion.Punch.Create(textAnimData.vectorPair.A, textAnimData.vectorPair.B, textAnimData.motionTime)
                    .WithDelay(i * textAnimData.delay)
                    .WithEase(textAnimData.easing)
                    .BindToTMPCharPosition(text, i);
            }
            else
            {
                LMotion.Create(textAnimData.vectorPair.A, textAnimData.vectorPair.B, textAnimData.motionTime)
                    .WithDelay(i * textAnimData.delay)
                    .WithEase(textAnimData.easing)
                    .BindToTMPCharPosition(text, i);
            }
        }
    }
    public override void OnReturn()
    {
        //dispose of all motions
    }
}