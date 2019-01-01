using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    [SerializeField] private float _minHInput = 0;

    static private InputManager _instance = null;
    private const int MAX_RAYCAST_DISTANCE = 25;
    private GameManager _gameManager = null;
    private PlayerBehaviour _player = null;
    private ArtefactBehaviour _artefact = null;
    private int _backgroundLayerMask = 0;
    private bool _teleportAsked = false;
    private bool _recallAsked = false;

    public bool usingSwitchPad;
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
        _artefact = ArtefactBehaviour.Instance;

        Debug.Log("Connected Gamepads");
        for(int i=0; i < Input.GetJoystickNames().Length; i++)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(_player.CanAim())
        {
            TestAimingInput();
        }

        if(_player.CanRecall())
        {
            TestRecallInput();
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
            _player.TeleportToArtefact();
        }
    }

    void ApplyMovementInput()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        bool isDashing = Input.GetAxis("Dash") < - 0.8f;

        if(usingSwitchPad)
        {
            HorizontalInput = Input.GetAxis("SwitchH");
        }

        // Overwrite deadzone for movement
        if (Mathf.Abs(HorizontalInput) < _minHInput)
            HorizontalInput = 0;

        _player.Move(new Vector3(HorizontalInput, 0, 0), isDashing);
    }

    void ApplyAimingInput()
    {
        Vector2 aim = GetAimInput();

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

    private void TestAimingInput()
    {
        //Check aiming state
        if (Input.GetButtonDown("Aim"))
        {
            if (_artefact.IsWithPlayer)
            {
                _player.StartAiming();
            }
            else if (_artefact.CanTeleport)
            {
                _teleportAsked = true;
            }
        }
        else if (Input.GetButtonUp("Aim"))
        {
            if (_player.IsAiming)
            {
                _player.OnAimingEnd();

                Vector3 aim = GetAimInput();
                _player.ThrowArtefact(aim);
            }
        }

        if (_player.IsAiming)
        {
            if (Input.GetButtonDown("AimCancel"))
            {
                _player.OnAimingEnd();
            }
            else
            {
                ApplyAimingInput();
            }
        }
    }

    private Vector2 GetAimInput()
    {
        float aimH = Input.GetAxis("Horizontal");
        float aimV = Input.GetAxis("Vertical"); //Reverse the Y axis for more intuitive movement (= normal)

        if(usingSwitchPad)
        {
            aimH = Input.GetAxis("SwitchH");
            aimV = Input.GetAxis("SwitchV");
        }

        //Debug.Log("  get H : " + aimH + "    V " + aimV);

        return new Vector2(aimH, aimV);
    }

    private void TestRecallInput()
    {
        if(Input.GetButton("Recall"))
        {
            _player.Recall();
        }
    }
    #endregion
}