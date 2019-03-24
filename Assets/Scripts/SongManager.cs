using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform _songZoneCenter;

    private enum Action
    {
        Idle,
        WaitStartSongEffects,
        Song,
        WaitEndSongEffects
    }
    static private Action _action;

    static private Transform _songZoneCenterStatic;
    static private bool _cameraReady;
    static private bool _actorsMoverReady;
    //private GameManager _gManager;
    #endregion

    #region Properties
    public static Transform SongZoneCenter
    {
        get { return _songZoneCenterStatic; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        //_gManager = GameManager.Instance;
        _songZoneCenterStatic = _songZoneCenter;
        _action = Action.Idle;
    }

    // Update is called once per frame
    void Update()
    {
    }

    static public void StartSong(Enemy enemy)
    {
        Debug.Log("COOL, start song");

        // Teleport Camera and Actors
        _action = Action.WaitStartSongEffects;
        GameManager.Instance.SetState(GameManager.Gamestate.song);
        EventsManager.FireSongStartEvent(enemy);
    }

    static private void StartSongGameplay()
    {
        _action = Action.WaitEndSongEffects;
    }

    static public void EndSong()
    {
        GameManager.Instance.SetState(GameManager.Gamestate.playing);
        EventsManager.FireSongEndEvent();
    }

    static public void EndSongGameplay()
    {
        Debug.Log("OK, END song GAMEPLAY");
        EventsManager.FireSongGameplayEndEvent();
    }

    static public void OnCameraReady()
    {
        Debug.Log("CAMERA READY");
        _cameraReady = true;
        CheckReadyState();
    }

    static public void OnActorsMoversReady()
    {
        Debug.Log("MOVER READY");
        _actorsMoverReady = true;
        CheckReadyState();
    }

    static private void CheckReadyState()
    {
        if (_cameraReady == false || _actorsMoverReady == false)
            return;

        _cameraReady = false;
        _actorsMoverReady = false;

        switch (_action)
        {
            case Action.WaitStartSongEffects:
                Debug.Log("START SONG GAMEPLAY");
                StartSongGameplay();
                break;
            case Action.WaitEndSongEffects:
                Debug.Log("END SONG");
                EndSong();
                break;
        }
    }
    #endregion
}
