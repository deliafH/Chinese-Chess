using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChessPoint : MonoBehaviour
{

    public Vector2 coordinates;
    public Vector2 position;
    public bool isEmpty;
    ChessMen chess;
    public ChessMen Chess => chess;
    CircleCollider2D circleCollider2D;
    SpriteRenderer sr;
    [SerializeField] Color redColor, blueColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        blueColor = new Color(0, 122f/255, 255f/255, 100f/255);
        redColor = new Color(1, 0, 0, 100f / 255);
    }

    public void InitChessMen(ChessMen chess)
    {
        isEmpty = false;
        chess.transform.position = position;
        this.chess = chess;
    }
    private void OnMouseDown()
    {
        SocketIOManager.Instance.SendMoveIn(coordinates);
        //Movein();
    }

    public void SetCanMove(ChessMen chess)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            sr.enabled = true;
            circleCollider2D.enabled = true;

            if (!isEmpty) sr.color = redColor;
            else sr.color = blueColor;
        });
    }

    public void Movein(ChessMen chess)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (!isEmpty) this.chess.gameObject.SetActive(false);
            this.chess = chess;
            isEmpty = false;
            chess.MoveTo(this);
            BoardGenerator.Instance.SetEndMoving();
        });
    }

    public void SetEndMoving()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            sr.enabled = false;
            circleCollider2D.enabled = false;
        });
    }

    public void MoveOut()
    {
        isEmpty = true;
    }
}
