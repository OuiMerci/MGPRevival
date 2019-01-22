using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour {

    #region fields
    [SerializeField] private float _timeSlowRatio = 1;
    [SerializeField] private SpriteRenderer _aimArrowRenderer = null;
    [SerializeField] private float _speed = 0.0f;
    [SerializeField] private float _runningSpeed = 0.0f;
    [SerializeField] private float _dashDistance = 0.0f;
    [SerializeField] private float _dashDuration = 0.0f;
    [SerializeField] private float _artefactOffset = 0.0f;
    [SerializeField] private float _baseTPTweeningDuration = 0.0f;
    [SerializeField] private float _distanceTweeningDurationRatio = 0.0f;
    [SerializeField] private GameObject _tweeningSprite = null;
    [SerializeField] private ComboManager _combo;
    [SerializeField] private float _jumpForce;

    // Cooldown
    [SerializeField] private CooldownHandler _dashCD = null;

    public enum PlayerState // set back to private once _state is hidden
    {
        Free,
        Hanging,
        Tweening,
        Dashing
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
    private int _walkHash = Animator.StringToHash("isWalking");
    private int _runHash = Animator.StringToHash("isRunning");
    private int _aimingHash = Animator.StringToHash("isAiming");
    private bool _isAiming = false;
    private Joint2D _joint2D = null;
    private float _baseGravity = 0;
    private bool _grounded;
    //private List<ForcedMovement> _forcedMovements;
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

    public int Orientation
    {
        get { return _spriteRenderer.flipX ? -1 : 1; }
    }

    public ComboManager Combo
    {
        get { return _combo; }
    }

    public bool Grounded
    {
        get { return _grounded; }
        set { _grounded = value; }
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
    void Start() {
        _gameManager = GameManager.Instance;
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _artefact = ArtefactBehaviour.Instance;
        _collider2D = GetComponent<Collider2D>();
        _rBody2D = GetComponent<Rigidbody2D>();
        _joint2D = GetComponent<Joint2D>();
        _baseGravity = _rBody2D.gravityScale;
        //_forcedMovements = new List<ForcedMovement>();

        _aimArrowRenderer.transform.position = GetPlayerCenter();
        ShowAimingArrow(false);
    }

    private void Update()
    {
        if(Input.GetButtonDown("Attack"))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
    }

    private void Jump()
    {
        _rBody2D.velocity = Vector2.zero;
        //AddForcedMovement(_jumpForce, Vector2.up, _jumpDuration);
        _rBody2D.AddForce(_jumpForce * Vector2.up, ForceMode2D.Impulse);
    }

    private void OnArtefactDemagnetised()
    {
        if (_state == PlayerState.Hanging)
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
        if (_artefact.IsMagnetised || _artefact.IsSnapping)
        {
            StartHanging();
        }
        else
        {
            SetPlayerState(PlayerState.Free); // update state
            _rBody2D.isKinematic = false; // set back kinematic

            // Make sure we get the latest position from the art
            transform.position = _artefact.GetTPPosition(HorizontalExtent, VerticalExtent, HorizontalExtent, VerticalExtent);
            _artefact.ResetArtifact();
        }
    }

    //private void AddForcedMovement(float force, Vector2 direction, float duration)
    //{
    //    ForcedMovement newMov = new ForcedMovement(force, direction, duration);
    //    _forcedMovements.Add(newMov);
    //    Debug.Log("Adding new FM : " + newMov + "   " + _forcedMovements.Count);
    //}

    //private Vector2 GetForcedMovementsSum()
    //{
    //    Vector2 totalMovement = Vector2.zero;
    //    ForcedMovement currentMovement = null;

    //    for(int i = 0; i < _forcedMovements.Count; i++)
    //    {
    //        currentMovement = _forcedMovements[i];

    //        if (currentMovement.IsAlive() == false)
    //        {
    //            _forcedMovements.RemoveAt(i);
    //        }
    //        else
    //        {
    //            totalMovement = currentMovement.AddMovement(totalMovement);
    //        }
    //    }

    //    return totalMovement;
    //}

    private void SetGravity(float newGrav)
    {
        _rBody2D.gravityScale = newGrav;
    }

    private void StartHanging()
    {
        SetPlayerState(PlayerState.Hanging);
        //_rBody2D.isKinematic = true;
        //_rBody2D.velocity = Vector3.zero;

        _rBody2D.isKinematic = false;
        _rBody2D.freezeRotation = false;
        _joint2D.enabled = true;
        _joint2D.connectedBody = _artefact._rBody2D;
    }

    private void EndHanging(PlayerState newState)
    {
        SetPlayerState(newState);
        _rBody2D.isKinematic = false;
        _rBody2D.freezeRotation = true;
        transform.eulerAngles = Vector3.zero;
        _joint2D.enabled = false;
    }

    private void SetPlayerState(PlayerState newState)
    {
        _state = newState;
    }

    public void Dash()
    {
        SetPlayerState(PlayerState.Dashing);
        _dashCD.StartCooldown();
        SetGravity(0); // Blindage : set gravity to 0

        Vector2 dest;
        float effectiveDuration = 0;
        Vector3 playerCenter = GetPlayerCenter();

        // Raycast in front of the player and check for obstacles
        RaycastHit2D hitH = Physics2D.Raycast(playerCenter, Vector2.right * Orientation, _dashDistance, GameManager.WALLS_AND_MAGNETS_LAYERMASK);
        if (hitH.collider != null)
        {
            if (hitH.collider.transform.eulerAngles.z !=0)
            {
                dest = hitH.point;
                Debug.DrawLine(playerCenter, hitH.point, Color.green, 1);
            }
            else
            {
                dest = new Vector3(hitH.point.x - Orientation * HorizontalExtent, transform.position.y);
                Debug.DrawLine(playerCenter, hitH.point, Color.cyan, 1);
            }

            // distance difference
            float diffX = Mathf.Abs(transform.position.x - dest.x);
            float ratio = diffX / _dashDistance;

            effectiveDuration = _dashDuration * ratio;

            Debug.DrawLine(playerCenter, dest, Color.red, 1);
        }
        else
        {
            // If no obstacle has been found, check for ground level difference
            Vector3 origin = playerCenter + new Vector3(_dashDistance * Orientation, 0, 0);
            RaycastHit2D hitV = Physics2D.Raycast(origin, Vector2.down, VerticalExtent, GameManager.WALLS_AND_MAGNETS_LAYERMASK);
            if (hitV.collider != null)
            {
                dest = hitV.point;
                Debug.DrawLine(origin, hitV.point, Color.green, 1);
            }
            else
            {
                Debug.DrawLine(playerCenter, origin, Color.green, 1);
                Debug.DrawLine(origin, origin + (Vector3.down * VerticalExtent), Color.red, 1);
                dest = new Vector3(transform.position.x + Orientation * _dashDistance, transform.position.y, transform.position.z);
            }

            effectiveDuration = _dashDuration;
        }

        //_rBody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        transform.DOMove(dest, effectiveDuration).SetUpdate(UpdateType.Fixed).OnComplete(OnDashComplete);
    }

    private void OnDashComplete()
    {
        SetPlayerState(PlayerState.Free);
        SetGravity(_baseGravity);
    }

    private void StartCooldown(ref float startTimeVariable)
    {
        startTimeVariable = Time.time;
    }

    /// <summary>
    /// Update player's position and handle animations.
    /// </summary>
    /// <param name="movement">Movement added to the player's position</param>
    public void Move(Vector3 movement, bool isRuning = false)
    {
        if (movement != Vector3.zero)
        {
            float actualSpeed = isRuning ? _runningSpeed : _speed;
            Vector3 newPos = transform.position + movement * actualSpeed * Time.deltaTime;
            transform.position = newPos; // Ajouter une fonction "ClampPosition" si nécessaire.

            //_rBody2D.velocity += (Vector2)movement * actualSpeed * Time.deltaTime;
            //Debug.Log("Veloc = " + movement * actualSpeed * Time.deltaTime);

            // Handle animations
            _anim.SetBool(_walkHash, true);
            _anim.SetBool(_runHash, isRuning);

            _spriteRenderer.flipX = movement.x < 0 ? true : false;
        }
        else
        {
            // Handle animations
            _anim.SetBool(_walkHash, false);
            _anim.SetBool(_runHash, false);
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
        _artefact.TryFreeze();

        // Get tweaked position from the artefact and teleport to it
        Vector2 dest = _artefact.GetTPPosition(HorizontalExtent, VerticalExtent, HorizontalExtent, VerticalExtent);

        // Compute the tweening duration
        float duration = _baseTPTweeningDuration + Vector3.Distance(transform.position, _artefact.transform.position) * _distanceTweeningDurationRatio;

        transform.DOMove(dest, duration, true).SetEase(Ease.InBack).OnComplete(OnTeleportComplete);
        ShowTweenRenderer(true);
        
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

    public bool CanDash()
    {
        switch (_state)
        {
            case PlayerState.Free:
                if (_dashCD.Available)
                    return true;
                else
                    return false;

            default:
                Debug.Log("Cannot Dash");
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