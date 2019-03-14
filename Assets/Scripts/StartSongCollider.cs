using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSongCollider : MonoBehaviour
{
    private BoxCollider2D _collider;
    private Collider2D[] _overlapingColliders;
    private ContactFilter2D _contactFilter;
    private int _filterLayerMask;
    private PlayerBehaviour _player;

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _overlapingColliders = new Collider2D[5];
        _player = PlayerBehaviour.Instance;

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
        int collidersCount = _collider.OverlapCollider(_contactFilter, _overlapingColliders);
        if (collidersCount > 0)
        {
            Debug.Log("found enemies : " + collidersCount + " --- First one :" + _overlapingColliders[0]);

            // an enemy or more have been found, get the closest one and try to start a song
            GetClosestEnemyTarget(collidersCount);
            DisableCollider();          
        }
    }

    private void GetClosestEnemyTarget(int collidersCount)
    {
        float shortestDistance = 999;
        Enemy target = null;

        // test the distance of every enemy from the player, to find the closest one
        for(int i = 0; i < collidersCount; i++)
        {
            Enemy enemy = _overlapingColliders[i].GetComponent<Enemy>();
            if(enemy == null)
            {
                Debug.LogError("This object is in enemy layer but doesn't have a Enemy script attached. Please fix this.");
                return;
            }

            float enemyDist = Vector2.Distance(enemy.GetCharacterCenter(), _player.GetCharacterCenter());

            if(enemyDist < shortestDistance)
            {
                shortestDistance = enemyDist;
                target = enemy;
            }
        }

        if(target == null)
        {
            Debug.LogError("The target for the song is null, this shouldn't happen as the array isn't supposed to be empty when this method is called.");
            return;
        }

        _player.TryStartSong(target);
    }

    private void StartSong()
    {

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
