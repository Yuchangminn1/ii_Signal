using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBallImage : MonoBehaviour
{
    public bool currentPlayer = true;

    RawImage _rawImage;

    public Texture[] ColorBallTextures;

    void Start()
    {
        _rawImage = GetComponentInChildren<RawImage>();
    }
    void OnEnable()
    {
        if (GameManager.Instance.IsStarted)
        {
            if (currentPlayer)
            {
                if (UserDataManager.Instance.CurrentDirection == Direction.Left)
                    _rawImage.texture = ColorBallTextures[(int)UserDataManager.Instance.GetPlayer(Direction.Left).ColorBallType];
                else
                    _rawImage.texture = ColorBallTextures[(int)UserDataManager.Instance.GetPlayer(Direction.Right).ColorBallType];
            }
            else
            {
                if (UserDataManager.Instance.CurrentDirection == Direction.Left)
                    _rawImage.texture = ColorBallTextures[(int)UserDataManager.Instance.GetPlayer(Direction.Right).ColorBallType];
                else
                    _rawImage.texture = ColorBallTextures[(int)UserDataManager.Instance.GetPlayer(Direction.Left).ColorBallType];
            }

        }
    }
}
