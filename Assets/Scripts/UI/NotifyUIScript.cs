using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyUIScript : Singleton<NotifyUIScript>
{
    [SerializeField] Text titleText;
    [SerializeField] GameObject notifyPanel;
    public void StartNotify(string title)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            titleText.text = title;
            // Start the coroutine to close the panel
            notifyPanel.SetActive(true);
            StartCoroutine(CloseCheckMatePanelAfterDelay(0.5f));
        });
    }

    // Coroutine to close the checkmate panel after a delay
    private IEnumerator CloseCheckMatePanelAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Deactivate the checkmate panel
        notifyPanel.SetActive(false);
    }
}
