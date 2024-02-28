using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    //// Interfaces can not be exposed to unity editor
    //[SerializeField] IProgressible progressible;
    [SerializeField] GameObject progressibleGameObject;

    private IProgressible progressible;

    // Start is called before the first frame update
    void Start()
    {
        progressible = progressibleGameObject.GetComponent<IProgressible>();
        if (progressible == null) {
            Debug.LogError("Not a progressible game object(counter)");
        }
		progressible.OnProgressUpdate += Counter_OnProgressUpdate;
        barImage.fillAmount = 0f;
        gameObject.SetActive(false);
    }

	private void Counter_OnProgressUpdate(object sender, IProgressible.OnProgressChangeEventArgs e)
	{
		barImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized == 0f || e.progressNormalized == 1f) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
	}

}
