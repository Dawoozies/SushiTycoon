using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class LMotionTextAnimation : MonoBehaviour
{
    public LMotionAnim[] textAnims;
    TMP_Text text;
    public void DoMotion(int index)
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        LMotionAnim textAnimData = textAnims[index];

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
}
