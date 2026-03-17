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
    List<QuestionInfo>[] _cachedCartridges;
    QuestionInfo morsePass = new QuestionInfo();
    int _cartridge = 1;


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


    public string[] GetMorsePattern(int index)
    {
        if (index < 0 || index >= questionInfos.Count)
        {
            Debug.LogWarning($"GetMorsePattern: Index {index} is out of range.");
            return new string[0];
        }
        return questionInfos[index].MorsePattern;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UpdateCartridge();
        }
    }


    public List<QuestionInfo> QuestionInfos
    {
        get { return questionInfos; }
    }

    public void Initialize(List<QuestionInfo> items)
    {
        questionInfos.Clear();
        CurrentIndex = 0;

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("로드된 질문 데이터가 비어 있습니다.");
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log($"{i} : {items[i].Question}");
        }
        questionInfos.AddRange(items);

        Debug.Log($"질문 데이터 교체 적용 완료: {questionInfos.Count}");
    }

    public List<QuestionInfo> Data()
    {
        return questionInfos;
    }

    public int Cartridge
    {
        get { return _cartridge; }
    }

    public void InitializeCartridges(List<QuestionInfo>[] cartridges)
    {
        _cachedCartridges = cartridges;
        Debug.Log($"총 {_cachedCartridges.Length}개 카트리지 캐싱 완료");
    }

    public void UpdateCartridge()
    {
        if (_cachedCartridges == null || _cachedCartridges.Length == 0)
        {
            Debug.LogWarning("카트리지 데이터가 없습니다.");
            return;
        }
        _cartridge++;
        if (_cartridge > _cachedCartridges.Length)
        {
            Debug.Log("모든 카트리지를 완료했습니다.");
            _cartridge = 0;
        }
        SetCartridge(_cartridge);
    }

    public void SetCartridge(int value)
    {
        if (_cachedCartridges == null) return;

        int index = Mathf.Clamp(value - 1, 0, _cachedCartridges.Length - 1);
        _cartridge = index + 1;
        Initialize(_cachedCartridges[index]);
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
