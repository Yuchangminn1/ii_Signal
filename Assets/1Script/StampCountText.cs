using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StampCountText : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isTotal = false;

    Text _text;

    void OnEnable()
    {


    }

    public void SetUpText()
    {
        if (GameManager.Instance.CurrentGameMode != GameMode.Playing)
            return;
        if (_text != null)
        {
            if (isTotal)
            {
                _text.text = _text.text.Replace("Count", (UserDataManager.Instance.GetPlayer().PieceCount + UserDataManager.Instance.GetPlayer().AddPiece).ToString());
                Debug.Log($"Total Stamp Count: {UserDataManager.Instance.GetPlayer().PieceCount + UserDataManager.Instance.GetPlayer().AddPiece}");

            }
            else
                _text.text = _text.text.Replace("Count", UserDataManager.Instance.GetPlayer().AddPiece.ToString());

        }

        //TODO 피스카운트 더하는 api 전송
    }

    void Start()
    {
        _text = GetComponent<Text>();
    }

}
