using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongActorsMover : MonoBehaviour
{
    #region Fields
    struct gm
    {
        public Color c;
        public Vector3 p;
    }

    static public Vector3 _playerPositionBackup;

    private List<gm> _guizmosList;
    [SerializeField] float _actorsDistance;
    [SerializeField] float _stopDistance;
    [SerializeField] float _actorsMoveDuration;

    // Actors
    private Transform _actor1;
    private Transform _actor2;

    // Positions before the song begins
    private Vector2 _actorsMiddlePoint;
    private Vector3 _actor1PositionBackup;
    private Vector3 _actor2PositionBackup;

    // Positions after being teleported to the song zone
    private Vector3 _actor1AfterTPPosition;
    private Vector3 _actor2AfterTPPosition;

    // Positions when the song gameplay begins
    private Vector3 _actor1AimedPosition;
    private Vector3 _actor2AimedPosition;

    // Movement
    private Vector3 _actor1Velocity;
    private Vector3 _actor2Velocity;

    // Position Check
    private bool _actor1OnPosition;
    private bool _actor2OnPosition;

    // state
    enum Action
    {
        Idle,
        MoveToSongPosition,
        MoveToOrigin
    }
    private Action _action;
    #endregion

    #region Methods
    private void OnDrawGizmos()
    {
        if(_guizmosList != null)
        {
            foreach (gm guizmo in _guizmosList)
            {
                Gizmos.color = guizmo.c;
                Gizmos.DrawCube(guizmo.p, new Vector3(3, 3, 3));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _guizmosList = new List<gm>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_action)
        {
            case Action.Idle:
                break;
            case Action.MoveToSongPosition:
                UpdateActorPosition(_actor1, _actor1AimedPosition, ref _actor1OnPosition, ref _actor1Velocity);
                UpdateActorPosition(_actor2, _actor2AimedPosition, ref _actor2OnPosition, ref _actor2Velocity);

                if(_actor1OnPosition && _actor2OnPosition)
                {
                    OnMovementCompleteReady();
                }
                break;
            case Action.MoveToOrigin:
                UpdateActorPosition(_actor1, _actor1AfterTPPosition, ref _actor1OnPosition, ref _actor1Velocity);
                UpdateActorPosition(_actor2, _actor2AfterTPPosition, ref _actor2OnPosition, ref _actor2Velocity);

                if (_actor1OnPosition && _actor2OnPosition)
                {
                    OnMovementCompleteReady();
                }
                break;
        }
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

    private void OnSongStart(Enemy enemy)
    {
        // Make a backup for the player's position
        _playerPositionBackup = PlayerBehaviour.Instance.transform.position;

        // Set actors
        _actor1 = PlayerBehaviour.Instance.transform;
        _actor2 = enemy.transform;

        // Get the middle point between actors
        _actorsMiddlePoint = GetActorsMiddlePoint(_actor1, _actor2);

        DrawGuizmo(_actorsMiddlePoint, Color.blue);

        // Teleport them and save variables
        TeleportSongActorsToSongZone(_actor1, _actor2);

        // Set aimed positions for song
        _actor1AimedPosition = GetSongPosition(_actor1);
        _actor2AimedPosition = GetSongPosition(_actor2);

        DrawGuizmo(_actor1AimedPosition, Color.blue);
        DrawGuizmo(_actor2AimedPosition, Color.red);

        // Set Action
        _action = Action.MoveToSongPosition;
    }

    private void OnSongGameplayEnd()
    {
        _action = Action.MoveToOrigin;
    }

    private void OnSongEnd()
    {
        ResetActorsPositions();
        _action = Action.Idle;
    }

    private Vector2 GetActorsMiddlePoint(Transform actor1, Transform actor2)
    {
        float middleX = (actor1.position.x + actor2.position.x) / 2.0f;
        float middleY = (actor1.transform.position.y + actor2.transform.position.y) / 2.0f;

        return new Vector2(middleX, middleY);
    }

    private Vector2 GetCenterToActorVector(Vector3 center, Transform actor)
    {
        return actor.position - center;
    }

    private void TeleportSongActorsToSongZone(Transform actor1, Transform actor2)
    {
        // Save the original positions
        _actor1PositionBackup = actor1.transform.position;
        _actor2PositionBackup = actor2.transform.position;
        DrawGuizmo(_actor1PositionBackup, Color.green);
        DrawGuizmo(_actor2PositionBackup, Color.red);

        // Get the vector from this middle point to the actors
        Vector2 centerToActor1Vector = GetCenterToActorVector(_actorsMiddlePoint, actor1);
        Vector2 centerToActor2Vector = GetCenterToActorVector(_actorsMiddlePoint, actor2);

        // Apply this vector the SongZone center to get the position we will TP the actors to
        _actor1AfterTPPosition = SongManager.SongZoneCenter.position + (Vector3)centerToActor1Vector;
        _actor2AfterTPPosition = SongManager.SongZoneCenter.position + (Vector3)centerToActor2Vector;

        // Teleport the actors to this position
        actor1.transform.position = _actor1AfterTPPosition;
        actor2.transform.position = _actor2AfterTPPosition;

        DrawGuizmo(_actor1AfterTPPosition, Color.black);
        DrawGuizmo(_actor2AfterTPPosition, Color.yellow);
    }

    private Vector2 GetSongPosition(Transform actor)
    {
        Vector2 center = SongManager.SongZoneCenter.position;
        float aimedY = center.y;
        float aimedX;

        //Set the aimed distance from middle on X axis
        if (actor.transform.position.x >= center.x)
        {
            // Actor is on the right
            aimedX = center.x + _actorsDistance / 2.0f;
        }
        else
        {
            // Actor is on the left
            aimedX = center.x - _actorsDistance / 2.0f;
        }

        return new Vector2(aimedX, aimedY);
    }

    private void UpdateActorPosition(Transform actor, Vector3 actorDest, ref bool actorOnPosition, ref Vector3 actorVelocity)
    {
        if (actorOnPosition)
            return;

        DrawGuizmo(actorDest, Color.white);
        actor.position = Vector3.SmoothDamp(actor.position, actorDest, ref actorVelocity, _actorsMoveDuration);
        actorOnPosition = IsActorOnDestination(actor, actorDest);
    }

    private bool IsActorOnDestination(Transform actor, Vector3 actorDest)
    {
        float actorDistanceToDest = Vector3.Distance(actor.position, actorDest);
        if(actorDistanceToDest <= _stopDistance)
        {
            actor.position = actorDest;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnMovementCompleteReady()
    {
        _actor1OnPosition = false;
        _actor2OnPosition = false;

        _action = Action.Idle;
        SongManager.OnActorsMoversReady();
    }

    private void ResetActorsPositions()
    {
        _actor1.position = _actor1PositionBackup;
        _actor2.position = _actor2PositionBackup;

        _actor1 = null;
        _actor2 = null;
    }

    private void DrawGuizmo(Vector3 pos, Color color)
    {
        gm guizmo = new gm { c = color, p = pos };
        _guizmosList.Add(guizmo);
    }
    #endregion
}
