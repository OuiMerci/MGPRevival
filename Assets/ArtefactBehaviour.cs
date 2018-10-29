using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactBehaviour : MonoBehaviour {

    #region Fields
    static private ArtefactBehaviour _instance = null;

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
    static public ArtefactBehaviour Instance
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

        SetArtifactActive(false);
        _rBody2D.gravityScale = 0;
        _isWithPlayer = true;

    }
	
	// Update is called once per frame
	void Update () {
	}

    private IEnumerator Spawn(Vector3 pos, Vector3 direction)
    {
        // The artefact is out


        // BLINDAGE _> Reset RBody velocity
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Initiate position
        transform.position = pos + _player.GetPlayerCenter();

        _rBody2D.AddForce(direction * _force, ForceMode2D.Impulse);
        _rBody2D.AddTorque(_firstRotForce, ForceMode2D.Impulse);
        _rBody2D.gravityScale = _gravityScale;
        _isWithPlayer = false;

        yield return new WaitForSeconds(_delayBeforeTP);
        _canTeleport = true;
    }

    public void SetArtifactActive(bool boolParam)
    {
        gameObject.SetActive(boolParam);
        //_spriteRenderer.enabled = boolParam;
        //_coll2D.enabled = boolParam;
    }

    public void StartSpawnCoroutine(Vector3 pos, Vector3 direction)
    {
        // Show the artefact
        SetArtifactActive(true);
        StartCoroutine(Spawn(pos, direction));
    }

    public void GoBackToPlayer()
    {
        _isWithPlayer = true;
        SetArtifactActive(false);
    }

    public void Reset()
    {
        // Reset RBody velocity
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Hide the artefact
        SetArtifactActive(false);

        // Is with Player
        _isWithPlayer = true;
        _canTeleport = false;
    }

    #endregion
}
