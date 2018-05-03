using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour {

    [SerializeField] private int _loadingTime = 0;

	// Use this for initialization
	void Start () {
        Invoke("LoadMenu", _loadingTime);		
	}

    private void LoadMenu()
    {
        // Scene "1" should be the menu
        SceneManager.LoadScene(1);
    }
}
