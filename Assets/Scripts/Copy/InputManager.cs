using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FruitRythm
{

    public class InputManager : MonoBehaviour
    {

        private float _downLength = 0;
        private bool _isDown = false;

        // Blue -> Yellow -> Green -> Red
        [SerializeField] private float _blueLength;
        [SerializeField] private float _yellowLength;
        [SerializeField] private float _greenLength;
        [SerializeField] private float _redLength;
        [SerializeField] private GameObject _clickFeedback;

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                OnClickDown();
                Vector2 clickedPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _clickFeedback.transform.position = new Vector3(clickedPoint.x, clickedPoint.y, 0);
                _clickFeedback.SetActive(true);
                Debug.Log("click : " + Input.mousePosition + "     point : " + clickedPoint);
            }

            else if (Input.GetMouseButtonUp(0))
            {
                OnClickUp();
                _clickFeedback.SetActive(false);
            }

            if (_isDown)
                _downLength += Time.deltaTime;

            if (Input.GetKeyDown("r"))
            {
                Partition.Instance.RestartPartition();
            }

        }

        private void OnClickDown()
        {
            _isDown = true;
        }

        private void OnClickUp()
        {
            Debug.Log("Mouse up, down time : " + _downLength);

            NoteType inputNote = GetNote(_downLength);

            _downLength = 0;
            _isDown = false;

            Partition.Instance.TestInput(inputNote);
        }

        private NoteType GetNote(float inputLength)
        {
            if (inputLength < _blueLength)
            {
                Debug.Log("Note -> Pink");
                return NoteType.Pink;
            }
            else if (inputLength < _yellowLength)
            {
                Debug.Log("Note -> Blue");
                return NoteType.Blue;
            }
            else if (inputLength < _greenLength)
            {
                Debug.Log("Note -> Yellow");
                return NoteType.Yellow;
            }
            else if (inputLength < _redLength)
            {
                Debug.Log("Note -> Green");
                return NoteType.Green;
            }
            else
            {
                Debug.Log("Note -> Red");
                return NoteType.Red;
            }
        }

    }
}