using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FruitRythm
{
    public class Partition : MonoBehaviour
    {

        public NoteBehaviour _notePrefab = null;

        [SerializeField] public Text _timeText;
        [SerializeField] public Text _bestTimeText;
        [SerializeField] public Text _startText;
        [SerializeField] public Text _feedbackOKText;
        [SerializeField] public Text _feedbackFailText;
        [SerializeField] private NoteType[] _typeList;
        private List<NoteBehaviour> _noteList = null;
        private int _currentNoteID = 0;
        private float _startTime = 0;
        private float _bestTime = 0;
        private Vector3 _cameraOrigin = Vector3.zero;
        private bool _timeStarted = false;
        private bool _partitionComplete = false;
        private float _totalTime = 0.0f;

        static private Partition _instance = null;

        static public Partition Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            _instance = this;
        }

        // Use this for initialization
        void Start()
        {
            InitializeNoteList();
            _cameraOrigin = Camera.main.transform.position;

        }

        // Update is called once per frame
        void Update()
        {
            if (_timeStarted == true && _partitionComplete == false)
            {
                _totalTime = ((_startTime - Time.realtimeSinceStartup) * -1);
                _timeText.text = "Time : " + _totalTime;
            }
        }

        private void InitializeNoteList()
        {
            _noteList = new List<NoteBehaviour>();

            int i = 0;

            foreach (NoteType type in _typeList)
            {
                NoteBehaviour note = GameObject.Instantiate<NoteBehaviour>(_notePrefab, this.transform);
                note.Type = type;
                SetNoteColor(note);

                note.gameObject.transform.position = new Vector2(i, 0);
                i++;

                _noteList.Add(note);
                Debug.Log("Added note to list. Type : " + type);
            }
        }

        public void SetNoteColor(NoteBehaviour note)
        {
            switch (note.Type)
            {
                case NoteType.Pink:
                    note.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                    break;

                case NoteType.Blue:
                    note.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    break;

                case NoteType.Yellow:
                    note.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                    break;

                case NoteType.Green:
                    note.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    break;

                case NoteType.Red:
                    note.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    break;
            }
        }

        public void TestInput(NoteType inputNote)
        {
            if (_currentNoteID == 0 && _timeStarted == false)
            {
                _timeStarted = true;
                _startTime = Time.realtimeSinceStartup;
                StartCoroutine(StartTextCoroutine());
            }

            if (inputNote == _noteList[_currentNoteID].Type)
            {
                Debug.Log("Correct note ! Type : " + inputNote);
                _noteList[_currentNoteID].gameObject.SetActive(false);

                if (_currentNoteID < _noteList.Count - 1)
                {
                    _currentNoteID++;
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + 1, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    ShowOKText();
                }
                else
                {
                    _partitionComplete = true;

                    Debug.Log("Complete Time = " + _totalTime);
                    _timeText.text = "Time : " + _totalTime.ToString();

                    if ((Time.realtimeSinceStartup - _startTime) < _bestTime || _bestTime == 0)
                    {
                        _bestTime = _totalTime;
                        _bestTimeText.text = "Best : " + _bestTime.ToString();
                        Debug.Log("update best time");
                    }

                    HideFeedbackTexts();
                }
            }
            else
            {
                Debug.Log("Wrong note !!!!! Inupt : " + inputNote + "    Expected : " + _noteList[_currentNoteID].Type);
                ShowFailText(inputNote);
            }
        }

        public void RestartPartition()
        {
            Camera.main.transform.position = _cameraOrigin;
            _timeText.text = "Time : 000";
            _currentNoteID = 0;
            _timeStarted = false;
            _partitionComplete = false;
            _totalTime = 0;

            foreach (NoteBehaviour n in _noteList)
            {
                n.gameObject.SetActive(true);
            }
        }

        IEnumerator StartTextCoroutine()
        {
            _startText.enabled = true;
            yield return new WaitForSeconds(1.5f);
            _startText.enabled = false;
        }

        private void ShowFailText(NoteType note)
        {
            _feedbackFailText.text = "Wrong timing ! Popped bubble type : " + note.ToString();

            _feedbackOKText.enabled = false;
            _feedbackFailText.enabled = true;
        }

        private void ShowOKText()
        {
            _feedbackOKText.enabled = true;
            _feedbackFailText.enabled = false;
        }

        private void HideFeedbackTexts()
        {
            _feedbackOKText.enabled = false;
            _feedbackFailText.enabled = false;
        }
    }
}