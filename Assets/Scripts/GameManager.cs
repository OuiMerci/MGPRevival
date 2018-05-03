using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Fields
    [SerializeField] private int _nextLevel = 0;
    [SerializeField] private int _previousLevel = 0;

    private static GameManager _instance = null;
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
    #endregion properties

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoadingComplete;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoadingComplete;
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
    #endregion Methods
}
