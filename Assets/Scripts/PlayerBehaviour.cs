using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour {

    #region fields
    [SerializeField] private float _timeSlowRatio = 1;
    [SerializeField] private SpriteRenderer _aimArrowRenderer = null;
    [SerializeField] private float _speed = 0.0f;
    [SerializeField] private float _dashingSpeed = 0.0f;
    [SerializeField] private float _artefactOffset = 0.0f;
    [SerializeField] private float _baseTPTweeningDuration = 0.0f;
    [SerializeField] private float _distanceTweeningDurationRatio = 0.0f;
    [SerializeField] private GameObject _tweeningSprite = null;

    public enum PlayerState // set back to private once _state is hidden
    {
        Free,
        Hanging,
        Tweening
    }
    public PlayerState _state;

    public enum ClampedAimEnum
    {
        top, left, right, topLeft, topRight
    }
    private ClampedAimEnum _clampedAim;

    static private PlayerBehaviour _instance = null;
    private SpriteRenderer _spriteRenderer = null;
    private ArtefactBehaviour _artefact = null;
    private Animator _anim = null;
    private Collider2D _collider2D = null;
    private GameManager _gameManager = null;
    private Rigidbody2D _rBody2D = null;
    private int _runHash = Animator.StringToHash("isRunning");
    private int _dashHash = Animator.StringToHash("isDashing");
    private int _aimingHash = Animator.StringToHash("isAiming");
    private bool _isAiming = false;
    private Joint2D _joint2D = null;
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

    public bool IsFree
    {
        get { return _state == PlayerState.Free; }
    }

    public bool IsHanging
    {
        get { return _state == PlayerState.Hanging; }
    }

    public bool IsTweening
    {
        get { return _state == PlayerState.Tweening; }
    }

    public Collider2D Coll2D
    {
        get { return _collider2D; }
    }

    public ClampedAimEnum ClampedAim
    {
        get { return _clampedAim; }
        set { _clampedAim = value; }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        EventsManager.OnDemagnetised += OnArtefactDemagnetised;
    }

    private void OnDisable()
    {
        EventsManager.OnDemagnetised -= OnArtefactDemagnetised;
    }

    // Use this for initialization
    void Start () {
        _gameManager = GameManager.Instance;
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _artefact = ArtefactBehaviour.Instance;
        _collider2D = GetComponent<Collider2D>();
        _rBody2D = GetComponent<Rigidbody2D>();
        _joint2D = GetComponent<Joint2D>();

        _aimArrowRenderer.transform.position = GetPlayerCenter();
        ShowAimingArrow(false);
        
        // Init de DoTween ?
    }

    private void OnArtefactDemagnetised()
    {
        if(_state == PlayerState.Hanging)
        {
            EndHanging(PlayerState.Free);
        }
    }

    private void SetAimingState(bool isAiming)
    {
        _isAiming = isAiming;
        _anim.SetBool(_aimingHash, isAiming);
    }

    private void ShowTweenRenderer(bool show)
    {
        _spriteRenderer.enabled = !show;
        _tweeningSprite.SetActive(show);
    }

    private void OnTeleportComplete()
    {
        // Hide Tween Sprite
        ShowTweenRenderer(false);

        // If the artfact isn't magnetised, reset its parameters
        if(_artefact.IsMagnetised || _artefact.IsSnapping)
        {
            StartHanging();
        }
        else
        {
            SetPlayerState(PlayerState.Free); // update state
            _rBody2D.isKinematic = false; // set back kinematic
            _artefact.ResetArtifact();
        }
    }

    private void StartHanging()
    {
        SetPlayerState(PlayerState.Hanging);
        //_rBody2D.isKinematic = true;
        //_rBody2D.velocity = Vector3.zero;

        _rBody2D.isKinematic = false;
        _joint2D.enabled = true;
    }

    private void EndHanging(PlayerState newState)
    {
        SetPlayerState(newState);
        _rBody2D.isKinematic = false;
        _joint2D.enabled = false;
    }

    private void SetPlayerState(PlayerState newState)
    {
        _state = newState;
    }

    /// <summary>
    /// Update player's position and handle animations.
    /// </summary>
    /// <param name="movement">Movement added to the player's position</param>
    public void Move(Vector3 movement, bool isDashing = false)
    {
        if (movement != Vector3.zero)
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

    public void StartAiming()
    {
        // Update player's state
        SetAimingState(true);

        // Time is slowed down while the player aims
        _gameManager.UpdateTimeScale(_timeSlowRatio);
    }

    public void OnAimingEnd()
    {
        // Set player's state
        SetAimingState(false);
        ShowAimingArrow(false);

        // Set back to normal time scale
        _gameManager.UpdateTimeScale(GameManager.BASE_TIMESCALE);
    }

    public void ApplyAimRotation(Vector3 angle)
    {
        ShowAimingArrow(true);
        _aimArrowRenderer.transform.eulerAngles = angle;
    }

    public void ShowAimingArrow(bool show)
    {
        _aimArrowRenderer.enabled = show;
    }

    public void ThrowArtefact(Vector3 direction)
    {
        if (_state == PlayerState.Hanging)
            EndHanging(PlayerState.Free);

        _artefact.StartSpawnCoroutine(direction.normalized * _artefactOffset, direction);
    }

    public void TeleportToArtefact()
    {
        // Update state
        SetPlayerState(PlayerState.Tweening);

        // Reset Velocity (cancel gravity if teleported while in the air)
        _rBody2D.velocity = Vector3.zero;

        // Get tweaked position from the artefact and teleport to it
        Vector2 dest = _artefact.GetTPPosition(HorizontalExtent, VerticalExtent, HorizontalExtent, VerticalExtent);

        // Compute the tweening duration
        float duration = _baseTPTweeningDuration + Vector3.Distance(transform.position, _artefact.transform.position) * _distanceTweeningDurationRatio;

        transform.DOMove(dest, duration, true).SetEase(Ease.InBack).OnComplete(OnTeleportComplete);
        ShowTweenRenderer(true);

        _artefact.TryFreeze();
        //Debug.Log("Dist = " + Vector3.Distance(transform.position, _artefact.transform.position) + "   total time : " + duration);
    }

    public void Recall()
    {
        _artefact.Recall();
    }

    public float ComputeTweenDuration(float distance)
    {
        float duration = _baseTPTweeningDuration + distance * _distanceTweeningDurationRatio;

        return duration;
    }

    public Vector3 GetPlayerCenter()
    {
        return _spriteRenderer.sprite.bounds.center + transform.position;
    }

    #region Input Booleans
    public bool CanMove()
    {
        if (_isAiming)
            return false;

        switch (_state)
        {
            case PlayerState.Free:
                return true;

            default:
                return false;
        }
    }

    public bool CanAim()
    {
        switch (_state)
        {
            case PlayerState.Free:
                if (_artefact.IsWithPlayer)
                    return true;
                else
                    return false;

            case PlayerState.Hanging:
                return true;

            default:
                return false;
        }
    }

    public bool CanRecall()
    {
        // Check Artefact
        if (_artefact.CanRecall() == false)
        {
            return false;
        }

        // Check Player state
        switch(_state)
        {
            case PlayerState.Free:
                return true;

            default:
                return false;
        }
    }
    #endregion
    #endregion
}