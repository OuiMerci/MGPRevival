using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    #region Fields
    private GameManager _gManager;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        _gManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void StartSong(Enemy enemy)
    {
        Debug.Log("COOL, start song");
        GameManager.Instance.SetState(GameManager.Gamestate.song);
        EventsManager.FireSongStartEvent(enemy);
    }

    static public void EndSong()
    {
        Debug.Log("OK, END song");
        GameManager.Instance.SetState(GameManager.Gamestate.playing);
        EventsManager.FireSongEndEvent();
    }
    #endregion
}
