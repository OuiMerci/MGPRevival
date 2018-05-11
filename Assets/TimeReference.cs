using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeReference : MonoBehaviour {

    [SerializeField] private float _speed = 0;
    [SerializeField] private float _xLimit = 0;
    private int _direction = 1;

	// Update is called once per frame
	void Update () {

        // Compute movement
        float movement = _speed * _direction * Time.deltaTime;

        //Apply movement
        transform.position = new Vector3(transform.position.x + movement, transform.position.y, 0);

        // Check if limit reached
        _direction = Mathf.Abs(transform.position.x) >= _xLimit ? _direction * -1 : _direction;
        //Debug.Log(" test : " + (Mathf.Abs(transform.position.x) >= _xLimit) + "   dir :  " + _direction);

    }
}