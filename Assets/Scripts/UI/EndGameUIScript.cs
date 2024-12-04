using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUIScript : Singleton<EndGameUIScript>
{
    [SerializeField] Image title;
    [SerializeField] Sprite winSprite, loseSprite;
    [SerializeField] GameObject endGamePanel;

    public void Init(bool isWin)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            endGamePanel.SetActive(true);
            title.sprite = isWin ? winSprite : loseSprite;
        });
    }

    public void Replay()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
