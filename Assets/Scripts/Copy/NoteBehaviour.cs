using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FruitRythm
{
    public enum NoteType
    {
        Pink,
        Blue,
        Yellow,
        Green,
        Red
    }

    public class NoteBehaviour : MonoBehaviour
    {

        [SerializeField] private NoteType _type = NoteType.Pink;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public NoteBehaviour(NoteType type)
        {
            _type = type;
        }

        public NoteType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}