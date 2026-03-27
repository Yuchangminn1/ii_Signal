using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : Singleton<ResultManager>
{

    public ResultContainer PlayerResultContainer;
    public DebugResultContainer DebugPlayerResultContainer;


    public CanvasGroup ResultCanvasGroup;


    Direction currentDirection;


    void Start()
    {
        if (NetworkManager.Instance.IsServer)
            currentDirection = Direction.Left;
        else
            currentDirection = Direction.Right;

    }


    public void Reset()
    {
        PlayerResultContainer?.Reset();
        DebugPlayerResultContainer?.Reset();
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ResultCanvasGroup == null)
            {
                return;
            }
            if (ResultCanvasGroup.alpha > 0.8)
            {
                FadeManager.Instance.SetAlphaZero(ResultCanvasGroup);
            }
            else
            {
                FadeManager.Instance.SetAlphaOne(ResultCanvasGroup);
            }
        }
    }

    public void Select(int selectIndex)
    {

        PlayerResultContainer.Select(selectIndex);
    }





}
