using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    static private InputManager _instance = null;

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
        ApplyMovementInput();
	}

    void ApplyMovementInput()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        bool isDashing = Input.GetButton("Dash");
        _player.Move(new Vector3(HorizontalInput, 0, 0), isDashing);
    }
    #endregion
}