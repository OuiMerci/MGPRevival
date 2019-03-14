using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEntity : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.OnSongStart += OnSongStart;
        EventsManager.OnSongEnd += OnSongEnd;
    }

    protected abstract void OnSongStart(Enemy enemy);
    protected abstract void OnSongEnd();
}
