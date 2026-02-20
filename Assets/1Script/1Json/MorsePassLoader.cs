using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMorsePassTarget
{
    void Initialize(QuestionInfo items);
    QuestionInfo Data();
}
public class MorsePassLoader : JsonLoaderBase<QuestionInfo, IMorsePassTarget>
{
    // 예: new QuestionLoader("Json/QuestionConfig.json")
    public MorsePassLoader(string jsonPath) : base(jsonPath) { }

    public override void ApplyLoadedData(string name, IMorsePassTarget obj)
    {
        if (obj == null) return;
        if (!TryGetData(name, out var items) || items == null)
        {
            Debug.LogWarning($"MorsePassLoader: No data found for key '{name}'");
            return;
        }
        obj.Initialize(items);
    }

    public override void JsonDataUpdate()
    {
        foreach (var kv in _target)
        {
            var target = kv.Value;
            if (target == null) continue;
            var items = target.Data();
            if (items != null)
            {
                data[kv.Key] = items;
            }
        }
        Save();
    }

    // 편의 헬퍼: 컨테이너 이름(기본 "SeasonContainer")으로 질문 리스트를 반환
    public QuestionInfo GetQuestions(string containerName = "SeasonContainer")
    {
        if (TryGetData(containerName, out var items) && items != null)
        {
            return items;
        }
        return new QuestionInfo();
    }
}
