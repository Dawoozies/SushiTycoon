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
    StateMachine<Mode> modeMachine = new StateMachine<Mode>();
    public GameObject gameModeButtons;
    public GameObject buildCanvas;
    public CustomerSpawner customerSpawner;
    void Start()
    {
        buildCanvas.SetActive(false);
        customerSpawner.allowSpawning = false;

        modeMachine.AddState(Mode.BuildMode, 
            onEnter: state => 
            {
                gameModeButtons.SetActive(true);
                buildCanvas.SetActive(true);
            },
            onExit: state => 
            {
                buildCanvas.SetActive(false);
            }
        );
        modeMachine.AddState(Mode.PathingTools, 
            onEnter: state => 
            { 

            },
            onExit: state =>
            {

            }
        );
        modeMachine.AddState(Mode.RestaurantOpen, 
            onEnter: state => 
            {
                gameModeButtons.SetActive(false);
                customerSpawner.OnOpenModeEnter();
            },
            onExit: state =>
            {
                customerSpawner.allowSpawning = false;
            }
            );

        modeMachine.Init();
    }
    public void SwapGameMode(Mode modeToSwapTo)
    {
        modeMachine.RequestStateChange(modeToSwapTo);
    }
}
