using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMagnet : MonoBehaviour
{
    public Vector3 speed;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = speed * Time.deltaTime;
        //transform.position += speed * Time.deltaTime;
    }
}
