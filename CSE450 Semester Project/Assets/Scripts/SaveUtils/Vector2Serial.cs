using UnityEngine;

[System.Serializable]
public class Vector2Serial
{
    public float x;
    public float y;

    public Vector2Serial(Vector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }

    public Vector2 SerialToVector2()
    {
        return new Vector2(x, y);
    }
}