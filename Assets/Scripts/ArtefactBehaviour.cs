using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArtefactBehaviour : MonoBehaviour {

    #region Fields
    static private ArtefactBehaviour _instance = null;
    public enum ArtefactState
    {
        Unknown, // This is an empty state the artefact should never be in
        WithPlayer,
        Free,
        Tweening,
        Magnetised
    }
    public ArtefactState _state;

    [SerializeField] private float _force = 0;
    [SerializeField] private float _firstRotForce = 0;
    [SerializeField] private float _delayBeforeTP = 0;
    [SerializeField] private float _magnetSnapDistance = 0;
    [SerializeField] private float _magnetSnapDuration = 0;
    [SerializeField] private GameObject _tweeningSprite = null;

    private SpriteRenderer _spriteRenderer = null;
    private PlayerBehaviour _player = null;
    public Rigidbody2D _rBody2D = null;
    private Collider2D _coll2D = null;
    private Joint2D _joint = null;
    //private Rigidbody2D _attachedRB;
    private MovingPlatform _attachedPlatform;

    // Is it worth it to add a state machine ?
    private bool _canTeleport = false;
    private bool _isMagnet = false;
    private bool _snapping = false;
    private bool _glued = false;
    private Tweener _snapTween;
    public Collider2D _magPreviousH; // stores the previous H wall it was magnetised to, snap will be ignored for it until it leaves range
    public Collider2D _magPreviousV; // stores the previous V wall it was magnetised to, snap will be ignored for it until it leaves range

    #endregion

    #region Properties
    static public ArtefactBehaviour Instance
    {
        get { return _instance; }
    }

    //public bool CanTeleport
    //{
    //    get { return _canTeleport; }
    //}

    public float VerticalExtent
    {
        get { return _coll2D.bounds.extents.y; }
    }

    public float HorizontalExtent
    {
        get { return _coll2D.bounds.extents.x; }
    }

    public bool IsWithPlayer
    {
        get { return _state == ArtefactState.WithPlayer; }
    }

    public bool IsMagnetised
    {
        get { return _state == ArtefactState.Magnetised; }
    }

    public bool IsTweening
    {
        get { return _state == ArtefactState.Tweening; }
    }

    public bool IsSnapping
    {
        get { return _snapping; }
    }

    public bool IsGlued
    {
        get { return _glued; }
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
        _joint = GetComponent<Joint2D>();

        //Physics2D.IgnoreCollision(_coll2D, _player.Coll2D, true);
        SetArtifactActive(false);
        SetArtefactState(ArtefactState.WithPlayer);
    }

    // A RETRAVAILLET POUR LE REGLER CONFLIT MAGNET / SNAP / RECALL
    private void Update()
    {
        //// AJOUTER UN CHECK DE DISTANCE ENTRE ART ET MC -> METTRE FIN AU HANGING MAIS LAISSER LE BATON MAGNETISED

        CheckSnap();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log( collision.otherCollider.name + " colliding with : " + collision.collider.name + "  with tag : " + collision.collider.tag + "  Object tag = " + collision.gameObject.tag);
        if(collision.gameObject.CompareTag("ArtGlue"))
        {
            StartGlue(collision.transform.GetComponent<MovingPlatform>());
        }
    }

    private IEnumerator Spawn(Vector3 pos, Vector3 direction)
    {
        // BLINDAGE -> No more joint
        _joint.enabled = false;

        // BLINDAGE -> Reset RBody velocity
        _rBody2D.isKinematic = false;
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Initiate position
        transform.position = pos + _player.GetCharacterCenter();

        _rBody2D.AddForce(direction * _force, ForceMode2D.Impulse);
        _rBody2D.AddTorque(_firstRotForce, ForceMode2D.Impulse);
        SetArtefactState(ArtefactState.Free);

        yield return new WaitForSeconds(_delayBeforeTP);
        _canTeleport = true;
    }

    private void CheckSnap()
    {
        //if (_isMagnet && IsMagnetised == false)
        //{
        //    if (_snapping == false)
        //    {
        //        Vector3 dest = GetSnapPosition(HorizontalExtent, VerticalExtent, _magnetSnapDistance, _magnetSnapDistance, out _attachedRB,GameManager.magnetLayer);
        //        if (dest != transform.position)
        //        {
        //            _snapping = true;
        //            _rBody2D.velocity = Vector3.zero;
        //            SetKinematic(true);
        //            _snapTween = transform.DOMove(dest, _magnetSnapDuration, true).SetEase(Ease.Linear).OnComplete(StartMagnetised);
        //        }
        //    }
        //}

        if (_isMagnet && IsMagnetised == false)
        {
            if (_snapping == false)
            {
                GameObject platform;
                Vector3 dest = GetSnapPosition(HorizontalExtent, VerticalExtent, _magnetSnapDistance, _magnetSnapDistance, out platform, GameManager.magnetLayer);
                if (dest != transform.position)
                {
                    _snapping = true;
                    _rBody2D.velocity = Vector3.zero;
                    SetKinematic(true);
                    _snapTween = transform.DOMove(dest, _magnetSnapDuration, true).SetEase(Ease.Linear).OnComplete(() => StartMagnetised(platform.GetComponent<MovingPlatform>()));
                }
            }
        }
    }

    private void SetKinematic(bool newStatus, bool resetVelocity = true)
    {
        if(resetVelocity)
        {
            _rBody2D.velocity = Vector3.zero;
            _rBody2D.angularVelocity = 0;
        }

        _rBody2D.isKinematic = newStatus;
    }

    private void ShowTweenRenderer(bool show)
    {
        _spriteRenderer.enabled = !show;
        _tweeningSprite.SetActive(show);
    }

    private void OnRecallComplete()
    {
        // Reset Parameters
        ResetArtifact();

        // Show artifact sprite
        ShowTweenRenderer(false);
    }

    private void StartGlue(MovingPlatform mp)
    {
        Debug.Log("Start glue !");
        _glued = true;
        ConnectPlatform(mp);
    }

    private void StartMagnetised(MovingPlatform mp)
    {
        //_snapping = false;
        //SetKinematic(false);
        //ConnectRigidBody(_attachedRB);
        //SetArtefactState(ArtefactState.Magnetised);

        _snapping = false;
        SetKinematic(true);
        ConnectPlatform(mp);
        SetArtefactState(ArtefactState.Magnetised);
    }

    private void EndMagnetised(ArtefactState newState)
    {
        // If the magnet button is released while the player is tweening, the artefact must stay kinematic
        if(_player.IsTweening == false)
            SetKinematic(false);

        DisconnectPlatform();
        SetArtefactState(newState);
    }

    private void ConnectPlatform(MovingPlatform mp)
    {
        //_joint.connectedBody = rb;
        //_joint.enabled = true;

        _attachedPlatform = mp;

        if (_attachedPlatform != null)
        {
            _attachedPlatform.UpdateArtefact = true;
        }
        else
        {
            Debug.LogWarning("Trying to connect platform, but there is none !!");
        }
    }

    private void DisconnectPlatform()
    {
        //_joint.connectedBody = null;
        //_joint.enabled = false;

        if (_attachedPlatform)
        {
            _attachedPlatform.UpdateArtefact = false;
        }
        else
        {
            Debug.LogWarning("Trying to disconnect platform, but there is none !!");
        }

        _attachedPlatform = null;
    }

    private void SetArtefactState(ArtefactState newState)
    {
        _state = newState;
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

    public void ResetArtifact()
    {
        // Reset RBody velocity
        _rBody2D.velocity = Vector2.zero;
        _rBody2D.angularVelocity = 0;

        // Reset kinematic settings
        _rBody2D.isKinematic = false;

        // Hide the artefact
        SetArtifactActive(false);

        // Is with Player
        SetArtefactState(ArtefactState.WithPlayer);
        _canTeleport = false;

        // The artefact isn't glued anymore
        _glued = false;

        // Reset Player's aim clamp
        InputManager.SetClampedAim(InputManager.ClampedAimEnum.none);
    }

    /// <summary>
    /// Raycasts around the artefact to check if a wall is close and gets the closest valid position.
    /// </summary>
    /// <param name="hExtent">Horizontal Extent of the object we want to TP</param>
    /// <param name="vExtent">Vertical Extent of the object we want to TP</param>
    /// <param name="hMaxDistance">Max distance for horizontal raycasts</param>
    /// <param name="vMaxDistance">Max distance for vertical raycasts</param>
    /// <returns>Returns the valid position that is the closest to the wall</returns>
    public Vector2 GetTPPosition(float hExtent, float vExtent, float hMaxDistance, float vMaxDistance, int mask = GameManager.WALLS_AND_MAGNETS_LAYERMASK)
    {
        RaycastHit2D hit;
        bool useTweakedX = false;
        bool useTweakedY = false;
        float tweakedX = 0;
        float tweakedY = 0;

        //If the sprite pivot is not in the center, but at the bottom, we need some tweaking.
        float spritePivotFix = vExtent;

        //raycast down
        hit = Physics2D.Raycast(transform.position, Vector2.down, vMaxDistance, mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * vMaxDistance), Color.green);
        if (hit.collider != null)
        {
            useTweakedY = true;
            tweakedY = hit.point.y + vExtent - spritePivotFix;
            //Debug.Log("Colliding bot with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);
        }
        else
        {
            //raycast up
            hit = Physics2D.Raycast(transform.position, Vector2.up, vMaxDistance, mask);
            Debug.DrawLine(transform.position, transform.position + (Vector3.up * vMaxDistance), Color.blue);
            if (hit.collider != null)
            {
                useTweakedY = true;
                tweakedY = hit.point.y - vExtent - spritePivotFix;
                //Debug.Log("Colliding up with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);
            }
        }

        //raycast left
        hit = Physics2D.Raycast(transform.position, Vector2.left, hMaxDistance, mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3.left * hMaxDistance), Color.red);
        if (hit.collider != null)
        {
            useTweakedX = true;
            tweakedX = hit.point.x + hExtent;
            //Debug.Log("Colliding left with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);
        }
        else
        {
            //raycast right
            hit = Physics2D.Raycast(transform.position, Vector2.right, hMaxDistance, mask);
            Debug.DrawLine(transform.position, transform.position + (Vector3.right * hMaxDistance), Color.yellow);
            if (hit.collider != null)
            {
                useTweakedX = true;
                tweakedX = hit.point.x - hExtent;
                //Debug.Log("Colliding right with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);
            }
        }

        //Set and return tweaked position
        Vector2 tweakedPosition = transform.position;

        if (useTweakedX && useTweakedY)
        {
            tweakedPosition = new Vector2(tweakedX, tweakedY);
            //Debug.Log("Used Tweaked X AND Y");
        }
        else if (useTweakedX)
        {
            tweakedPosition = new Vector2(tweakedX, transform.position.y);
            //Debug.Log("Used Tweaked X")

        }
        else if(useTweakedY)
        {
            tweakedPosition = new Vector2(transform.position.x, tweakedY);
            //Debug.Log("Used Tweaked Y");
        }

        return tweakedPosition;
    }

    public Vector2 GetSnapPosition(float hExtent, float vExtent, float hMaxDistance, float vMaxDistance, out GameObject otherRB, int mask = GameManager.WALLS_AND_MAGNETS_LAYERMASK)
    {
        RaycastHit2D hit;
        otherRB = null;
        bool useTweakedX = false;
        bool useTweakedY = false;
        float tweakedX = 0;
        float tweakedY = 0;
        bool topClamped = false;

        //If the sprite pivot is not in the center, but at the bottom, we need some tweaking.
        float spritePivotFix = vExtent;

        //raycast down
        hit = Physics2D.Raycast(transform.position, Vector2.down, vMaxDistance, mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * vMaxDistance), Color.green);
        if (hit.collider != null)
        {
            useTweakedY = true;
            tweakedY = hit.point.y + vExtent - spritePivotFix;
            //Debug.Log("Colliding bot with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);

            if (hit.collider == _magPreviousV) // if we hit the same wall as the previous one, ignore it
                useTweakedY = false;
            else
                _magPreviousV = hit.collider; // if the wall isn't the same, update _magPreviousV

            // Try assign otherRB
            //otherRB = hit.transform.GetComponent<Rigidbody2D>();
            otherRB = hit.transform.gameObject;
        }
        else
        {
            //raycast up
            hit = Physics2D.Raycast(transform.position, Vector2.up, vMaxDistance, mask);
            Debug.DrawLine(transform.position, transform.position + (Vector3.up * vMaxDistance), Color.blue);
            if (hit.collider != null)
            {
                useTweakedY = true;
                tweakedY = hit.point.y - vExtent - spritePivotFix;
                //Debug.Log("Colliding up with : " + hit.collider.name + ", Tweaking Y to : " + tweakedY);

                if (hit.collider == _magPreviousV) // if we hit the same wall as the previous one, ignore it
                    useTweakedY = false;
                else
                    _magPreviousV = hit.collider; // if the wall isn't the same, update _magPreviousV

                // update player's aim clamp
                InputManager.SetClampedAim(InputManager.ClampedAimEnum.top);
                topClamped = true;

                // Try assign otherRB
                if(otherRB == null)
                    otherRB = hit.transform.gameObject;
                    //otherRB = hit.transform.GetComponent<Rigidbody2D>();
            }
            else
            {
                // Nothing hit ? Reset _magPreviousV
                _magPreviousV = null;
            }
        }

        //raycast left
        hit = Physics2D.Raycast(transform.position, Vector2.left, hMaxDistance, mask);
        Debug.DrawLine(transform.position, transform.position + (Vector3.left * hMaxDistance), Color.red);
        if (hit.collider != null)
        {
            useTweakedX = true;
            tweakedX = hit.point.x + hExtent;
            //Debug.Log("Colliding left with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);

            if (hit.collider == _magPreviousH) // if we hit the same wall as the previous one, ignore it
                useTweakedX = false;
            else
                _magPreviousH = hit.collider; // if the wall isn't the same, update _magPreviousH

            // Update player's aim clamp
            if (topClamped)
                InputManager.SetClampedAim(InputManager.ClampedAimEnum.topLeft);
            else
                InputManager.SetClampedAim(InputManager.ClampedAimEnum.left);

            // Try assign otherRB
            if (otherRB == null)
                otherRB = hit.transform.gameObject;
                //otherRB = hit.transform.GetComponent<Rigidbody2D>();
        }
        else
        {
            //raycast right
            hit = Physics2D.Raycast(transform.position, Vector2.right, hMaxDistance, mask);
            Debug.DrawLine(transform.position, transform.position + (Vector3.right * hMaxDistance), Color.yellow);
            if (hit.collider != null)
            {
                useTweakedX = true;
                tweakedX = hit.point.x - hExtent;
                //Debug.Log("Colliding right with : " + hit.collider.name + ", Tweaking X to : " + tweakedX);

                if (hit.collider == _magPreviousH) // if we hit the same wall as the previous one, ignore it
                    useTweakedX = false;
                else
                    _magPreviousH = hit.collider; // if the wall isn't the same, update _magPreviousH

                // Update player's aim clamp
                if (topClamped)
                    InputManager.SetClampedAim(InputManager.ClampedAimEnum.topRight);
                else
                    InputManager.SetClampedAim(InputManager.ClampedAimEnum.right);

                // Try assign otherRB
                if (otherRB == null)
                    otherRB = hit.transform.gameObject;
                    //otherRB = hit.transform.GetComponent<Rigidbody2D>();
            }
            else
            {
                // Nothing hit ? Reset _magPreviousV
                _magPreviousH = null;
            }
        }

        //Set and return tweaked position
        Vector2 tweakedPosition = transform.position;

        if (useTweakedX && useTweakedY)
        {
            tweakedPosition = new Vector2(tweakedX, tweakedY);
            //Debug.Log("Used Tweaked X AND Y");
        }
        else if (useTweakedX)
        {
            tweakedPosition = new Vector2(tweakedX, transform.position.y);
            //Debug.Log("Used Tweaked X");, n, nbbgvc

        }
        else if(useTweakedY)
        {
            tweakedPosition = new Vector2(transform.position.x, tweakedY);
            //Debug.Log("Used Tweaked Y");
        }

        //Debug.Log("Art. position : " + transform.position + " tweaked position : " + tweakedPosition);
        return tweakedPosition;
    }

    public void Recall()
    {
        if (_state == ArtefactState.Magnetised)
            EndMagnetised(ArtefactState.Tweening);

        // clear movement from rigidbody
        TryFreeze();

        // Get destination
        Vector2 dest = _player.GetCharacterCenter();
        // Debug.Log("dest = " + dest);

        // compute distance from player
        float distance = Vector3.Distance(transform.position, dest);

        // Get the duration of tweening
        float duration = _player.ComputeTweenDuration(distance);
        transform.DOMove(dest, duration, true).SetEase(Ease.InBack).OnComplete(OnRecallComplete);

        ShowTweenRenderer(true);

        // Update state
        SetArtefactState(ArtefactState.Tweening);
    }

    public void TryFreeze()
    {
        if(IsMagnetised || IsGlued)
        {
            Debug.Log("Artefact is already magnetised / frozen");
            return;
        }

        _rBody2D.velocity = Vector3.zero;
        _rBody2D.isKinematic = true;
    }

    public bool CanRecall()
    {
        // it may be necessary to add state tests eventually
        if(_canTeleport == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void StartBeMagnet()
    {
        _isMagnet = true;
    }

    public void StopBeMagnet()
    {
        _magPreviousH = null;
        _magPreviousV = null;

        if (_state == ArtefactState.Magnetised)
        {        _magPreviousH = null;

            if(_player.IsHanging)
            {
                EndMagnetised(ArtefactState.WithPlayer); // the artefact goes back to the player
                ResetArtifact();
            }
            else
            {
                EndMagnetised(ArtefactState.Free); // the artefact just falls from the wall
            }

            EventsManager.FireDemagnetisedEvent();
        }
        else if(_snapping) // If the player release the magnet button while snapping
        {
            _snapping = false;
            _snapTween.Kill();

            EndMagnetised(ArtefactState.Free);
        }

        _isMagnet = false;
        _snapping = false;
    }

    public bool ReadyForInteraction()
    {
        if (_canTeleport == false || IsTweening == true)
            return false;
        else
            return true;
    }
    #endregion
}
