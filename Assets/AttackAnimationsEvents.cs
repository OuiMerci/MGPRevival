using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationsEvents : MonoBehaviour
{
    [SerializeField] private StartSongCollider _songStarterCollider;
    [SerializeField] private BoxCollider2D _attackCollider;

    #region Methods
    public void EnableSongStarterCollider()
    {
        _songStarterCollider.EnableCollider();
    }

    public void DisableSongStarterCollider()
    {
        _songStarterCollider.DisableCollider();
    }

    public void EnableAttackCollider()
    {

        _attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        _attackCollider.enabled = false;
    }
    #endregion
}
