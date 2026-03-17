using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextGuider : MonoBehaviour
{

    public RawImage lastImage;

    Text text;

    string lastText = "모든 체험이 완료되었습니다.\n결과 출력 공간으로 이동해 주세요.";
    string nextText = "체험이 완료되었습니다.\n카드에 표시된 블록으로 이동해 주세요.";

    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void CheckIsLast()
    {
        if (UserDataManager.Instance.GetPlayer().IsAllContentPlayed)
        {
            FadeManager.Instance.SetAlphaOne(lastImage);
            text.text = lastText;
        }
        else
        {
            FadeManager.Instance.SetAlphaZero(lastImage);
            text.text = nextText;
        }
    }
}
