using UnityEngine;
using UnityEngine.UI;

public class ResultSelector : MonoBehaviour
{
    public Text QuestionText;

    public GameObject MorseData;

    public GameObject TextBox;

    public GameObject DotImage;



    RawImage[] MorseDataImage;
    RawImage[] TextDataImage;

    Text[] TextDataText;


    RawImage[] DotImages;



    protected virtual void Awake()
    {
        //MorseDataImage = GetComponentsInChildren<RawImage>();
    }

    protected virtual void Start()
    {
        // Debug.Log("1 / " + TextBox.GetComponentsInChildren<RawImage>().Length);
        // Debug.Log("2 / " + MorseData.GetComponentsInChildren<RawImage>().Length);
        if (TextBox != null)
        {
            TextDataText = TextBox.GetComponentsInChildren<Text>();
            TextDataImage = TextBox.GetComponentsInChildren<RawImage>();
        }
        if (MorseData != null)
            MorseDataImage = MorseData.GetComponentsInChildren<RawImage>();
        if (DotImage != null)
            DotImages = DotImage.GetComponentsInChildren<RawImage>();
    }

    public void SetSelectionText(string[] selections)
    {
        if (TextBox != null)
        {
            Text[] textComponents = TextBox.GetComponentsInChildren<Text>();
            for (int i = 0; i < selections.Length && i < textComponents.Length; i++)
            {
                textComponents[i].text = selections[i].Replace("\r", " ").Replace("\n", " ");
            }
        }

    }

    public void SetQuestionText(string question)
    {
        // if (QuestionText != null)
        //     QuestionText.text = question.Replace("\r", " ").Replace("\n", " ");
    }


    public virtual void Reset()
    {
        if (MorseDataImage != null && MorseDataImage.Length != 0)
            FadeManager.Instance.SetAlphaZero(MorseDataImage);

        if (TextDataImage != null && TextDataImage.Length != 0)
            FadeManager.Instance.SetAlphaZero(TextDataImage);
        if (TextDataText != null && TextDataText.Length != 0)
            FadeManager.Instance.SetAlphaZero(TextDataText);
        if (QuestionText != null)
            FadeManager.Instance.SetAlphaZero(QuestionText);

        if (DotImages != null && DotImages.Length != 0)
            FadeManager.Instance.SetAlphaZero(DotImages);

    }

    public virtual void SelectAnswer(int selectIndex)
    {
        if (MorseDataImage != null && MorseDataImage.Length != 0)
            FadeManager.Instance.SetAlphaOne(MorseDataImage[selectIndex]);
        if (TextDataImage != null && TextDataImage.Length != 0)
            FadeManager.Instance.SetAlphaOne(TextDataImage[selectIndex]);
        if (QuestionText != null)
            FadeManager.Instance.SetAlphaOne(QuestionText);

        if (TextDataText != null && TextDataText.Length != 0)
            FadeManager.Instance.SetAlphaOne(TextDataText[selectIndex]);

        if (DotImages != null && DotImages.Length != 0)
            FadeManager.Instance.SetAlphaOne(DotImages[selectIndex]);

    }
}
