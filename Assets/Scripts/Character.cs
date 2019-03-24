using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Character : GameEntity
{
    #region fields
    [SerializeField] private float _speed = 0.0f;
    private SpriteRenderer _spriteRenderer = null;
    private Animator _anim = null;
    private Collider2D _collider2D = null;
    #endregion

    #region Properties
    public SpriteRenderer SpriteRenderer
    {
        get { return _spriteRenderer; }
    }

    public Animator Anim
    {
        get { return _anim; }
    }

    public float Speed
    {
        get { return _speed; }
    }

    public float VerticalExtent
    {
        get { return _collider2D.bounds.extents.y; }
    }

    public float HorizontalExtent
    {
        get { return _collider2D.bounds.extents.x; }
    }

    public Collider2D Coll2D
    {
        get { return _collider2D; }
    }

    public int Orientation
    {
        get { return _spriteRenderer.flipX ? -1 : 1; }
    }
    #endregion

    #region Methods
    // Use this for initialization
    protected virtual void Start()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
    }

    public Vector3 GetCharacterCenter()
    {
        return _spriteRenderer.sprite.bounds.center + transform.position;
    }
    #endregion
}