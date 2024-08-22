using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSwapButton : MonoBehaviour
{
    public ModeManager.Mode mode;
    public void OnClick()
    {
        ModeManager.ins.SwapGameMode(mode);
    }
}
