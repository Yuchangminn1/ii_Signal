using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : Singleton<ResultManager>
{

    public ResultContainer LeftPlayerResultContainer;
    public ResultContainer RightPlayerResultContainer;

    public CanvasGroup[] ResultCanvasGroups;


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
        LeftPlayerResultContainer.Reset();
        RightPlayerResultContainer.Reset();
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ResultCanvasGroups[0].alpha > 0.8)
            {
                for (int i = 0; i < ResultCanvasGroups.Length; i++)
                {
                    FadeManager.Instance.SetAlphaZero(ResultCanvasGroups[i]);
                }
            }
            else
            {
                for (int i = 0; i < ResultCanvasGroups.Length; i++)
                {
                    FadeManager.Instance.SetAlphaOne(ResultCanvasGroups[i]);
                }
            }
        }
    }

    public void LeftSelect(int selectIndex)
    {

        LeftPlayerResultContainer.Select(selectIndex);
    }

    public void RightSelect(int selectIndex)
    {
        RightPlayerResultContainer.Select(selectIndex);
    }



}
