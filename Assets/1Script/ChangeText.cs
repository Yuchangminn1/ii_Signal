using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{

    Text text;
    string[] _changeTexts = { "Name님의 마음 신호가 모두 기록되었습니다.", "STEP.2\n오늘 기분을 부호로 설정하기" };






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
