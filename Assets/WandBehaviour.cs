using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandBehaviour : MonoBehaviour {

    #region Fields
    static private WandBehaviour _instance = null;

    [SerializeField] private float _force = 0;

    private SpriteRenderer _spriteRenderer = null;
    private PlayerBehaviour _player = null;
    private Rigidbody2D _rBody2D = null;
    #endregion

    #region Properties
    static public WandBehaviour Instance
    {
        get { return _instance; }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = PlayerBehaviour.Instance;
        _rBody2D = GetComponent<Rigidbody2D>();

        SetVisible(false);
        _rBody2D.gravityScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetVisible(bool show)
    {
        _spriteRenderer.enabled = show;
    }

    public void Spawn(Vector3 pos, Vector3 direction)
    {
        transform.position = pos + _player.GetPlayerCenter();
        SetVisible(true);

        _rBody2D.AddForce(direction * _force);
        _rBody2D.gravityScale = 1;
    }
    #endregion
}
