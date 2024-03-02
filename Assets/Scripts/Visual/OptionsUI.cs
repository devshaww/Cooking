using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    public static OptionsUI Instance;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(() => {
            Hide();
            //GamePauseUI.Instance.Hide();
        });
    }

    private void Start()
    {
        Hide();
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
