using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;

    private const string NUMBER_POPUP = "NumberPopup";

    private int previousPopupNumber;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        Hide();
    }

    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStart()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update()
    {
        int popupNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
        if (previousPopupNumber != popupNumber) {
            previousPopupNumber = popupNumber;
            animator.SetTrigger(NUMBER_POPUP); 
        }
        countdownText.text = popupNumber.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
