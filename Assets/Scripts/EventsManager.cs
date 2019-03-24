using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    #region Events
    public delegate void Demagnetised();
    public static Demagnetised OnDemagnetised;

    public delegate void SongStart(Enemy enemy);
    public static SongStart OnSongStart;

    public delegate void SongGameplayEnd();
    public static SongGameplayEnd OnSongGameplayEnd;

    public delegate void SongEnd();
    public static SongEnd OnSongEnd;
    #endregion

    #region Methods
    public static void FireDemagnetisedEvent()
    {
        if (OnDemagnetised != null)
            OnDemagnetised();
    }

    public static void FireSongStartEvent(Enemy enemy)
    {
        if (OnSongStart != null)
            OnSongStart(enemy);
    }

    public static void FireSongGameplayEndEvent()
    {
        if (OnSongGameplayEnd != null)
            OnSongGameplayEnd();
    }

    public static void FireSongEndEvent()
    {
        if (OnSongEnd != null)
            OnSongEnd();
    }
    #endregion
}
