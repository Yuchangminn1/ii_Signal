using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuestionSelectTextContainer : MonoBehaviour
{
    public Texture NormalDotTexture;

    public Texture SelectedDotTexture;

    public Texture NormalDashTexture;

    public Texture SelectedDashTexture;

    SelectOption[] selectOptions;

    string[] currentQuestions =
    {
        "봄", "여름", "가을", "겨울","사계절"
    };

    public string[] CurrentQuestions
    {
        get { return currentQuestions; }
    }

    void Awake()
    {
        selectOptions = GetComponentsInChildren<SelectOption>();
    }


    public void SetSelectedOption(string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            selectOptions[i].Initialize(options[i]);
        }
    }

    public void SetTextColor()
    {
        int index = MorseTranslator.CurrentDataIndex;
        if (index < 0 || index >= selectOptions.Length)
            return;
        selectOptions[index].Select();
    }

    public void Reset()
    {
        foreach (var option in selectOptions)
        {
            option.Reset();
        }
    }


}
