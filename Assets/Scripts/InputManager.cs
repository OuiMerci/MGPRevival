using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    static private InputManager _instance = null;

    [SerializeField] private float _timeSlowRatio = 1;

    private const int MAX_RAYCAST_DISTANCE = 25;
    private GameManager _gameManager = null;
    private PlayerBehaviour _player = null;
    private WandBehaviour _wand = null;
    private int _backgroundLayerMask = 0;
    private bool _teleportAsked = false;
    #endregion Fields

    #region Properties
    static public InputManager Instance
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
        _player = PlayerBehaviour.Instance;
        _wand = WandBehaviour.Instance;
    }
	
	// Update is called once per frame
	void Update () {

        //Check aiming state
        if(Input.GetButtonDown("Aim"))
        {
            if(_wand.IsWithPlayer)
            {
                StartAiming();
            }
            else if(_wand.CanTeleport)
            {
                _teleportAsked = true;
            }
        }
        else if (Input.GetButtonUp("Aim"))
        {
            if(_player.IsAiming)
            {
                OnAimingEnd();

                Vector3 aim = GetAimInput();
                _player.ThrowWand(aim);
            }
        }

        if (_player.IsAiming)
        {
            if(Input.GetButtonDown("AimCancel"))
            {
                OnAimingEnd();
                _wand.SetVisible(false);
            }
            else
            {
                ApplyAimingInput();
            }
        }
    }

    // Operations implying movements / physics
    private void FixedUpdate()
    {
        if (_player.IsAiming == false)
        {
            ApplyMovementInput();
        }

        if (_teleportAsked == true)
        {
            _teleportAsked = false;
            _player.TeleportToWand();
        }
    }

    void ApplyMovementInput()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        bool isDashing = Input.GetAxis("Dash") < - 0.8f;

        _player.Move(new Vector3(HorizontalInput, 0, 0), isDashing);
    }

    void ApplyAimingInput()
    {
        Vector2 aim = GetAimInput();

        //Debug.Log(" axis = " + aimH + "    " + aimV + "  get  : " + Input.GetAxisRaw("AimH") + "     " + Input.GetAxisRaw("AimV"));

        if (aim.x != 0.0 || aim.y != 0.0)
        {
            _player.ShowAimingArrow(true);

            float zRot = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
            _player.ApplyAimRotation(new Vector3(0, 0, zRot));
        }
        else
        {
            _player.ShowAimingArrow(false);
        }
    }

    private Vector2 GetAimInput()
    {
        float aimH = Input.GetAxis("Horizontal");
        float aimV = Input.GetAxis("Vertical") * -1; //Reverse the Y axis for more intuitive movement (= normal)

        return new Vector2(aimH, aimV);
    }

    void StartAiming()
    {
        // Update player's state
        _player.SetAimingState(true);

        // Time is slowed down while the player aims
        Time.timeScale = _timeSlowRatio;
    }

    void OnAimingEnd()
    {
        //Debug.Log("On aiming end !!");

        // Set player's state
        _player.SetAimingState(false);
        _player.ShowAimingArrow(false);

        // Set back to normal time scale
        Time.timeScale = 1;
    }
    #endregion
}