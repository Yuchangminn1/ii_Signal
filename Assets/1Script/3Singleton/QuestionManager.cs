using System.Collections.Generic;
using UnityEngine;


public class QuestionInfo
{
    string _question;
    public string Question
    {
        get { return _question; }
        set { _question = value; }
    }
    string[] _selection;
    public string[] Selection
    {
        get { return _selection; }
        set { _selection = value; }
    }


    string[] _morsePattern;
    public string[] MorsePattern
    {
        get { return _morsePattern; }
        set { _morsePattern = value; }
    }
}
public class QuestionManager : Singleton<QuestionManager>, IQuestionTarget, IMorsePassTarget
{
    List<QuestionInfo> questionInfos = new List<QuestionInfo>(16);
    QuestionInfo morsePass = new QuestionInfo();


    int _currentIndex = 0;

    public int CurrentIndex
    {
        get { return _currentIndex; }
        set { _currentIndex = value; }
    }

    public string[] CurrentSelection
    {
        get { return questionInfos[_currentIndex].Selection; }
    }

    public string[] CurrentMorsePattern
    {
        get { return questionInfos[_currentIndex].MorsePattern; }
    }

    public string CurrentQuestionText
    {
        get { return questionInfos[_currentIndex].Question; }
    }

    public void UpdateUserAnswer(int selection)
    {
        UserDataManager.Instance.RequestUserDataUpdate(CurrentIndex, selection, UserDataManager.Instance.CurrentDirection);
    }

    public void AnswerQuestion()
    {
        if (UserDataManager.Instance.CurrentDirection == Direction.Left)
        {

        }
    }

    public QuestionInfo CurrentMorsePass
    {
        get { return morsePass; }
    }




    public List<QuestionInfo> QuestionInfos
    {
        get { return questionInfos; }
    }

    public void Initialize(List<QuestionInfo> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log($"{i} : {items[i].Question}");
        }
        questionInfos = new List<QuestionInfo>(items.Count);
        questionInfos = items;

        Debug.Log("로드된 질문 수: " + items.Count);
    }

    public List<QuestionInfo> Data()
    {
        return questionInfos;
    }

    public void Initialize(QuestionInfo items)
    {
        morsePass = items;

        Debug.Log("로드된 질문: " + morsePass.Question);
        Debug.Log("로드된 선택지 수: " + morsePass.Selection.Length);
        Debug.Log("로드된 모스 패턴 수: " + morsePass.MorsePattern.Length);
    }

    QuestionInfo IMorsePassTarget.Data()
    {
        return morsePass;
    }
}
