using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : GameEntity
{
    [SerializeField] private float _duration;
    [SerializeField] private float _delay;
    [SerializeField] private Transform _destination;

    private Vector3 _start;
    private Vector3 _dest;
    private List<GameObject> _children;
    private bool _updateArt;
    private ArtefactBehaviour _artefact;
    private Sequence _seq;

    // Position Update
    private Vector3 _previousPos;
    private Vector3 _deltaPos;

    public bool UpdateArtefact
    {
        set { _updateArt = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _dest = _destination.position;
        _start = transform.position;
        _previousPos = transform.position;
        _children = new List<GameObject>();
        _artefact = ArtefactBehaviour.Instance;

        // Grab a free Sequence to use
        _seq = DOTween.Sequence();
        // Add a movement tween at the beginning
        _seq.Append(transform.DOMove(_dest, _duration).SetDelay(_delay));
        // Add a a backward movement
        _seq.Append(transform.DOMove(_start, _duration).SetDelay(_delay));
        // Add loop
        _seq.SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerBehaviour.Instance.transform.parent = this.transform;
        }
    }

    private void LateUpdate()
    {
        UpdateChildren();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("New child : " + other.gameObject.name);

        GroundCheck newGC = other.GetComponent<GroundCheck>();
        if (newGC != null)
            _children.Add(newGC.Parent);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Lost child : " + other.gameObject.name);

        GroundCheck newGC = other.GetComponent<GroundCheck>();
        if (newGC != null)
            _children.Remove(newGC.Parent);
    }

    void UpdateChildren()
    {
        _deltaPos = transform.position - _previousPos;
        _previousPos = transform.position;

        for(int i = 0; i < _children.Count; i++)
        {
            //Debug.Log("Updated child : " + _children[i]);
            _children[i].transform.position += _deltaPos;
        }

        if(_updateArt)
        {
            _artefact.transform.position += _deltaPos;
        }
    }

    protected override void OnSongStart(Enemy enemy)
    {
        _seq.Pause();
    }

    protected override void OnSongEnd()
    {
        _seq.Play();
    }
}
