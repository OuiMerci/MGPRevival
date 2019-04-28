using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmMatchInput : MonoBehaviour
{
    #region Fields
    [SerializeField] RhythmMatchGameplay _gameplay;
    [SerializeField] float _beatDuration;

    private float _singInputStartTime;
    #endregion
    
    #region Methods
    void Start()
    {
        if (_gameplay == null)
        {
            _gameplay = GetComponent<RhythmMatchGameplay>();

            if (_gameplay == null)
                Debug.LogError("RhythmeMatch Error : No Song Gameplay was found.");
        }
    }

    void Update()
    {
        CheckSingInput();
    }

    void CheckSingInput()
    {
        if (Input.GetButtonDown("Sing") || Input.GetMouseButtonDown(0))
        {
            _singInputStartTime = Time.time;
        }
        else if(Input.GetButtonUp("Sing") || Input.GetMouseButtonUp(0))
        {
            float inputDuration = Time.time - _singInputStartTime;
            HandleInput(inputDuration);
        }
    }

    private void HandleInput(float inputDuration)
    {
        // For automatic recognition we could use the index of the enum + recursivity, but this is much easier to read
        Debug.Log("duration : " + inputDuration);

        if (inputDuration <= _beatDuration * 1)
        {
            _gameplay.HandlePlayedNote(RhythmMatchGameplay.Note.one);
        }
        else if (inputDuration <= _beatDuration * 2)
        {
            _gameplay.HandlePlayedNote(RhythmMatchGameplay.Note.two);
        }
        else if (inputDuration <= _beatDuration * 3)
        {
            _gameplay.HandlePlayedNote(RhythmMatchGameplay.Note.three);
        }
        else if (inputDuration <= _beatDuration * 4)
        {
            _gameplay.HandlePlayedNote(RhythmMatchGameplay.Note.four);
        }
    }
    #endregion
}
