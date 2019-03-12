using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    private PlayerBehaviour _player;
    private List<Collider2D> _collidingGrounds;

    #region Properties
    public GameObject Parent
    {
        get { return _parent; }
    }
    #endregion

    private void Start()
    {
        _player = PlayerBehaviour.Instance;
        _collidingGrounds = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_collidingGrounds.Contains(other) == false)
        {
            _collidingGrounds.Add(other);
            _player.Grounded = true;
            //Debug.Log("Grounded TRUE");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_collidingGrounds.Contains(other) == true)
        {
            _collidingGrounds.Remove(other);

            if(_collidingGrounds.Count < 1)
                _player.Grounded = false;

            //Debug.Log("Grounded FALSE");
        }
    }
}
