using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Transform MenuPointer;
    Button[] MenuButtons;
    int currentlySelectedButton;
    PlayerInputActions menuControls;
    InputAction move, dPadMove, select;

    bool movedThisInput;
    // Start is called before the first frame update
    void Awake()
    {
        menuControls = new PlayerInputActions();
        MenuButtons = GetComponentsInChildren<Button>();
        MenuPointer.position = new Vector3(MenuPointer.position.x, MenuButtons[currentlySelectedButton].transform.position.y, MenuPointer.position.z);
    }

    private void OnEnable()
    {
        move = menuControls.Player.Move;
        move.Enable();

        dPadMove = menuControls.Player.DPad;
        dPadMove.Enable();

        select = menuControls.Player.SelectMenu;
        select.Enable();
        select.performed += SelectMenuButton;
    }

    private void OnDisable()
    {
        move.Disable();
        dPadMove.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = move.ReadValue<Vector2>();
        Vector2 dPadDirection = dPadMove.ReadValue<Vector2>();

        if(!movedThisInput && (moveDirection.y > 0 || dPadDirection.y > 0))
        {
            ChangeMenuButton(-1);
            movedThisInput = true;
        }

        if (!movedThisInput && (moveDirection.y < 0 || dPadDirection.y < 0))
        {
            ChangeMenuButton(1);
            movedThisInput = true;
        }

        if (moveDirection.y == 0 && dPadDirection.y == 0)
            movedThisInput = false;
    }

    void ChangeMenuButton(int offset)
    {
        currentlySelectedButton += offset;

        if (currentlySelectedButton == MenuButtons.Length)
        {
            currentlySelectedButton = 0;
        }

        else if (currentlySelectedButton < 0)
        {
            currentlySelectedButton = MenuButtons.Length - 1;
        }

        MenuPointer.position = new Vector3(MenuPointer.position.x, MenuButtons[currentlySelectedButton].transform.position.y, MenuPointer.position.z);
    }

    void SelectMenuButton(InputAction.CallbackContext context)
    {
        MenuButtons[currentlySelectedButton].onClick.Invoke();
    }
}
