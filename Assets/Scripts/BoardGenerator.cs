using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public static BoardGenerator Instance;

    [SerializeField] float maxX, maxY;
    [SerializeField] GameObject pointPrefab, pointParent, chessParent;
    [SerializeField] ChessMen chessPrefab;
    public Dictionary<Vector2, ChessPoint> points;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public void Generate(List<Piece> board)
    {
        points = new Dictionary<Vector2, ChessPoint>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Vector2 coordinates = new Vector2(i, j);
                GameObject go = Instantiate(pointPrefab, new Vector3(-maxX + (maxX / 4) * j, -maxY + (maxY / 4.5f) * i, 0), Quaternion.identity, pointParent.transform);

                ChessPoint point = go.AddComponent<ChessPoint>();
                point.isEmpty = true;
                point.coordinates = coordinates;
                point.position = go.transform.position;

                points[coordinates] = point;
                Piece piece = board[GameRoom.GetPieceAt(coordinates)];
                if (piece != null &&
                    piece.type != null &&
                    piece.type != "")
                {
                    ChessMen chess = Instantiate(chessPrefab, Vector3.zero, 
                        Quaternion.identity, chessParent.transform);
                    chess.Init(coordinates, piece.type, piece.color);
                }
            }
        }
    }

    public void SetEndMoving()
    {
        foreach (var v in points) v.Value.SetEndMoving();
    }
}