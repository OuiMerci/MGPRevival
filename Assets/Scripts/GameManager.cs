﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Fields
    public const float BASE_TIMESCALE = 1;
    [SerializeField] private int _nextLevel = 0;
    [SerializeField] private int _previousLevel = 0;

    private static GameManager _instance = null;
    public const int wallsLayer = 1 << 8;
    public const int magnetLayer = 1 << 11;
    public const int WALLS_AND_MAGNETS_LAYERMASK = wallsLayer | magnetLayer;

    public enum Gamestate
    {
        playing,
        song,
        pause
    }
    private Gamestate _currentState = Gamestate.playing;
    #endregion Fields

    #region properties
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public int NextLevel
    {
        get { return _nextLevel; }
    }

    public Gamestate CurrentState
    {
        get { return _currentState; }
    }
    #endregion properties

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoadingComplete;
        EventsManager.OnSongStart += OnSongStart;
        EventsManager.OnSongEnd += OnSongEnd;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoadingComplete;
        EventsManager.OnSongStart -= OnSongStart;
        EventsManager.OnSongEnd -= OnSongEnd;
    }

    private void OnSongStart(Enemy enemy)
    {
    }

    private void OnSongEnd()
    {
    }

    void OnLevelLoadingComplete(Scene level, LoadSceneMode mode)
    {
        Debug.Log("Level loading complete : " + level.buildIndex);
        if(level.buildIndex == 2)
        {
            LoadFirstLevel();
        }
        else if (_nextLevel != 3 && SceneManager.GetSceneByBuildIndex(_previousLevel).isLoaded == true)
        {
            Debug.Log("Unload current level : " + _previousLevel);
            SceneManager.UnloadSceneAsync(_previousLevel);
        }

        _previousLevel = _nextLevel;
    }

    void LoadFirstLevel()
    {
        _nextLevel = 3;
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        Debug.Log("Load Next level : " + _nextLevel);
        SceneManager.LoadScene(_nextLevel, LoadSceneMode.Additive);
    }

    public void OnLevelFinished()
    {
        Debug.Log("On level finished");
        _nextLevel++;
        LoadNextLevel();
    }

    public void UpdateTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetState(Gamestate newState)
    {
        _currentState = newState;
    }
    #endregion Methods
}
