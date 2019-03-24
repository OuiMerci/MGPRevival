using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    [SerializeField] private float _minPressLongAttack;
    [SerializeField] private float _minStickInput;
    [SerializeField] private float _minTriggerInput;
    [SerializeField] private float _aimClampOffset; // when clamping, an offset is used -> ex : max = (90° - offset)

    public enum ClampedAimEnum
    {
        none, top, left, right, topLeft, topRight
    }
    static private ClampedAimEnum _clampedAim = ClampedAimEnum.none;

    static private InputManager _instance = null;
    private GameManager _gManager = null;
    private const int MAX_RAYCAST_DISTANCE = 25;
    private PlayerBehaviour _player = null;
    private ArtefactBehaviour _artefact = null;
    private bool _teleportAsked = false;
    private float _lastValidZRot = 0;
    private double _startAttackPressTime = 0;
    private bool _aimCanceled;

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
        _gManager = GameManager.Instance;

        //Debug.Log("Connected Gamepads");
        //for(int i=0; i < Input.GetJoystickNames().Length; i++)
        //{
        //    Debug.Log(Input.GetJoystickNames()[i]);
        //}
    }
	
	// Update is called once per frame
	void Update () {

        switch(_gManager.CurrentState)
        {
            case GameManager.Gamestate.playing:
                TestComboInput();
                TestAimingInput();
                TestRecallInput();
                TestMagnetInput();
                TestDashInput();

                // test jump input only if the jump button isn't used to cancel an aim
                if (_aimCanceled == false)
                    TestJumpInput();

                // reset boolean
                _aimCanceled = false;
                break;

            case GameManager.Gamestate.song:

                if (Input.GetButtonDown("SongStarter"))
                {
                    SongManager.EndSongGameplay();
                }
                break;

            case GameManager.Gamestate.pause:
                break;
        }
    }

    // Operations implying movements / physics
    private void FixedUpdate()
    {
        switch (_gManager.CurrentState)
        {
            case GameManager.Gamestate.playing:
                if (_teleportAsked == true)
                {
                    _teleportAsked = false;
                    _player.TeleportToArtefact();
                }

                if (_player.CanMove())
                {
                    ApplyMovementInput();
                }
                break;

            case GameManager.Gamestate.song:
                break;

            case GameManager.Gamestate.pause:
                break;
        }
    }

    void TestComboInput()
    {
        if (_player.CanMove() == false)
            return;

        ComboManager.Inputs input;

        if (Input.GetButtonDown("SongStarter"))
        {
            input = ComboManager.Inputs.SongStart;
            _player.Combo.HandleInput(input);
        }
        else if(Input.GetButtonUp("Attack"))
        {
            float vAxis = Input.GetAxis("Vertical");

            if (_startAttackPressTime + _minPressLongAttack < Time.time)
            {
                input = ComboManager.Inputs.LongAttack;
            }
            else if (vAxis > _minStickInput)
            {
                input = ComboManager.Inputs.UpAttack;
            }
            else if(vAxis <  -_minStickInput)
            {
                input = ComboManager.Inputs.DownAttack;
            }
            else
            {
                input = ComboManager.Inputs.Neutral;
            }

            _player.Combo.HandleInput(input);
        }
        else if(Input.GetButtonDown("Attack"))
        {
            _startAttackPressTime = Time.time;
        }
    }

    void ApplyMovementInput()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");

        if(usingSwitchPad)
        {
            HorizontalInput = Input.GetAxis("SwitchH");
        }

        // Overwrite deadzone for movement
        if (Mathf.Abs(HorizontalInput) < _minStickInput)
            HorizontalInput = 0;

        _player.Move(new Vector3(HorizontalInput, 0, 0), IsRunning());
    }

    private bool IsRunning()
    {
        return IsLeftTriggerPressed() || Input.GetButtonDown("Run");
    }

    void ApplyAimingInput()
    {
        Vector2 aim = GetAimInput();

        if (aim.x != 0.0 || aim.y != 0.0)
        {
            _player.ShowAimingArrow(true);

            float zRot = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
            AimClamper(ref zRot);

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
            if (_player.CanAim())
            {
                _lastValidZRot = 0; // initialize last valid input
                _player.StartAiming();
            }
            else if (_artefact.ReadyForInteraction())
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
                _aimCanceled = true;
            }
            else
            {
                ApplyAimingInput();
            }
        }
    }
    // TODO : Add some code to clamp the aiming (when hanging, can't aim toward the wall)
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
        if(Input.GetButtonDown("Recall") && _player.CanRecall())
        {
            _player.Recall();
        }
    }

    private void TestMagnetInput()
    {
        if(Input.GetButtonDown("Magnet"))
        {
            _artefact.StartBeMagnet();
        }
        else if (Input.GetButtonUp("Magnet"))
        {
            _artefact.StopBeMagnet();
        }
    }

    private void TestDashInput()
    {
        bool dashInput = IsRightTriggerPressed() || Input.GetButtonDown("Dash");

        if (dashInput && _player.CanDash())
        {
            _player.Dash();
        }
    }

    static public void SetClampedAim(ClampedAimEnum clampedAim)
    {
        _clampedAim = clampedAim;
    }

    private void AimClamper(ref float zRot)
    {
        switch (_clampedAim)
        {
            case ClampedAimEnum.left:
                if (zRot > (90 - _aimClampOffset) || zRot < (-90 + _aimClampOffset))
                {
                    zRot = _lastValidZRot;
                }
                else
                {
                    _lastValidZRot = zRot;
                }
                break;
            case ClampedAimEnum.right:
                if (_lastValidZRot == 0)
                {
                    _lastValidZRot = 180; // initialise zRot
                }

                if (zRot < (90 + _aimClampOffset)  && zRot > (-90 - _aimClampOffset))
                {
                    zRot = _lastValidZRot;
                }
                else
                {
                    _lastValidZRot = zRot;
                }
                break;
            case ClampedAimEnum.top:
                if (_lastValidZRot == 0)
                {
                    _lastValidZRot = -90; // initialise zRot
                }

                if (zRot > (0 - _aimClampOffset) || zRot < (-180 +_aimClampOffset))
                {
                    zRot = _lastValidZRot;
                }
                else
                {
                    _lastValidZRot = zRot;
                }
                break;
            case ClampedAimEnum.topLeft:
                if (_lastValidZRot == 0)
                {
                    _lastValidZRot = -45; // initialise zRot
                }

                if (zRot < (-90 + _aimClampOffset) || zRot > (0 - _aimClampOffset))
                {
                    zRot = _lastValidZRot;
                }
                else
                {
                    _lastValidZRot = zRot;
                }
                break;
            case ClampedAimEnum.topRight:
                if (_lastValidZRot == 0)
                {
                    _lastValidZRot = -135; // initialise zRot
                }

                if (zRot < (-180 - _aimClampOffset) || zRot > (-90 -_aimClampOffset))
                {
                    zRot = _lastValidZRot;
                }
                else
                {
                    _lastValidZRot = zRot;
                }
                break;
        }
    }

    private void TestJumpInput()
    {
        if(Input.GetButtonDown("Jump"))
        {
            _player.Jump();
        }
    }

    private bool IsRightTriggerPressed()
    {
        return Input.GetAxis("RightTrigger") > _minTriggerInput;
    }

    private bool IsLeftTriggerPressed()
    {
        return Input.GetAxis("LeftTrigger") < - _minTriggerInput;
    }
    #endregion
}