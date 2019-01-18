using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _zOffset;
    [SerializeField] private float _levelZ;
    [SerializeField] private float _smoothDuration;
    [SerializeField] private Transform _topBound;
    [SerializeField] private Transform _botBound;
    [SerializeField] private Transform _leftBound;
    [SerializeField] private Transform _rightBound;

    private Transform _player;
    private Vector3 _velocity;
    private Camera _cam;

    private Vector2 _camSize;
    private float _maxY;
    private float _minY;
    private float _maxX;
    private float _minX;
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

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateMaxY();
        UpdateMinY();
        UpdateMaxX();
        UpdateMinX();

        FollowPlayer();
    }

    

    private void FollowPlayer()
    {
        Vector3 desiredPos = ClampPosition(_player.position); //clamp the position
        desiredPos = desiredPos + new Vector3(0, 0, _zOffset); // add the camera zOffset

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _velocity, _smoothDuration); //apply smooth follow
    }

    private void FollowPlayerInstant()
    {
        Vector3 desiredPos = ClampPosition(_player.position); //clamp the position
        desiredPos = desiredPos + new Vector3(0, 0, _zOffset); // add the camera zOffset

        transform.position = desiredPos; // Apply position without smooth
    }

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
