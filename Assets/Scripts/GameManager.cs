using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerV2 player;
    public GameObject pauseMenu;

    PlayerInputActions managerControls;
    InputAction pause;
    // Start is called before the first frame update
    void Awake()
    {
        managerControls = new PlayerInputActions();
        //initialize out perfomance debuffs
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;
    }

    private void OnEnable()
    {
        pause = managerControls.Player.PauseGame;
        pause.Enable();
        pause.performed += PauseGame; 
    }

    private void OnDisable()
    {
        pause.Disable();
        pause.performed -= PauseGame;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0)
        {
            player.enabled = false;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }

        else
        {
            player.enabled = true;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
