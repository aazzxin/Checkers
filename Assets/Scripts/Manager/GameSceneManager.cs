using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GameRunManager))]
[RequireComponent(typeof(HUDManager))]
[RequireComponent(typeof(GameUIManger))]
/// <summary>
/// 管理所有 Manager 的主管理器
/// </summary>
public class GameSceneManager : MonoBehaviour {

    private static GameSceneManager s_instance;
    public static GameRunManager runManager { get; private set; }
    public static HUDManager hudManager { get; private set; }
    public static GameUIManger uiManager { get; private set; }

    private List<IGameManager> _startSequence;

    void Awake()
    {
        s_instance = this;
        _startSequence = new List<IGameManager>();

        runManager = GetComponent<GameRunManager>();
        hudManager = GetComponent<HUDManager>();
        uiManager = GetComponent<GameUIManger>();

        // 添加顺序不要改
        _startSequence.Add(runManager);
        _startSequence.Add(hudManager);
        _startSequence.Add(uiManager);

        StartCoroutine(StartupManagers());
    }

    public static GameSceneManager Instance { get { return s_instance; } }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }
        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    ++numReady;
                }
            }

            if (numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }
            yield return null;
        }
    }

    public IEnumerator ResetManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Reset();
        }
        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    ++numReady;
                }
            }

            if (numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }
            yield return null;
        }
    }

    public void shutDownAllManager()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.ShutDown();
        }
    }

    public void ReturnTitle()
    {
        //返回主菜单标题界面
        SceneManager.LoadScene("TitleScene");
    }
}
