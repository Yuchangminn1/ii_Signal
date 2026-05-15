using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResultContainer : MonoBehaviour
{
    ResultSelector[] resultSelectors;

    public Direction currentDirection;

    public NameText nameText;


    void Start()
    {
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
        Debug.Log($"{name} 결과 UI  {QuestionManager.Instance.CurrentIndex}질문 답 {selectIndex + 1} 선택");
        if (currentDirection == Direction.Right)
        {
            resultSelectors[UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count].SelectAnswer(selectIndex);
        }
        else
        {
            resultSelectors[QuestionManager.Instance.CurrentIndex].SelectAnswer(selectIndex);
        }
    }

    public void Reset()
    {
        if (GameManager.Instance.IsStarted)
        {
            if (resultSelectors == null || resultSelectors.Length == 0)
                resultSelectors = GetComponentsInChildren<ResultSelector>();
            foreach (var selector in resultSelectors)
            {
                selector?.Reset();
            }
            StartCoroutine(DelayTOPlay());
        }

    }

    IEnumerator DelayTOPlay()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(1f);
        nameText.SetText();

    }

}
