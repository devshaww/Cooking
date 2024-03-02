using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectOwner
{
	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public event EventHandler OnPickupSomething;
	public class OnSelectedCounterChangedEventArgs : EventArgs {
		public BaseCounter selectedCounter;
	}

	//public static Player Instance { get; private set; }

	[SerializeField] private int moveSpeed;
	[SerializeField] private LayerMask counterLayerMask;
	[SerializeField] private Transform holdPoint;
	[SerializeField] private InputHandler inputHandler;

	private bool isWalking;
    public bool IsWalking { get => isWalking; }

    //private InputHandler inputHandler;

	private Vector3 moveDirection;
	private Vector3 prevMoveDirection;
	private BaseCounter selectedCounter;
	private KitchenObject kitchenObject;

	private const float interactDistance = 2f;
	private const float playerHeight = 2.0f;
	private const float capsuleRadius = 0.7f;

	private void Awake()
	{
		//if (Instance != null)
		//{
		//	Debug.LogError("More than one Player instance");
		//}
		//Instance = this;
	}

	private void Start()
	{
		//inputHandler = GetComponent<InputHandler>();
		InputHandler.Instance.OnInteractAction += InputHandler_OnInteractAction;
		InputHandler.Instance.OnInteractAlternateAction += InputHandler_OnInteractAlternateAction;
	}

	private void InputHandler_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if (!GameManager.Instance.IsGamePlaying()) { return; }
		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}

	private void InputHandler_OnInteractAction(object sender, System.EventArgs e)
	{
		if (!GameManager.Instance.IsGamePlaying()) { return; }
		if (selectedCounter != null)
		{
			selectedCounter.Interact(this);
		}
	}

	private void Update()
    {
		if (!IsOwner) {
			return;
		}
		CheckMovement();
		CheckInteraction();	
    }

    private void CheckMovement() {
		Vector2 inputVector = InputHandler.Instance.GetMovementVector();

		moveDirection = new(inputVector.x, 0f, inputVector.y);

		isWalking = moveDirection != Vector3.zero;
		float moveDistance = moveSpeed * Time.deltaTime;
		
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, capsuleRadius, moveDirection, moveDistance);

		if (canMove)
		{
			transform.position += moveDistance * moveDirection;
		}

		if (moveDirection != Vector3.zero)
		{
			prevMoveDirection = moveDirection;
			float rotationSpeed = 10f;
			transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
		}
	}

	private void CheckInteraction() {
		if (Physics.Raycast(transform.position, prevMoveDirection, out RaycastHit hit, interactDistance, counterLayerMask)) {
			if (hit.transform.TryGetComponent(out BaseCounter counter)) {
				SetSelectedCounter(counter);
				//if (clearCounter != selectedCounter) {
				//	SetSelectedCounter(clearCounter);
				//}
			}
		} else {
			SetSelectedCounter(null);
		}
	}

	private void SetSelectedCounter(BaseCounter counter) {
		selectedCounter = counter;
		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = counter });
	}

	public void SetKitchenObject(KitchenObject ko)
	{
		kitchenObject = ko;
		if (ko != null) {
			OnPickupSomething?.Invoke(this, EventArgs.Empty);
		}
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}

	public Transform GetSpawnPoint()
	{
		return holdPoint;
	}
}
