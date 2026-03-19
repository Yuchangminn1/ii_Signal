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
    }

    public void Select(int selectIndex)
    {
        if (QuestionManager.Instance.CurrentIndex == 0)
            return;
        Debug.Log($"{name} 결과 UI  {QuestionManager.Instance.CurrentIndex}질문 답 {selectIndex + 1} 선택");
        if (currentDirection == Direction.Right)
        {
            resultSelectors[UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count - 1].SelectAnswer(selectIndex);
        }
        else
        {
            resultSelectors[QuestionManager.Instance.CurrentIndex - 1].SelectAnswer(selectIndex);
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
