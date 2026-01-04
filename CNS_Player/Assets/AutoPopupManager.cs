using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AutoPopupManager : MonoBehaviour
{
    public static AutoPopupManager Instance;

    [Header("UI References")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;        
    public float defaultDuration = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            popupPanel.SetActive(false);
        }
    }
    public void ShowPopup(string message, float duration = -1f)
    {
        
        if (duration <= 0f) duration = defaultDuration;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowPopupCoroutine(message, duration));
    }

    private IEnumerator ShowPopupCoroutine(string message, float duration)
    {
        popupText.text = message;
        popupPanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        popupPanel.SetActive(false);
        currentRoutine = null;
    }
}
