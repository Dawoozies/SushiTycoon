using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class ModeManager : MonoBehaviour
{
    public static ModeManager ins;
    void Awake()
    {
        ins = this;
    }
    public enum Mode
    {
        None,
        BuildMode,
        PathingTools,
        RestaurantOpen,
    }
    StateMachine<Mode> modeStateMachine = new();
}
