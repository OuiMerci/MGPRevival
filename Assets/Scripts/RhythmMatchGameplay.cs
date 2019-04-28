using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmMatchGameplay : MonoBehaviour
{
    #region Fields
    public enum Note
    {
        wait,
        one,
        two,
        three,
        four
    }

    private Queue<Note> _notes;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetNotes(Queue<Note> notes)
    {
        _notes = notes;
    }

    public void HandlePlayedNote(Note note)
    {
        Debug.Log("Handle played note : " + note);
    }
    #endregion
}
