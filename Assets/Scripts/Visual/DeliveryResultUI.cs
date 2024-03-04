using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP = "Popup";

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image background;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSucceed += DeliveryManager_OnRecipeSucceed;
        DeliveryManager.Instance.OnRecipeFail += DeliveryManager_OnRecipeFail;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFail(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
        iconImage.sprite = failedSprite;     
        background.color = failedColor;
        messageText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManager_OnRecipeSucceed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
        iconImage.sprite = successSprite;
        background.color = successColor;
        messageText.text = "DELIVERY\nSUCCESS";
    }
}
