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

    void Start()
    {
        // for (int i = 0; i < selectOptions.Length; i++)
        // {
        //     selectOptions[i].Initialize(currentQuestions[i]);
        // }
    }

    public void SetSelectedOption(string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            selectOptions[i].Initialize(options[i]);
        }
    }

    public string Select(int index)
    {
        if (index < 0 || index >= selectOptions.Length)
            return "";

        selectOptions[index].Select();
        return selectOptions[index]._morseValue;

    }

    public int GetOptionCount()
    {
        return selectOptions.Length;
    }

    public void Reset()
    {
        foreach (var option in selectOptions)
        {
            option.Reset();
        }
    }


}
