using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

	
	// Update is called once per frame
	public void StartLevel () {
        SceneManager.LoadScene(1);
	}
}
