using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	private Animator animator;
	private Canvas interactionUI;
	bool OpenTrigger = false;
	bool PlayerInCollider = false;

	// Use this for initialization
	void Start ()
	{
		//animator = GetComponent<Animator> ();
		animator = transform.Find("Door_01").GetComponent<Animator> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player" && this.transform.gameObject.tag != "Room_Exit") {
			PlayerInCollider = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player" && this.transform.gameObject.tag != "Room_Exit") {
			PlayerInCollider = false;
		}
	}

	void OpenDoor() {
		if (animator != null) {
			animator.SetBool ("IsOpen", !animator.GetBool("IsOpen"));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (OpenTrigger) {
			OpenDoor ();
			OpenTrigger = false;
		}

        if (Input.GetKeyDown (KeyCode.E)) {
            if (PlayerInCollider) {
                OpenTrigger = true;
            }
        }
	}
}
