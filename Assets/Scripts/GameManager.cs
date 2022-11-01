using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerV2 player;
    public GameObject pauseMenu;
    int currentHealth;

    PlayerInputActions managerControls;
    InputAction pause;

    public RawImage healthVisual;
    public Texture fullHealth, ThreeHealth, TwoHealth, OneHealth, NoHealth;
    // Start is called before the first frame update
    void Awake()
    {
        managerControls = new PlayerInputActions();
        //initialize out perfomance debuffs
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        if(player.playerHealth != currentHealth)
        {
            currentHealth = player.playerHealth;

            switch (currentHealth)
            {
                case 4:
                    healthVisual.texture = fullHealth;
                    break;
                case 3:
                    healthVisual.texture = ThreeHealth;
                    break;
                case 2:
                    healthVisual.texture = TwoHealth;
                    break;
                case 1:
                    healthVisual.texture = OneHealth;
                    break;
                case 0:
                    healthVisual.texture = NoHealth;
                    break;
            }
        }
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
        if (player.playerHealth > 0)
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

    public void PauseGameAsCall()
    {
        if (Time.timeScale > 0)
        {
            player.enabled = false;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }

        else
        {
            pauseMenu.SetActive(false);
            player.enabled = true;
            Time.timeScale = 1;
        }
    }
}
