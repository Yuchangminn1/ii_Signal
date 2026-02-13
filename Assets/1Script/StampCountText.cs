using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StampCountText : MonoBehaviour
{
    // Start is called before the first frame update

    Text _text;

    void OnEnable()
    {
        if (GameManager.Instance.CurrentGameMode != GameMode.Playing)
            return;
        if (_text != null)
            _text.text = _text.text.Replace("Count", PlayerData.Instance.GetPlayer().StampCount.ToString()); ;

    }
    void Start()
    {
        _text = GetComponent<Text>();
    }

}
