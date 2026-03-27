using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResultContainer : MonoBehaviour
{
    ResultSelector[] resultSelectors;

    public Direction currentDirection;

    public NameText nameText;


    int debugIndex = 0;


    void Start()
    {
        debugIndex = 0;
        resultSelectors = GetComponentsInChildren<ResultSelector>();
        nameText = GetComponentInChildren<NameText>();
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
            //resultSelectors[debugIndex].SelectAnswer(selectIndex);
        }
        else
        {
            resultSelectors[QuestionManager.Instance.CurrentIndex].SelectAnswer(selectIndex);
            //resultSelectors[debugIndex].SelectAnswer(selectIndex);
        }
        debugIndex++;
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
