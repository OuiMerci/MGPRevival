using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedMovement
{
    #region Fields
    private float _force;
    private Vector2 _direction;
    private double _startTime;
    private float _duration;
    #endregion

    #region Methods
    public ForcedMovement(float force, Vector2 direction, float duration)
    {
        _force = force;
        _direction = direction;
        _duration = duration;
        _startTime = Time.time;
    }

    public Vector2 AddMovement(Vector2 baseVector)
    {
        baseVector += _force * _direction * Time.deltaTime;
        Debug.Log("Adding : " + baseVector);
        return baseVector;
    }

    public bool IsAlive()
    {
        bool alive = _startTime + _duration > Time.time;
        return alive;
    }
    #endregion
}
