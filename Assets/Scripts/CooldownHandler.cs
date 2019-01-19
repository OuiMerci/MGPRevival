using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private double _cooldownDuration;
    [SerializeField] private GameObject _feedback;
    private Animator _animator;
    private SpriteRenderer _feedbackRenderer;
    private bool _available;
    private double _lastUseTime;
    
    #endregion

    #region Properties
    public bool Available
    {
        get { return _available; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _feedbackRenderer = GetComponent<SpriteRenderer>();
        _available = true;
        _lastUseTime = -_cooldownDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(_available == false)
        {
            if(_lastUseTime + _cooldownDuration < Time.time)
            {
                CooldownComplete();
            }
        }
    }

    private void CooldownComplete()
    {
        //Debug.Log("CD COMPLETE");
        ShowFeedback(true);
        _available = true;
    }

    private void ShowFeedback(bool show)
    {
        //Debug.Log("Show feedback : " + show);
        _animator.enabled = show;
        _feedbackRenderer.enabled = show;
    }

    public void FeedbackComplete()
    {
        //Debug.Log("FB COMPLETE");
        ShowFeedback(false);
    }

    public void StartCooldown()
    {
        //Debug.Log("CD STARTED");
        _lastUseTime = Time.time;
        _available = false;
    }
    #endregion
}
