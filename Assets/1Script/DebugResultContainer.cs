using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugResultContainer : MonoBehaviour
{
    public ResultSelector[] resultSelectors;

    public Direction currentDirection;

    public NameText nameText;



    int debugIndex = 0;


    void Start()
    {
        debugIndex = 0;
        resultSelectors = GetComponentsInChildren<ResultSelector>();
        nameText = GetComponentInChildren<NameText>();

        QuestionManager.Instance.AddOnQuestionChanged(SetTexts);
    }

    public void SetTexts(List<QuestionInfo> questionInfo)
    {
        if (resultSelectors == null || resultSelectors.Length == 0)
            resultSelectors = GetComponentsInChildren<ResultSelector>();

        for (int i = 0; i < resultSelectors.Length; i++)
        {
            if (i < questionInfo.Count)
            {
                resultSelectors[i].SetSelectionText(questionInfo[i].Selection);

                if (NetworkManager.Instance.IsServer)
                    resultSelectors[i].SetQuestionText(questionInfo[i].QuestionL);
                else
                    resultSelectors[i].SetQuestionText(questionInfo[i].QuestionR);
            }
            else
            {
                resultSelectors[i].SetSelectionText(new string[] { });
            }
        }


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Select(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Select(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Select(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Select(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Select(4);
        }
    }


    public void Select(int selectIndex)
    {
        if (UserDataManager.Instance.IsUser())
        {
            nameText.SetText();
        }
        else
        {
            Debug.LogWarning("유저 데이터가 없습니다. 결과 UI 업데이트를 건너뜁니다.");
        }
        Debug.Log($"{name} 결과 UI  {debugIndex}질문 답 {selectIndex + 1} 선택");
        if (currentDirection == Direction.Right)
        {
            resultSelectors[debugIndex].SelectAnswer(selectIndex);
        }
        else
        {
            resultSelectors[debugIndex].SelectAnswer(selectIndex);
        }
        debugIndex++;
    }

    public void Reset()
    {
        // if (GameManager.Instance.IsStarted)
        // {
        if (resultSelectors == null || resultSelectors.Length == 0)
            resultSelectors = GetComponentsInChildren<ResultSelector>();
        foreach (var selector in resultSelectors)
        {
            selector?.Reset();
        }
        StartCoroutine(DelayTOPlay());
        // }

    }

    IEnumerator DelayTOPlay()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(1f);
        nameText.SetText();

    }
}
