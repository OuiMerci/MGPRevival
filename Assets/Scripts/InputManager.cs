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
    private int _backgroundLayerMask = 0;
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
	}
	
	// Update is called once per frame
	void Update () {

        //Check aiming state
        if(Input.GetButtonDown("Aim"))
        {
            // Update player's state
            _player.SetAimingState(true);

            // Time is slowed down while the player aims
            Time.timeScale = _timeSlowRatio;
        }
        else if (Input.GetButtonUp("Aim"))
        {
            if(_player.IsAiming)
            {
                OnAimingEnd();
                // Do Stick throw here ?
            }
        }


        if (_player.IsAiming)
        {
            if(Input.GetButtonDown("AimCancel"))
            {
                OnAimingEnd();
            }
            else
            {
                ApplyAimingInput();
            }
        }
        else
        {
            ApplyMovementInput();
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
        float aimH = Input.GetAxis("Horizontal");
        float aimV = Input.GetAxis("Vertical") * -1; //Reverse the Y axis for more intuitive movement (= normal)

        //Debug.Log(" axis = " + aimH + "    " + aimV + "  get  : " + Input.GetAxisRaw("AimH") + "     " + Input.GetAxisRaw("AimV"));

        if (aimH != 0.0 || aimV != 0.0)
        {
            _player.ShowAimingArrow(true);

            float zRot = Mathf.Atan2(aimV, aimH) * Mathf.Rad2Deg;
            _player.ApplyAimRotation(new Vector3(0, 0, zRot));
            _player.ThrowWand(new Vector3(aimH, aimV, 0));

        }
        else
        {
            _player.ShowAimingArrow(false);
        }
    }

    void OnAimingEnd()
    {
        Debug.Log("On aiming end !!");

        // Set player's state
        _player.SetAimingState(false);
        _player.ShowAimingArrow(false);

        // Set back to normal time scale
        Time.timeScale = 1;
    }
    #endregion
}