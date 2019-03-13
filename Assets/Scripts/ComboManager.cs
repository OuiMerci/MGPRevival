using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    #region Fields
    public enum ComboState
    {
        empty,
        tooEarly,
        linkedInputOnly,
        AnyInput
    }
    public ComboState _comboState = ComboState.empty;

    public enum HitboxType
    {
        ForwardSmall,
        ForwardBig,
        ForwardUp,
        ForwardDown,
    }

    public enum AttackAnimations
    {
        SongStarter,
        slash01,
        slash02,
        slash03,
        slashDown,
        slashUp
    }

    public enum Inputs
    {
        SongStart, // this is a special input and doesn't behave like other combo inputs
        Neutral,
        UpAttack,
        LongAttack,
        DownAttack,
    }

    [SerializeField] private HitInfo _songStarterInfo;
    [SerializeField] private HitInfo[] _initiatingHits;
    [SerializeField] private Inputs[] _initiatingInputs;

    private int[] _animHashes;
    private HitInfo _currentHit;
    private int _initInputLength;
    private Animator _animator;
    private double _animCompleteTime = 0;
    #endregion

    #region Properties
    public bool Attacking
    {
        get { return (_comboState == ComboState.tooEarly || _comboState == ComboState.linkedInputOnly); }
    }
    #endregion

    #region Methods
    // Avoid using strings to change the state of the animator, fill an array with all possible hashes instead
    void FillHashes()
    {
        string[] animNames = System.Enum.GetNames(typeof(AttackAnimations));
        int length = animNames.Length;
        _animHashes = new int[length];

        // Iterate through the list of anims
        for (int i = 0; i < length; i++)
        {
            _animHashes[i] = Animator.StringToHash(animNames[i]);
            //Debug.Log("Anim name : " + animNames[i] + "   Anim hash : " + _animHashes[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _initInputLength = _initiatingInputs.Length;
        FillHashes();
    }

    // Update is called once per frame
    void Update()
    {
        if (_comboState == ComboState.AnyInput)
        {
            CheckComboDelay();
        }
    }

    // Can the player still combo ?
    private void CheckComboDelay()
    {
        if(_animCompleteTime + _currentHit.ComboDelay < Time.time)
        {
            _comboState = ComboState.empty;
            //Debug.Log("Delay over");
        }
    }

    // Start a new combo based on the input
    private void InitiateCombo(Inputs newInput)
    {
        for(int i = 0; i < _initInputLength; i++)
        {
            if(_initiatingInputs[i] == newInput)
            {
                StartNewHit(_initiatingHits[i]);
                return;
            }
        }
    }

    // Go through le list of linked inputs to see if the newInput keeps comboing
    private void CheckLinkedInputs(Inputs newInput, bool initiate)
    {
        // Song start is considered linked to every attack
        if(newInput == Inputs.SongStart)
        {
            StartNewHit(_songStarterInfo);
            return;
        }

        bool isNeutralLinked = false;
        int neutralIndex = 0;

        for(int i = 0; i < _currentHit.LinkedInputs.Length; i++)
        {
            Inputs testedInput = _currentHit.LinkedInputs[i];

            if(_currentHit.LinkedInputs[i] == newInput)
            {
                StartNewHit(_currentHit.LinkedHits[i]);
                //Debug.Log("Combo found : " + _currentHit);
                return;
            }

            // is neutral attack linked ? (save the index)
            else if (isNeutralLinked == false && testedInput == Inputs.Neutral)
            {
                isNeutralLinked = true;
                neutralIndex = i;
            }
        }

        // if neutral is linked and the input is up/down attack --> trigger it as neutral attack
        if(isNeutralLinked && (newInput == Inputs.DownAttack || newInput == Inputs.UpAttack))
        {
            StartNewHit(_currentHit.LinkedHits[neutralIndex]);
        }


        // If no linked input was found, should we try to initiate a new combo ?
        if(initiate)
        {
            InitiateCombo(newInput);
        }
    }

    public void HandleInput(Inputs newInput)
    {
        switch(_comboState)
        {
            case ComboState.empty:
                InitiateCombo(newInput);
                break;

            case ComboState.linkedInputOnly:
                // If the animation isn't over yet, check only linked inputs
                CheckLinkedInputs(newInput, false);
                break;

            case ComboState.AnyInput:
                // If the animation is over, also check initiating inputs
                CheckLinkedInputs(newInput, true);
                break;
            default:
                break;
        }
    }

    public void StartNewHit(HitInfo hit)
    {
        _currentHit = hit;
        _comboState = ComboState.tooEarly;
        _animator.Play(_animHashes[(int)hit.Animation]);
        //Debug.Log("Playing hit : " + hit.HitName);
    }

    public void ReadyForNext()
    {
        _comboState = ComboState.linkedInputOnly;
    }

    public void AnimationComplete()
    {
        //Debug.Log("Anim complete");
        _animCompleteTime = Time.time;
        _comboState = ComboState.AnyInput;
    }
    #endregion
}
