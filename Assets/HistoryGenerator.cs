using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum VideoState { AUTO, NORMAL}
public class HistoryGenerator : MonoBehaviour
{
    [SerializeField] BoardGenerator board;
    const string startBoard = "[{\"type\": \"XE\", \"color\": \"RED\"}, {\"type\": \"MA\", \"color\": \"RED\"}, {\"type\": \"TINH\", \"color\": \"RED\"}, {\"type\": \"SI\", \"color\": \"RED\"}, {\"type\": \"TUONG\", \"color\": \"RED\"}, {\"type\": \"SI\", \"color\": \"RED\"}, {\"type\": \"TINH\", \"color\": \"RED\"}, {\"type\": \"MA\", \"color\": \"RED\"}, {\"type\": \"XE\", \"color\": \"RED\"}, null, null, null, null, null, null, null, null, null, null, {\"type\": \"PHAO\", \"color\": \"RED\"}, null, null, null, null, null, {\"type\": \"PHAO\", \"color\": \"RED\"}, null, {\"type\": \"TOT\", \"color\": \"RED\"}, null, {\"type\": \"TOT\", \"color\": \"RED\"}, null, {\"type\": \"TOT\", \"color\": \"RED\"}, null, {\"type\": \"TOT\", \"color\": \"RED\"}, null, {\"type\": \"TOT\", \"color\": \"RED\"}, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, {\"type\": \"TOT\", \"color\": \"BLACK\"}, null, {\"type\": \"TOT\", \"color\": \"BLACK\"}, null, {\"type\": \"TOT\", \"color\": \"BLACK\"}, null, {\"type\": \"TOT\", \"color\": \"BLACK\"}, null, {\"type\": \"TOT\", \"color\": \"BLACK\"}, null, {\"type\": \"PHAO\", \"color\": \"BLACK\"}, null, null, null, null, null, {\"type\": \"PHAO\", \"color\": \"BLACK\"}, null, null, null, null, null, null, null, null, null, null, {\"type\": \"XE\", \"color\": \"BLACK\"}, {\"type\": \"MA\", \"color\": \"BLACK\"}, {\"type\": \"TINH\", \"color\": \"BLACK\"}, {\"type\": \"SI\", \"color\": \"BLACK\"}, {\"type\": \"TUONG\", \"color\": \"BLACK\"}, {\"type\": \"SI\", \"color\": \"BLACK\"}, {\"type\": \"TINH\", \"color\": \"BLACK\"}, {\"type\": \"MA\", \"color\": \"BLACK\"}, {\"type\": \"XE\", \"color\": \"BLACK\"}]";
    int currentState;
    List<GameMove> moves;
    VideoState videoState;
    float time;
    [SerializeField] float duration;
    [SerializeField] Sprite autoState, normalState;
    [SerializeField] Image stateSprite;
    Stack<bool> hasEaten;
    [SerializeField] ProfileButtonDisplay urProfile, enemyProfile;
    void Start()
    {
        List<Piece> pieces = JsonConvert.DeserializeObject<List<Piece>>(startBoard);

        currentState = 0;
        moves = GameManager.Instance.gameHistoryDetail.gameMoves;
        board.Generate(pieces);
        videoState = VideoState.NORMAL;
        time = 0;
        hasEaten = new Stack<bool>();
        if (GameManager.Instance.user.userId == GameManager.Instance.gameHistoryDetail.player1.userId)
        {
            urProfile.Init(GameManager.Instance.gameHistoryDetail.player1);
            enemyProfile.Init(GameManager.Instance.gameHistoryDetail.player2);
        }
        else
        {
            urProfile.Init(GameManager.Instance.gameHistoryDetail.player2);
            enemyProfile.Init(GameManager.Instance.gameHistoryDetail.player1);

        }
    }
    private void Update()
    {
        if(videoState == VideoState.AUTO)
        {
            time += Time.deltaTime;
            if(time >= duration)
            {
                NextState();
                time -= duration;
            }
        }
    }
    public void NextState()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (currentState < moves.Count)
            {
                GameMove gameMove = moves[currentState];
                Vector2 startPos, endPos;
                if (GameManager.Instance.isRed())
                {
                    startPos = new Vector2(gameMove.fromX, gameMove.fromY);
                    endPos = new Vector2(gameMove.toX, gameMove.toY);
                }
                else
                {
                    startPos = new Vector2(9 - gameMove.fromX, gameMove.fromY);
                    endPos = new Vector2(9 - gameMove.toX, gameMove.toY);
                }
                hasEaten.Push(board.points[endPos].Chess != null);
                board.points[endPos].Movein(board.points[startPos].Chess);
                currentState += 1;
            }
        });
    }

    public void PrevState()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (currentState > 0)
            {
                currentState -= 1;
                GameMove gameMove = moves[currentState];
                Vector2 startPos, endPos;
                if (GameManager.Instance.isRed())
                {
                    startPos = new Vector2(gameMove.fromX, gameMove.fromY);
                    endPos = new Vector2(gameMove.toX, gameMove.toY);
                }
                else
                {
                    startPos = new Vector2(9 - gameMove.fromX, gameMove.fromY);
                    endPos = new Vector2(9 - gameMove.toX, gameMove.toY);
                }
                board.points[startPos].Movein(board.points[endPos].Chess);
                if(hasEaten.Peek())
                {
                    board.points[endPos].RespawnChess();
                }
                hasEaten.Pop();
            }
        });
    }

    public void ChangeState()
    {
        this.videoState = 1 - this.videoState;
        stateSprite.sprite = (videoState == VideoState.AUTO) ? autoState : normalState;
    }

    public void Leave()
    {
        SceneManager.LoadScene("PickRoom");
    }
}
