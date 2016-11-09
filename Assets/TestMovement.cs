using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestMovement : MonoBehaviour {

		public float speed = 5;
		public float gravity = -100;

		CharacterController cc;
		Rigidbody rbody;
		// Use this for initialization
		void Start () {
		cc = GetComponent<CharacterController> ();
		rbody = cc.attachedRigidbody;

	}

		Vector3 delta;
		// Update is called once per frame
		void Update () {

			delta.x = Input.GetAxis("Horizontal")*speed;
			delta.z = Input.GetAxis("Vertical")*speed;

			if (cc.isGrounded)
			{
				delta.y = 0;
			}

			delta.y += gravity * Time.deltaTime;
			//rbody.AddForce (delta);
			cc.Move (delta * Time.deltaTime);
		}

		void OnCollisionEnter (Collision col)
		{
			if (col.gameObject.tag == "AI") {

				Destroy (gameObject);
				Debug.Log ("Inside collision enter in GameChar.cs");
				SceneManager.LoadScene (0);
			}
		}
	}