using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    #region fields
    [SerializeField] private SpriteRenderer _aimArrowRenderer = null;
    [SerializeField] private float _speed = 0.0f;
    [SerializeField] private float _dashingSpeed = 0.0f;
    [SerializeField] private float _artefactOffset = 0.0f;

    static private PlayerBehaviour _instance = null;
    private SpriteRenderer _spriteRenderer = null;
    private ArtefactBehaviour _artefact = null;
    private Animator _anim = null;
    private Collider2D _collider2D = null;
    private int _runHash = Animator.StringToHash("isRunning");
    private int _dashHash = Animator.StringToHash("isDashing");
    private int _aimingHash = Animator.StringToHash("isAiming");

    private bool _isAiming = false;

    #endregion

    #region Properties
    static public PlayerBehaviour Instance
    {
        get { return _instance; }
    }

    public SpriteRenderer SpriteRenderer
    {
        get { return _spriteRenderer; }
    }

    public bool IsAiming
    {
        get { return _isAiming; }
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
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _artefact = ArtefactBehaviour.Instance;
        _collider2D = GetComponent<Collider2D>();

        _aimArrowRenderer.transform.position = GetPlayerCenter();
        ShowAimingArrow(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Update player's position and handle animations.
    /// </summary>
    /// <param name="movement">Movement added to the player's position</param>
    public void Move(Vector3 movement, bool isDashing = false)
    {
        if(movement != Vector3.zero)
        {
            float actualSpeed = isDashing ? _dashingSpeed : _speed;
            Vector3 newPos = transform.position + movement * actualSpeed * Time.deltaTime;
            transform.position = newPos; // Ajouter une fonction "ClampPosition" si nécessaire.

            // Handle animations
            _anim.SetBool(_runHash, true);
            _anim.SetBool(_dashHash, isDashing);

            _spriteRenderer.flipX = movement.x < 0 ? true : false;
        }
        else
        {
            // Handle animations
            _anim.SetBool(_runHash, false);
            _anim.SetBool(_dashHash, false);
        }
    }

    public void ApplyAimRotation(Vector3 angle)
    {
        //Debug.Log("Apply angle : " + angle);
        ShowAimingArrow(true);
        _aimArrowRenderer.transform.eulerAngles = angle;
    }

    public void ShowAimingArrow(bool show)
    {
        _aimArrowRenderer.enabled = show;
    }

    public void SetAimingState(bool isAiming)
    {
        _isAiming = isAiming;
        _anim.SetBool(_aimingHash, isAiming);
    }

    public void ThrowArtefact(Vector3 direction)
    {
        _artefact.StartSpawnCoroutine(direction.normalized * _artefactOffset, direction);
    }

    public void TeleportToArtefact()
    {
        // Get tweaked position from the artefact and teleport to it
        transform.position = _artefact.GetTPPosition(); ;
        _artefact.Reset();
    }

    public Vector3 GetPlayerCenter()
    {
        return _spriteRenderer.sprite.bounds.center + transform.position;
    }
    #endregion
}