using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiveSceneLoad : MonoBehaviour
{
    public bool loadInDivingScene;
    [Disable] public bool divingSceneLoaded;
    private void Start()
    {
        if(loadInDivingScene)
        {
            Destroy(Camera.main.gameObject);
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            divingSceneLoaded = true;
        }
    }
}
