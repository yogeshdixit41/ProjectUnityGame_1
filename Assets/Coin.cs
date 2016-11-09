using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public float angle = 90;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 rotation = new Vector3(0, angle, 0);
        rotation *= Time.deltaTime;
        transform.Rotate(rotation);
	}
}
