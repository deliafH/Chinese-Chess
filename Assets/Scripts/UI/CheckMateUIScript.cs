using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CheckMateUIScript : Singleton<CheckMateUIScript>
{
    [SerializeField]Image chessSr, baseSr;
    [SerializeField] GameObject checkMatePanel;
    [SerializeField]ChessSprite chessSprite;
    Animator animator;
    public void StartAnim(string color)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (color == "RED")
            {
                baseSr.sprite = chessSprite.redBase;
                chessSr.sprite = chessSprite.spriteRedList[4];
            }
            else
            {
                baseSr.sprite = chessSprite.blackBase;
                chessSr.sprite = chessSprite.spriteBlackList[4];
            }
            // Activate the panel and play the animation
            checkMatePanel.SetActive(true);

            // Start the coroutine to close the panel
            StartCoroutine(CloseCheckMatePanelAfterDelay(0.5f));
        });
    }

    // Coroutine to close the checkmate panel after a delay
    private IEnumerator CloseCheckMatePanelAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Deactivate the checkmate panel
        checkMatePanel.SetActive(false);
    }
}
