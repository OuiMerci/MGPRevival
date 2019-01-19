using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _delay;
    [SerializeField] private Transform _destination;

    private Rigidbody2D _rb;
    private Vector3 _start;
    private Vector3 _dest;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _dest = _destination.position;
        _start = transform.position;

        // Grab a free Sequence to use
        Sequence mySequence = DOTween.Sequence();
        // Add a movement tween at the beginning
        mySequence.Append(_rb.DOMove(_dest, _duration).SetDelay(_delay));
        // Add a a backward movement
        mySequence.Append(_rb.DOMove(_start, _duration).SetDelay(_delay));
        // Add loop
        mySequence.SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
