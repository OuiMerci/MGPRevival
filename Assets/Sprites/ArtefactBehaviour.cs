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

    private const int LEVELSTRUCTURE_LAYERMASK = 1 << 8;
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

        //Physics2D.IgnoreCollision(_coll2D, _player.Coll2D, true);
        SetArtifactActive(false);
        _rBody2D.gravityScale = 0;
        _isWithPlayer = true;

    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log( collision.otherCollider.name + " colliding with : " + collision.collider.name);
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

    public Vector2 GetTPPosition()
    {
        RaycastHit2D hit;
        bool useTweakedX = false;
        bool useTweakedY = false;
        float tweakedX = 0;
        float tweakedY = 0;

        //If the sprite pivot is not in the center, but at the bottom, we need some tweaking.
        float spritePivotFix = _player.VerticalExtent;

        //raycast down
        hit = Physics2D.Raycast(transform.position, Vector2.down, _player.VerticalExtent, LEVELSTRUCTURE_LAYERMASK);
        //Debug.DrawLine(transform.position, transform.position + (Vector3.down * _player.VerticalExtent), Color.green);
        if (hit.collider != null)
        {
            useTweakedY = true;
            tweakedY = hit.point.y + _player.VerticalExtent - spritePivotFix;
            Debug.Log("Colliding bot with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);
        }
        else
        {
            //raycast up
            hit = Physics2D.Raycast(transform.position, Vector2.up, _player.VerticalExtent, LEVELSTRUCTURE_LAYERMASK);
            //Debug.DrawLine(transform.position, transform.position + (Vector3.up * _player.VerticalExtent), Color.blue);
            if (hit.collider != null)
            {
                useTweakedY = true;
                tweakedY = hit.point.y - _player.VerticalExtent - spritePivotFix;
                Debug.Log("Colliding up with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);
            }
        }

        //raycast left
        hit = Physics2D.Raycast(transform.position, Vector2.left, _player.HorizontalExtent, LEVELSTRUCTURE_LAYERMASK);
        //Debug.DrawLine(transform.position, transform.position + (Vector3.left * _player.HorizontalExtent), Color.red);
        if (hit.collider != null)
        {
            useTweakedX = true;
            tweakedX = hit.point.x + _player.HorizontalExtent;
            Debug.Log("Colliding left with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);
        }
        else
        {
            //raycast right
            hit = Physics2D.Raycast(transform.position, Vector2.right, _player.HorizontalExtent, LEVELSTRUCTURE_LAYERMASK);
            //Debug.DrawLine(transform.position, transform.position + (Vector3.right * _player.HorizontalExtent), Color.yellow);
            if (hit.collider != null)
            {
                useTweakedX = true;
                tweakedX = hit.point.x + _player.HorizontalExtent;
                Debug.Log("Colliding right with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);
            }
        }

        //Set and return tweaked position
        Vector2 tweakedPosition = transform.position;

        if (useTweakedX && useTweakedY)
        {
            tweakedPosition = new Vector2(tweakedX, tweakedY);
            Debug.Log("Used Tweaked X AND Y");
        }
        else if (useTweakedX)
        {
            tweakedPosition = new Vector2(tweakedX, transform.position.y);
            Debug.Log("Used Tweaked X");
        }
        else if(useTweakedY)
        {
            tweakedPosition = new Vector2(transform.position.x, tweakedY);
            Debug.Log("Used Tweaked Y");
        }

        Debug.Log("Art. position : " + transform.position + " tweaked position : " + tweakedPosition);
        return tweakedPosition;
        
    }
    #endregion
}
