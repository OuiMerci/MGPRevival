using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("GoLoadLevel2", 3);
	}
	
	void GoLoadLevel2()
    {
        Debug.Log("Go Load level2");
        GameManager.Instance.OnLevelFinished();
    }
}
