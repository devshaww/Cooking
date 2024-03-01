using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Instance { get; private set; }

	private PlayerInputAction playerInputAction;

	public event EventHandler OnInteractAction;
	public event EventHandler OnInteractAlternateAction;
	public event EventHandler OnPauseAction;

	private void Awake()
	{
		Instance = this;
		// won't get destroyed automatically even if InputHander has been destroyed when scene changes
		playerInputAction = new PlayerInputAction();
		playerInputAction.Player.Enable();
		playerInputAction.Player.Interact.performed += Interact_performed;
		playerInputAction.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputAction.Player.Pause.performed += Pause_performed;
	}

    private void OnDestroy()
    {
        playerInputAction.Player.Interact.performed -= Interact_performed;
		playerInputAction.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputAction.Player.Pause.performed -= Pause_performed;

		playerInputAction.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
		OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAction?.Invoke(this, EventArgs.Empty);
	}

	// Update is called once per frame
	public Vector2 GetMovementVector()
    {
		Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
		return inputVector.normalized;
    }
}
