using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // ← 반드시 using 추가

public interface IJsonLoader
{
    void Load();
    string JsonPath { get; set; }
}

public abstract class JsonLoaderBase<DataType, TargetType> : IJsonLoader
{
    public string JsonPath { get; set; }

    // JSON을 그대로 Dictionary로 로드
    protected Dictionary<string, DataType> data = new Dictionary<string, DataType>();

    // 등록된 오브젝트 (예: Text, Image, Audio 등)
    protected readonly Dictionary<string, TargetType> _target = new Dictionary<string, TargetType>();
    private const int JsonLoadTimeoutSeconds = 10;
    protected JsonLoaderBase(string jsonPath)
    {
        JsonPath = jsonPath;
    }

    public void Load()
    {
        data = LoadData();
    }

    public IEnumerator LoadCoroutine()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        yield return LoadDataCoroutine();
#else
        Load();
        yield break;
#endif
    }

    public virtual void SetJsonPath(string jsonPath)
    {
        JsonPath = jsonPath;
        Load();
    }

    private Dictionary<string, DataType> LoadData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, JsonPath);

        if (!File.Exists(path))
        {
            Debug.LogError($"Json file not found: {path}");
            return new Dictionary<string, DataType>();
        }

        string json = File.ReadAllText(path);
        return DeserializeData(json, path);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator LoadDataCoroutine()
    {
        string path = Path.Combine(Application.streamingAssetsPath, JsonPath);

        using (var request = UnityWebRequest.Get(path))
        {
            request.timeout = JsonLoadTimeoutSeconds;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                data = DeserializeData(request.downloadHandler.text, path);
                yield break;
            }

            Debug.LogError($"Failed to load JSON from {path}: {request.error}");
            data = new Dictionary<string, DataType>();
        }
    }
#endif

    private Dictionary<string, DataType> DeserializeData(string json, string path)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning($"Json file is empty: {path}");
            return new Dictionary<string, DataType>();
        }

        var loadedData = JsonConvert.DeserializeObject<Dictionary<string, DataType>>(json);
        return loadedData ?? new Dictionary<string, DataType>();
    }

    public void Register(string name, TargetType obj)
    {
        if (!_target.ContainsKey(name))
            _target.Add(name, obj);

        ApplyLoadedData(name, obj);
    }

    public bool TryGet(string name, out TargetType obj)
    {
        return _target.TryGetValue(name, out obj);
    }

    public bool TryGetData(string name, out DataType item)
    {
        return data.TryGetValue(name, out item);
    }

    // public void Save()
    // {
    //     string path = Path.Combine(Application.streamingAssetsPath, JsonPath);
    //     var json = JsonConvert.SerializeObject(data, Formatting.Indented);
    //     File.WriteAllText(path, json);
    //     Debug.Log($"Saved JSON → {path}");
    // }
    public void Save()
    {
        string path = Path.Combine(Application.streamingAssetsPath, JsonPath);

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore   // 루프 무시
        };
        var ordered = new SortedDictionary<string, DataType>(data);
        string json = JsonConvert.SerializeObject(ordered, settings);
        File.WriteAllText(path, json);

        Debug.Log($"Saved JSON → {path}");
    }

    public void Sol()
    {
        var ordered = new SortedDictionary<string, DataType>(data);
        data = new Dictionary<string, DataType>(ordered);
        Save();
    }



    public abstract void ApplyLoadedData(string name, TargetType obj);

    public abstract void JsonDataUpdate();

    public void CaptureData()
    {
        var keysToRemove = new List<string>();
        foreach (var kv in data)
        {
            _target.TryGetValue(kv.Key, out var existingTarget);
            if (existingTarget == null)
            {
                Debug.Log($"kv.Key{kv.Key}  Is Null ");
                keysToRemove.Add(kv.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            data.Remove(key);
        }
        JsonDataUpdate();
    }
}
