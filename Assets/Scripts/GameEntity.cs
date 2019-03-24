using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEntity : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        Debug.Log("On enable test : " + gameObject.name);
        EventsManager.OnSongStart += OnSongStart;
        EventsManager.OnSongEnd += OnSongEnd;
    }

    protected virtual void OnDisable()
    {
        EventsManager.OnSongStart -= OnSongStart;
        EventsManager.OnSongEnd -= OnSongEnd;
    }

    protected abstract void OnSongStart(Enemy enemy);
    protected abstract void OnSongEnd();
}
