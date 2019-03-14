using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    #region Fields
    private bool _ignoreUpdate;
    private float _animeSpeedBackup = 1;
    private bool _isWeakToSong = true;
    #endregion

    #region Properties
    public bool IsWeakToSong
    {
        get { return _isWeakToSong; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnSongStart(Enemy enemy)
    {
        if (enemy != this)
        {
            Debug.Log("I am not the target");
            _ignoreUpdate = true;

            _animeSpeedBackup = Anim.speed;
            Anim.speed = 0;
        }
        else
        {
            Debug.Log("I AM the target");
        }
    }

    protected override void OnSongEnd()
    {
        if(_ignoreUpdate)
        {
            _ignoreUpdate = false;
            Anim.speed = _animeSpeedBackup;
        }
    }
    #endregion
}
