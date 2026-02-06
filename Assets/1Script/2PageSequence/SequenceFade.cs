using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SequenceFade : SequenceScript
{
    [Tooltip("컷 효과를 적용할 Graphic 배열 (예: UI 이미지 등)")]

    [Header("In 나타남")]

    public List<Graphic> CutINGraphics;
    [Header("Out 사라짐")]

    public List<Graphic> CutOutGraphics;


    public List<CanvasGroup> CutInCanvasGroups;
    public List<CanvasGroup> CutOutCanvasGroups;
    public List<Graphic> FadeInGraphics;

    public List<Graphic> FadeOutGraphics;


    public List<CanvasGroup> FadeInCanvasGroups;


    public List<CanvasGroup> FadeOutCanvasGroups;


    [Header("0보다 클 경우 이 스크립트만 따로 시간 적용")] public float CustomFadeDuration = -1f;


    WaitForSeconds _fadeDelay;

    protected override void Initialize()
    {
        base.Initialize();
        if (CustomFadeDuration < 0)
        {
            CustomFadeDuration = FadeManager.Instance.FadeDuration;
        }
        _fadeDelay = new WaitForSeconds(CustomFadeDuration);
    }

    protected override IEnumerator RunSequence()
    {
        StartCutEffect();



        yield return StartFadeEffect(FadeOutGraphics, FadeOutCanvasGroups);

        yield return StartFadeEffect(FadeInGraphics, FadeInCanvasGroups);


        // 모든 페이드 효과가 완료될 때까지 기다립니다.
    }



    private IEnumerator StartFadeEffect(List<Graphic> graphics, List<CanvasGroup> canvasGroups)
    {
        if ((graphics == null || graphics.Count == 0) && (canvasGroups == null || canvasGroups.Count == 0))
        {
            yield break;
        }
        else
        {
            if (graphics.Count > 0)
            {
                for (int i = 0; i < graphics.Count; i++)
                {
                    FadeManager.Instance.ToggleFade(graphics[i], CustomFadeDuration);
                }
            }
            // 모든 그래픽에 대해 페이드 효과를 동시에 시작합니다.

            if (canvasGroups.Count > 0)
            {
                for (int i = 0; i < canvasGroups.Count; i++)
                {
                    FadeManager.Instance.ToggleFade(canvasGroups[i], CustomFadeDuration);
                }
            }
            yield return _fadeDelay;
        }

    }

    private void StartCutEffect()
    {
        if (CutINGraphics.Count < 1 && CutInCanvasGroups.Count < 1 && CutOutGraphics.Count < 1 && CutOutCanvasGroups.Count < 1)
        {
            return;
        }

        for (int i = 0; i < CutINGraphics.Count; i++)
        {
            FadeManager.Instance.SetAlphaOne(CutINGraphics[i]);
        }
        for (int i = 0; i < CutInCanvasGroups.Count; i++)
        {
            FadeManager.Instance.SetAlphaOne(CutInCanvasGroups[i]);
        }
        for (int i = 0; i < CutOutGraphics.Count; i++)
        {
            FadeManager.Instance.SetAlphaZero(CutOutGraphics[i]);
        }
        for (int i = 0; i < CutOutCanvasGroups.Count; i++)
        {
            FadeManager.Instance.SetAlphaZero(CutOutCanvasGroups[i]);
        }
    }

}