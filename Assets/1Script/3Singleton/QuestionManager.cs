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
        get
        {
            if (questionInfos == null || questionInfos.Count == 0 || _currentIndex < 0 || _currentIndex >= questionInfos.Count)
            {
                return new string[0];
            }
            return questionInfos[_currentIndex].Selection;
        }
    }

    public string[] CurrentMorsePattern
    {
        get
        {
            if (questionInfos == null || questionInfos.Count == 0 || _currentIndex < 0 || _currentIndex >= questionInfos.Count)
            {
                return new string[0];
            }
            return questionInfos[_currentIndex].MorsePattern;
        }
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
        get
        {
            if (questionInfos == null || questionInfos.Count == 0 || _currentIndex < 0 || _currentIndex >= questionInfos.Count)
            {
                return string.Empty;
            }
            return questionInfos[_currentIndex].Question;
        }
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
        CurrentIndex = 0;

        if (items == null || items.Count == 0)
        {
            questionInfos = new List<QuestionInfo>(0);
            Debug.LogWarning("로드된 질문 데이터가 비어 있습니다.");
            return;
        }

        // 카트리지 전환 시 전체 복사를 피해서 GC/CPU 부담을 줄인다.
        questionInfos = items;
        foreach (var item in questionInfos)
        {
            if (item == null)
            {
                Debug.LogWarning("질문 데이터 중에 null 항목이 있습니다.");
            }
            Debug.Log($"질문: {item.Question}, 선택지: {string.Join(", ", item.Selection)}, 모스 패턴: {string.Join(", ", item.MorsePattern)}");
        }

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
        if (cartridges == null || cartridges.Length == 0)
        {
            _cachedCartridges = null;
            Debug.LogWarning("캐싱할 카트리지 데이터가 없습니다.");
            return;
        }

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
        if (_cachedCartridges == null || _cachedCartridges.Length == 0) return;

        int index = Mathf.Clamp(value - 1, 0, _cachedCartridges.Length - 1);
        if (_cachedCartridges[index] == null)
        {
            Debug.LogWarning($"카트리지 {index + 1} 데이터가 비어 있습니다.");
            return;
        }

        _cartridge = index + 1;
        Initialize(_cachedCartridges[index]);
    }

    public void Initialize(QuestionInfo items)
    {
        morsePass = items;

    }

    QuestionInfo IMorsePassTarget.Data()
    {
        return morsePass;
    }
}
