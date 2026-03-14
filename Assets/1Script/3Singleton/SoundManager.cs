using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectSoundNum
{
    // ...existing code...
    BGM,
    SaveSound,      // 답변 저장 소리
    SoulPieceSound, // 마음 조각 뜨는 소리
    ConfirmSound,   // 사용자 확인 완료음
    PopupSound,     // 팝업 뜨는 소리
    ActiveSound,     // 활성화음
    StepTextSound,
    MorseDotSound_1,
    MorseDashSound_1,
    MorseDotSound_2,
    MorseDashSound_2,
    SignalReceiveSound, // 마음신호 도착음
    SignalSendSound,     // 전송음
    ArduinoButtonSound,     // 아두이노 버튼 사운드
    MorseResetSound,     // 모스 신호 초기화 사운드

}
public class SoundManager : Singleton<SoundManager>, IJsonGenericTarget
{


    JsonGenericUpData _genericData = new JsonGenericUpData();

    AudioSource[] audioSources;

    float _baseVolume = 0.6f;
    float[] _volumes = new float[System.Enum.GetValues(typeof(EffectSoundNum)).Length];




    [Header("BGM = 0 \n SaveSound = 1 \n SoulPieceSound = 2 \n ConfirmSound = 3 \n PopupSound = 4 \n ActiveSound = 5 \n StepTextSound = 6 \n MorseDotSound_1 = 7 \n MorseDashSound_1 = 8 \n MorseDotSound_2 = 9 \n MorseDashSound_2 = 10 \n SignalReceiveSound = 11 \n SignalSendSound = 12 \n ArduinoButtonSound = 13 \n MorseResetSound = 14")]
    public bool headerYoung = false; //헤더용 


    //TODO 제이슨 뺴서 정리하기
    void Start()
    {

    }


    public void MuteBGM()
    {
        AudioSource tempAudioSource = audioSources[(int)(EffectSoundNum.BGM)];

        if (tempAudioSource == null) return;
        tempAudioSource.volume = 0f;
        tempAudioSource.Stop();

    }
    public void PlayBGM()
    {
        if (audioSources[(int)(EffectSoundNum.BGM)] == null) return;
        audioSources[(int)(EffectSoundNum.BGM)].Play();
    }

    public void PlayingLoopSound()
    {

        if (audioSources == null || audioSources[(int)(EffectSoundNum.MorseResetSound)] == null || audioSources[(int)(EffectSoundNum.MorseResetSound)].isPlaying)
        {
            return;
        }

        audioSources[(int)(EffectSoundNum.MorseResetSound)].Play();

    }
    public void StopLoopSound()
    {
        if (audioSources == null || audioSources[(int)(EffectSoundNum.MorseResetSound)] == null || !audioSources[(int)(EffectSoundNum.MorseResetSound)].isPlaying)
        {
            return;
        }
        audioSources[(int)(EffectSoundNum.MorseResetSound)].Stop();

    }

    public void PlayEffectSound(EffectSoundNum effectSoundNum)
    {
        if (GameManager.Instance.IsStarted == false)
        {
            Debug.Log("Game Not Started Yet");
            return;
        }
        Debug.Log("Attempting to play sound: " + effectSoundNum.ToString());


        if (audioSources[(int)effectSoundNum] != null)
        {
            audioSources[(int)effectSoundNum].Play();
        }
        // Debug.Log("Played sound: " + effectSoundNum.ToString() + " with volume: " + soundVolume);

    }
    public void StopEffectSound(EffectSoundNum effectSoundNum)
    {
        if (GameManager.Instance.IsStarted == false)
        {
            Debug.Log("Game Not Started Yet");
            return;
        }

        if (audioSources[(int)effectSoundNum] != null)
        {
            audioSources[(int)effectSoundNum].Stop();
        }
        //  Debug.Log("Stopped sound: " + effectSoundNum.ToString() + " with volume: " + soundVolume);

    }
    public void Initialize(JsonGenericUpData data)
    {
        _genericData = data;
        if (data.floatParams.TryGetValue("baseVolume", out float baseVolume))
        {
            _baseVolume = baseVolume;
            Debug.Log("Base Volume set to: " + _baseVolume);
        }

        foreach (EffectSoundNum sound in System.Enum.GetValues(typeof(EffectSoundNum)))
        {
            string key = sound.ToString();
            if (data.floatParams.TryGetValue(key, out float volume))
            {
                _volumes[(int)sound] = volume;
            }
        }
        audioSources = GetComponentsInChildren<AudioSource>();


        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = _baseVolume * _volumes[audioSource.transform.GetSiblingIndex()];
                Debug.Log("Set volume for " + audioSource.gameObject.name + ": " + audioSource.volume);

            }
        }
        audioSources[(int)(EffectSoundNum.BGM)].volume = _volumes[0]; // BGM 볼륨 설정

        PlayBGM();


    }
    public JsonGenericUpData Data()
    {
        _genericData.intParams = new Dictionary<string, int>();
        _genericData.floatParams = new Dictionary<string, float>();
        _genericData.boolParams = new Dictionary<string, bool>();
        _genericData.stringParams = new Dictionary<string, string>();


        _genericData.floatParams["baseVolume"] = _baseVolume;
        foreach (EffectSoundNum sound in System.Enum.GetValues(typeof(EffectSoundNum)))
        {
            _genericData.floatParams[sound.ToString()] = _volumes[(int)sound];
        }

        return _genericData;
    }
}
