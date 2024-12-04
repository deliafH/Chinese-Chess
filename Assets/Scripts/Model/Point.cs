using System.Collections.Generic;

[System.Serializable]
public class Point
{
    public int x;
    public int y;
}

[System.Serializable]
public class ValidMoveData
{
    public List<Point> points;
}

[System.Serializable]
public class MoveData
{
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
}