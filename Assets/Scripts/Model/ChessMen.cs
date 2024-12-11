using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//public enum ChessType { General, Advisor, Chariot, Cannon, Elephant, Horse, Solider}
// vua - si - xe - phao - tuong - ma - tot
public class ChessMen: MonoBehaviour
{
    public static ChessMen choosenChessMen;
    public string[] ChessType { get; } = { "XE", "MA", "TINH", "SI", "TUONG", "PHAO", "TOT" };
    public bool isUrChessmen;
    public Vector2 curCoordnates;
    //[SerializeField] ChessType chessType;
    [SerializeField] ChessSprite chessSprite;
    [SerializeField] SpriteRenderer sprite;
    public void Init(Vector2 startCoordinates, string type, string color)
    {
        curCoordnates = startCoordinates;
        choosenChessMen = null;
        BoardGenerator.Instance.points
            [startCoordinates].InitChessMen(this);

        for (int i = 0; i < ChessType.Length; i++) {
            if (type == ChessType[i])
            {
                if (color == "RED")
                    sprite.sprite = chessSprite.spriteRedList[i];
                else sprite.sprite = chessSprite.spriteBlackList[i];
            }
        }

        isUrChessmen = GameManager.Instance.isRed() ^ (color == "BLACK");
    }

    private void OnMouseDown()
    {
        if (choosenChessMen == this)
        {
            BoardGenerator.Instance.SetEndMoving();
            choosenChessMen = null;
        }
        else if (isUrChessmen)
        {
            ChooseToMove();
        }
    }

    public void MoveTo(ChessPoint chessPoint)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            BoardGenerator.Instance.points[curCoordnates].MoveOut();
            curCoordnates = chessPoint.coordinates;
            transform.position = chessPoint.position;
        });
    }

    protected void ChooseToMove()
    {
        choosenChessMen = this;
        if(GameManager.Instance.isRed())
        {
            SocketIOManager.Instance.SendChessMen(new Vector2(curCoordnates.x, curCoordnates.y));

        }else
        {

            SocketIOManager.Instance.SendChessMen(new Vector2(9 - curCoordnates.x, curCoordnates.y));
        }
        BoardGenerator.Instance.SetEndMoving();
    }

}
