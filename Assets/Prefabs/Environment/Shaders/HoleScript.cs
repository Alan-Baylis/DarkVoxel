using UnityEngine;
using System.Collections;

public class HoleScript : MonoBehaviour {

	public Collider player; // assign in inspector?
	public TerrainCollider tCollider; // assign in inspector?

	//public GameObject groupHide;

	void OnTriggerEnter (Collider c) {
		if (c.tag == "Player") {
			Physics.IgnoreCollision(player, tCollider, true);
			//groupHide.gameObject.SetActive (false);
		}
	}

	void OnTriggerExit (Collider c) {
		if (c.tag == "Player") {
			Physics.IgnoreCollision(player, tCollider, false);
			//groupHide.gameObject.SetActive (true);
		} 
	}

}