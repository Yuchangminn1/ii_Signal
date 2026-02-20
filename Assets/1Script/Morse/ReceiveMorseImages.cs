using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveMorseImages : MorseImage
{
    readonly float SoundPosX = 1650f;
    Coroutine spawnSoundCoroutine = null;

    public bool IsDebug = false;


    override public void SetMorseType(MorseType morseType)
    {
        base.SetMorseType(morseType);

        if (spawnSoundCoroutine != null)
        {
            StopCoroutine(spawnSoundCoroutine);
            spawnSoundCoroutine = null;
        }
        spawnSoundCoroutine = StartCoroutine(SpawnSoundCoroutine());
    }

    public IEnumerator SpawnSoundCoroutine()
    {
        if (IsDebug)
        {
            Debug.Log($"{name} /  {_rectTransform.position.x}");
        }
        while (_rectTransform.position.x > SoundPosX)
        {
            if (IsDebug)
                Debug.Log($"{name} /  {_rectTransform.position.x}");

            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        if (_currentMorseType == MorseType.Dot)
        {
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDotSound_2);
        }
        else if (_currentMorseType == MorseType.Dash)
        {
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDashSound_2);

        }
    }
    override public void Reset()
    {
        base.Reset();
        if (spawnSoundCoroutine != null)
        {
            StopCoroutine(spawnSoundCoroutine);
            spawnSoundCoroutine = null;
        }
    }
}
