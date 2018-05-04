using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    #region fields
    [SerializeField] private float _speed = 0.0f;
    [SerializeField] private float _dashingSpeed = 0.0f;

    static private PlayerBehaviour _instance = null;
    private Animator _anim = null;
    private SpriteRenderer _spriteRenderer = null;
    int _runHash = Animator.StringToHash("isRunning");
    int _dashHash = Animator.StringToHash("isDashing");
    #endregion

    #region Properties
    static public PlayerBehaviour Instance
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
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

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
    #endregion
}