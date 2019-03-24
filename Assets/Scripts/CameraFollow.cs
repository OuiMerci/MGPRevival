using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Fields
    [Header("Player Follow")]
    [SerializeField] private float _zOffsetExploration;
    [SerializeField] private float _levelZ;
    [SerializeField] private float _smoothDuration;
    [SerializeField] private Transform _topBound;
    [SerializeField] private Transform _botBound;
    [SerializeField] private Transform _leftBound;
    [SerializeField] private Transform _rightBound;

    [Header("Song Gameplay")]
    [SerializeField] private float _zOffsetSong;
    [SerializeField] private float _zoomDuration;
    [SerializeField] private float _zoomStopDistance;
    private Vector3 _positionBackup; // The camera position before the song started

    enum Action
    {
        Following,
        ZoomIn,
        SongIdle,
        ZoomOut
    }
    private Action _action;

    private Transform _player;
    private Vector3 _velocity;
    private Camera _cam;

    private Vector2 _camSize;
    private float _maxY;
    private float _minY;
    private float _maxX;
    private float _minX;

    // Zoom variables
    private float _currentZ;
    private Vector3 _zoomTarget;

    #endregion

    #region Properties
    public Transform TopBound
    {
        get { return _topBound; }
        set
        {
            _topBound = value;
            UpdateMaxY();
        }
    }

    public Transform BotBound
    {
        get { return _botBound; }
        set
        {
            _botBound = value;
            UpdateMinY();
        }
    }

    public Transform LeftBound
    {
        get { return _leftBound; }
        set
        {
            _leftBound = value;
            UpdateMinX();
        }
    }

    public Transform RightBound
    {
        get { return _rightBound; }
        set
        {
            _rightBound = value;
            UpdateMaxX();
        }
    }
    #endregion Properties

    #region Methods
    private void OnDrawGizmos()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = PlayerBehaviour.Instance.transform;
        _cam = GetComponent<Camera>();

        UpdateCamSizeInfo();
        UpdateMaxY();
        UpdateMinY();
        UpdateMaxX();
        UpdateMinX();

        FollowPlayerInstant();
    }

    private void OnEnable()
    {
        EventsManager.OnSongStart += OnSongStart;
        EventsManager.OnSongGameplayEnd += OnSongGameplayEnd;
        EventsManager.OnSongEnd += OnSongEnd;
    }

    private void OnDisable()
    {
        EventsManager.OnSongStart -= OnSongStart;
        EventsManager.OnSongGameplayEnd -= OnSongGameplayEnd;
        EventsManager.OnSongEnd -= OnSongEnd;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(_action)
        {
            case Action.Following:
                UpdateMaxY();
                UpdateMinY();
                UpdateMaxX();
                UpdateMinX();

                FollowPlayer();
                break;

            case Action.ZoomIn:
            case Action.ZoomOut:
                UpdateCameraZoom();
                break;

            case Action.SongIdle:
                break;
        }
    }

    
    ///// Follow Methods
    private void FollowPlayer()
    {
        transform.position = Vector3.SmoothDamp(transform.position, GetDesiredPosition(), ref _velocity, _smoothDuration); //apply smooth follow
    }

    private Vector3 GetDesiredPosition()
    {
        Vector3 desiredPos = ClampPosition(_player.position); //clamp the position
        desiredPos = desiredPos + new Vector3(0, 0, _zOffsetExploration); // add the camera zOffset

        return desiredPos;
    }

    private void FollowPlayerInstant()
    {
        Vector3 desiredPos = ClampPosition(_player.position); //clamp the position
        desiredPos = desiredPos + new Vector3(0, 0, _zOffsetExploration); // add the camera zOffset

        transform.position = desiredPos; // Apply position without smooth
    }
    ////

    //// Song Methods
    private void OnSongStart(Enemy enemy)
    {
        // Teleport the camera to its new position
        Vector3 camDest = SongManager.SongZoneCenter.position + new Vector3(0, 0, _zOffsetExploration);
        transform.position = camDest;

        // Set the zoomTarget for the song gameplay
        _zoomTarget = new Vector3(transform.position.x, transform.position.y, _zOffsetSong);

        // Set the new Action
        _action = Action.ZoomIn;
    }
    
    private void OnSongGameplayEnd()
    {
        // Set the new zoom target
        _zoomTarget = new Vector3(transform.position.x, transform.position.y, _zOffsetExploration);

        // Set the new Action
        _action = Action.ZoomOut;
    }

    private void OnSongEnd()
    {
        // Set the camera position to its position before the song started
        Vector3 aimedPos = ClampPosition(SongActorsMover._playerPositionBackup);
        aimedPos = new Vector3(aimedPos.x, aimedPos.y, _zOffsetExploration);
        transform.position = aimedPos;

        // Set the new action
        _action = Action.Following;
    }

    private void UpdateCameraZoom()
    {
        // Move the Camera
        transform.position = Vector3.SmoothDamp(transform.position, _zoomTarget, ref _velocity, _zoomDuration); //apply smooth follow

        // Check if destination has been reached
        CheckZoomDistance();
    }

    private void CheckZoomDistance()
    {
        if (Mathf.Abs(transform.position.z - _zoomTarget.z) <= _zoomStopDistance)
        {
            transform.position = _zoomTarget;

            switch (_action)
            {
                case Action.ZoomIn:
                    _action = Action.SongIdle;
                    Debug.Log("Call movement complete in !!");

                    SongManager.OnCameraReady();
                    break;
                case Action.ZoomOut:
                    //Debug.Log("Call movement complete out!!");

                    SongManager.OnCameraReady();
                    break;
            }
        }
    }

    ////

    private Vector2 ClampPosition(Vector2 pos)
    {
        float clampedX = Mathf.Clamp(pos.x, _minX, _maxX);
        float clampedY = Mathf.Clamp(pos.y, _minY, _maxY);
        return new Vector2(clampedX, clampedY);
    }

    void UpdateCamSizeInfo()
    {
        if(_cam.orthographic)
        {
            _camSize.y = _cam.orthographicSize;
            _camSize.x = _camSize.y * _cam.aspect;
        }
        else
        {
            float distance = Mathf.Abs(transform.position.z - _levelZ);
            Vector2 botLeftCorner = _cam.ViewportToWorldPoint(new Vector3(0, 0, distance));

            _camSize.x = Mathf.Abs(botLeftCorner.x - transform.position.x);
            _camSize.y = Mathf.Abs(botLeftCorner.y - transform.position.y);
        }
    }

    //private void DefineZoomTarget()

    private void UpdateMaxY()
    {
        _maxY = _topBound.position.y - _camSize.y;
    }

    private void UpdateMinY()
    {
        _minY = _botBound.position.y + _camSize.y;
    }

    private void UpdateMaxX()
    {
        _maxX = _rightBound.position.x - _camSize.x;
    }

    private void UpdateMinX()
    {
        _minX = _leftBound.position.x + _camSize.x;
    }
    #endregion Methods
}
