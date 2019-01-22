using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewHitInfo", menuName = "HitInfo")]
public class HitInfo : ScriptableObject
{
    #region Fields
    [SerializeField] private string _name;
    [SerializeField] private int _damage;
    [SerializeField] private float _ComboDelay;
    [SerializeField] private ComboManager.AttackAnimations _anim;
    //[SerializeField] private ComboManager.AttackAnimations _playerAnimation;
    [SerializeField] private HitInfo[] _linkedHits;
    [SerializeField] private ComboManager.Inputs[] _linkedInputs;
    //[SerializeField] private ComboManager.HitboxType _hitBox;
    //[SerializeField] private AudioClip _sound;
    #endregion

    #region Properties
    public string HitName
    {
        get { return _name; }
    }

    public int Damage
    {
        get { return _damage; }
    }

    public float ComboDelay
    {
        get { return _ComboDelay; }
    }

    public ComboManager.AttackAnimations Animation
    {
        get { return _anim; }
    }

    public ComboManager.Inputs[] LinkedInputs
    {
        get { return _linkedInputs; }
    }

    public HitInfo[] LinkedHits
    {
        get { return _linkedHits; }
    }
    #endregion
}
