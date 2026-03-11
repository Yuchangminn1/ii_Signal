using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{

    Text text;
    string[] _changeTexts = { "지금까지 저장한 Name님의 마음 신호를\n상대방에게 전송할 거예요!", "전송 전,\nName님의 오늘 기분으로 암호를 정해볼까요?", "STEP.2\n암호 설정하기" };





    void Start()
    {
        text = GetComponent<Text>();
    }

    public void SetText(int index)
    {
        if (index == 2)
        {
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.StepTextSound);
        }

        text.text = _changeTexts[index].Replace("Name", UserDataManager.Instance.GetPlayer().FirstName);


    }

    public void HideText()
    {
        FadeManager.Instance.SetAlphaZero(text);
    }


    void OnDisable()
    {
        text.text = "";
    }
}
