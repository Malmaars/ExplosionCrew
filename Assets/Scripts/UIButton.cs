using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    GameManager gameManager;
    public GameObject newMenu, oldMenu;
    PlayerV2 player;

    bool fpsCap = true;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerV2>();
    }
    public void UnPause()
    {
        gameManager.PauseGameAsCall();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenNewMenu(GameObject newMenu)
    {
        newMenu.SetActive(true);
        transform.gameObject.SetActive(false);
    }

    public void SetFPSCap(Toggle toggle)
    {
        if (!fpsCap)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            toggle.isOn = true;
            fpsCap = true;
        }

        else
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
            toggle.isOn = false;
            fpsCap = false;
        }
    }

    public void InvertCameraControls(Toggle toggle)
    {
        player.invertedCameraControls = !player.invertedCameraControls;
        toggle.isOn = !toggle.isOn;
    }
}
