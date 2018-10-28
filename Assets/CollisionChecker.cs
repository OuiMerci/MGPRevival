using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour {

    [SerializeField] BoxCollider2D _rightColl;
    [SerializeField] BoxCollider2D _leftColl;
    [SerializeField] BoxCollider2D _topColl;
    [SerializeField] BoxCollider2D _bottomColl;
    private CapsuleCollider2D _mainColl;
    private float playerArtefactScaleRatio;

    private void Awake()
    {
        _mainColl = GetComponent<CapsuleCollider2D>();
        Debug.Log("awake from maincoll : " + _mainColl);
    }

    // Use this for initialization
    void Start () {
        InitColliders();
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = Vector3.zero;
	}

    void InitColliders()
    {
        // Compute the ratio between player and artefact scales
        playerArtefactScaleRatio = PlayerBehaviour.Instance.transform.localScale.magnitude / ArtefactBehaviour.Instance.transform.localScale.magnitude;

        // Copy the player's collider
        CapsuleCollider2D playerColl = PlayerBehaviour.Instance.GetComponent<CapsuleCollider2D>();
        _mainColl.size = playerColl.size * playerArtefactScaleRatio;
        _mainColl.offset = Vector3.zero;

        // init sub colliders
        _rightColl.offset = new Vector2(_mainColl.size.x * 1/4, 0);
        _rightColl.size = new Vector2(_mainColl.size.x/2, _mainColl.size.y);

        _leftColl.offset = new Vector2(-_mainColl.size.x * 1/4, 0);
        _leftColl.size = new Vector2(_mainColl.size.x/2, _mainColl.size.y);

        _topColl.offset = new Vector2(0, _mainColl.size.y * 1/4);
        _topColl.size = new Vector2(_mainColl.size.x, _mainColl.size.y/2);

        _bottomColl.offset = new Vector2(0, -_mainColl.size.y * 1/4);
        _bottomColl.size = new Vector2(_mainColl.size.x, _mainColl.size.y/2);

        /*
        _leftColl.offset = new Vector2(_mainColl.size.x * 3/8, 0);
        _leftColl.size = new Vector2(_mainColl.size.x/4, _mainColl.size.y * 6/8);

        _rightColl.offset = new Vector2(-_mainColl.size.x * 3 / 8, 0);
        _rightColl.size = new Vector2(_mainColl.size.x / 4, _mainColl.size.y * 6 / 8);

        _topColl.offset = new Vector2(0, _mainColl.size.y * 7 / 16);
        _topColl.size = new Vector2(_mainColl.size.x / 2, _mainColl.size.y * 1 / 8);

        _bottomColl.offset = new Vector2(0, -_mainColl.size.y * 7 / 16);
        _bottomColl.size = new Vector2(_mainColl.size.x / 2, _mainColl.size.y * 1 / 8);
        */
    }

    void CheckCollisions()
    {
    }
}
