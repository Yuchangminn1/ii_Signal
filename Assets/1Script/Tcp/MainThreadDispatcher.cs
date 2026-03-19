using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 백그라운드 스레드에서 메인 스레드로 작업을 분배하는 유틸리티
/// 네트워크 스레드에서 Unity API를 안전하게 호출할 수 있도록 함
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;
    private static readonly Queue<Action> _actionQueue = new Queue<Action>();
    private static readonly object _lockObject = new object();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (_lockObject)
        {
            while (_actionQueue.Count > 0)
            {
                Action action = _actionQueue.Dequeue();
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[MainThreadDispatcher] Error executing action: {e.Message}\n{e.StackTrace}");
                }
            }
        }
    }

    /// <summary>
    /// 메인 스레드에서 실행할 작업을 큐에 추가
    /// </summary>
    public static void RunOnMainThread(Action action)
    {
        if (action == null) return;

        lock (_lockObject)
        {
            _actionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// MainThreadDispatcher가 없으면 자동으로 생성
    /// </summary>
    public static void EnsureCreated()
    {
        if (_instance == null)
        {
            GameObject dispatcherGO = new GameObject("MainThreadDispatcher");
            dispatcherGO.AddComponent<MainThreadDispatcher>();
        }
    }
}
