using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSongCollider : MonoBehaviour
{
    private BoxCollider2D _collider;
    private Collider2D[] _overlapingColliders;
    private ContactFilter2D _contactFilter;
    private int _filterLayerMask;

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _overlapingColliders = new Collider2D[5];

        // Set a filter only for enemy layer
        _filterLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        _contactFilter.SetLayerMask(_filterLayerMask);
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision !! " + other.name);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Stay !! " + other.name);
    }

    private void CheckOverlap()
    {
        int res = _collider.OverlapCollider(_contactFilter, _overlapingColliders);
        if (res > 0)
        {
            // deactivate collider
            // check is enemy enemy can be sung to (or try to another enemy on the list?)
            // call StartSong(enemy)
            Debug.Log("found enemy : " + _overlapingColliders[0]);
            DisableCollider();
        }
    }

    public void EnableCollider()
    {
        _collider.enabled = true;
        CheckOverlap();
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }
    #endregion
}
