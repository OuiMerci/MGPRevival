using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    private PlayerBehaviour _player;

    #region Properties
    public GameObject Parent
    {
        get { return _parent; }
    }
    #endregion

    private void Start()
    {
        _player = PlayerBehaviour.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Grounded TRUE");
        _player.Grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Grounded FALSE");
        _player.Grounded = false;
    }
}
