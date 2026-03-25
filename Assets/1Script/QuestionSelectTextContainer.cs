using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuestionSelectTextContainer : MonoBehaviour
{
    public Texture DotTexture;


    public Texture DashTexture;



    SelectOption[] selectOptions;

    CanvasGroup _canvasGroup;

    void Awake()
    {
    }

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        selectOptions = GetComponentsInChildren<SelectOption>();

    }
    public CanvasGroup GetCanvasGroup()
    {
        return _canvasGroup;
    }

    public void UpdateData()
    {
        string[] selection = null;
        string[] patterns = null;
        if (PageController.Instance.CurrentPage == 4)
        {
            selection = QuestionManager.Instance.CurrentSelection;
            patterns = QuestionManager.Instance.CurrentMorsePattern;
        }

        else
        {
            return;
        }

        for (int i = 0; i < selection.Length; i++)
        {
            selectOptions[i].Initialize(selection[i]);
        }
        for (int i = 0; i < selectOptions.Length; i++)
        {
            selectOptions[i].SetPattern(patterns[i]);
        }
    }

    public void SetTextColor()
    {
        int index = MorseTranslator.CurrentDataIndex;


        if (index < 0 || index >= selectOptions.Length)
            return;
        selectOptions[index].Select();
    }

    public void SaveAnswer()
    {

        int index = MorseTranslator.CurrentDataIndex;

        if (index < 0 || index >= selectOptions.Length)
            return;

        var userDataManager = UserDataManager.Instance;
        var questionManager = QuestionManager.Instance;
        var pageController = PageController.Instance;

        if (userDataManager == null || questionManager == null || pageController == null)
        {
            Debug.LogError($"[{name}] SaveAnswer failed: manager instance is null.", this);
            return;
        }

        var player = userDataManager.GetPlayer();
        if (player == null)
        {
            Debug.LogError($"[{name}] SaveAnswer failed: player is null.", this);
            return;
        }

        try
        {
            StartCoroutine(userDataManager.RequestUserDataUpdate(questionManager.CurrentIndex + 1, index + 1, player.Direction));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{name}] SaveAnswer RequestUserDataUpdate failed: {ex}", this);
        }

        if (pageController.CurrentPage == 4)
        {
            try
            {
                ResultManager.Instance?.Select(index);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{name}] SaveAnswer ResultManager.Select failed: {ex}", this);
            }
        }



    }

    public void Reset()
    {
        foreach (var option in selectOptions)
        {
            option.Reset();
        }
        UpdateData();

    }

    void OnEnable()
    {
        if (GameManager.Instance.IsStarted)
            Reset();
    }


}
