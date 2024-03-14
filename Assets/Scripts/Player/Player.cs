using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectOwner
{
	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	// Singleplayer Play Sound
	//public event EventHandler OnPickupSomething;

	public static event EventHandler OnAnyPlayerSpawned;
	// Play sound
	public static event EventHandler OnAnyPickupSomething;

	public class OnSelectedCounterChangedEventArgs : EventArgs {
		public BaseCounter selectedCounter;
	}

	public static Player LocalInstance { get; private set; }

	[SerializeField] private int moveSpeed;
	[SerializeField] private LayerMask counterLayerMask;
	[SerializeField] private LayerMask collisionLayerMask;
	[SerializeField] private Transform holdPoint;
	[SerializeField] private InputHandler inputHandler;
	[SerializeField] private Vector3[] playerSpawnPointList;

	private bool isWalking;
    public bool IsWalking { get => isWalking; }

    //private InputHandler inputHandler;

	private Vector3 moveDirection;
	private Vector3 prevMoveDirection;
	private BaseCounter selectedCounter;
	private KitchenObject kitchenObject;

	private const float interactDistance = 2f;
	private const float playerHeight = 2.0f;
	private const float playerRadius = 0.7f;

	private void Awake()
	{
		//if (Instance != null)
		//{
		//	Debug.LogError("More than one Player instance");
		//}
		//Instance = this;
	}

	public static void ResetStaticData() {
		OnAnyPlayerSpawned = null;
		OnAnyPickupSomething = null;
	}

	private void Start()
	{
		//inputHandler = GetComponent<InputHandler>();
		InputHandler.Instance.OnInteractAction += InputHandler_OnInteractAction;
		InputHandler.Instance.OnInteractAlternateAction += InputHandler_OnInteractAlternateAction;
	}

	public override void OnNetworkSpawn() {
		if (IsOwner) {
			LocalInstance = this;
		}
		transform.position = playerSpawnPointList[(int)OwnerClientId];
		OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);  // 保证绑定SelectCounterVisual事件成功
	}

	private void InputHandler_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if (!GameManager.Instance.IsGamePlaying()) { return; }
		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}

	private void InputHandler_OnInteractAction(object sender, EventArgs e)
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
		
		bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionLayerMask);

		if (!canMove) {
			// test if can move horizontally
			Vector3 moveDirX = new(moveDirection.x, 0, 0);
			canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionLayerMask);
			if (canMove) {
				moveDirection = moveDirX;
			} else {
				// test if can move vertically
				Vector3 moveDirZ = new(0, 0, moveDirection.z);
				canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionLayerMask);
				if (canMove) {
					moveDirection = moveDirZ;
				}
			}
		}

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
			//OnPickupSomething?.Invoke(this, EventArgs.Empty);
			OnAnyPickupSomething?.Invoke(this, EventArgs.Empty);  // Play sound event
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

    public NetworkObject GetNetworkObject()
    {
		return NetworkObject;
    }
}
