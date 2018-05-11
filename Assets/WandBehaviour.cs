using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandBehaviour : MonoBehaviour {

    #region Fields
    static private WandBehaviour _instance = null;

    [SerializeField] private float _force = 0;
    [SerializeField] private float _gravityScale = 0;
    [SerializeField] private float _firstRotForce = 0;
    [SerializeField] private float _delayBeforeTP = 0;

    private SpriteRenderer _spriteRenderer = null;
    private PlayerBehaviour _player = null;
    private Rigidbody2D _rBody2D = null;
    private Collider2D _coll2D = null;
    private bool _isWithPlayer = true;
    private bool _canTeleport = false;
    #endregion

    #region Properties
    static public WandBehaviour Instance
    {
        get { return _instance; }
    }

    public bool IsWithPlayer
    {
        get { return _isWithPlayer; }
    }

    public bool CanTeleport
    {
        get { return _canTeleport; }
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
        _coll2D = GetComponent<Collider2D>();

        SetVisible(false);
        _rBody2D.gravityScale = 0;
        _isWithPlayer = true;

    }
	
	// Update is called once per frame
	void Update () {
	}

    private IEnumerator Spawn(Vector3 pos, Vector3 direction)
    {
        // The wand is out


        // BLINDAGE _> Reset RBody velocity
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Initiate position
        transform.position = pos + _player.GetPlayerCenter();

        // Show the wand
        SetVisible(true);

        _rBody2D.AddForce(direction * _force, ForceMode2D.Impulse);
        _rBody2D.AddTorque(_firstRotForce, ForceMode2D.Impulse);
        _rBody2D.gravityScale = _gravityScale;
        _isWithPlayer = false;

        yield return new WaitForSeconds(_delayBeforeTP);
        _canTeleport = true;
    }

    public void SetVisible(bool show)
    {
        _spriteRenderer.enabled = show;
        _coll2D.enabled = show;
    }

    public void StartSpawnCoroutine(Vector3 pos, Vector3 direction)
    {
        StartCoroutine(Spawn(pos, direction));
    }

    public void GoBackToPlayer()
    {
        _isWithPlayer = true;
        SetVisible(false);
    }

    public void Reset()
    {
        // Reset RBody velocity
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Hide the wand
        SetVisible(false);

        // Is with Player
        _isWithPlayer = true;
        _canTeleport = false;
    }

    #endregion
}
