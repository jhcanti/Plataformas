using UnityEngine;

[System.Serializable]
public class GameData
{
    public float _pointX;
    public float _pointY;

    public GameData(float pointX, float pointY)
    {
        _pointX = pointX;
        _pointY = pointY;
    }
}
