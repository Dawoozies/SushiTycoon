using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TotalCashDisplay : MonoBehaviour
{
    TMP_Text text;
    float _totalCash;
    [SerializeField] float countChangeTime;
    public LMotionAnim gainMoneyAnim;
    public LMotionAnim loseMoneyAnim;
    MotionHandle motionHandle;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        _totalCash = -1;
    }
    void Update()
    {
        if (!motionHandle.IsActive() && !Mathf.Approximately(_totalCash,RestaurantParameters.ins.TotalCash))
        {
            motionHandle = LMotion.Create(_totalCash, RestaurantParameters.ins.TotalCash, countChangeTime).BindToText(text, "{0:C2}");
            if(_totalCash > RestaurantParameters.ins.TotalCash)
            {
                DoTextAnim(loseMoneyAnim);
            }
            else
            {
                DoTextAnim(gainMoneyAnim);
            }

            _totalCash = RestaurantParameters.ins.TotalCash;
        }
    }
    void DoTextAnim(LMotionAnim textAnimData)
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
}
