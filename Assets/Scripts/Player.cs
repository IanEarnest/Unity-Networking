using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int speed = 5;
	/*
	private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
	 */

	void Update () {
		// Seperate controlls
		if(networkView.isMine){
			// When you press a button, move transform of player.
			if(Input.GetKey(KeyCode.W)){
				rigidbody.MovePosition(rigidbody.position + Vector3.up * speed * Time.deltaTime);
			}
			if(Input.GetKey(KeyCode.A)){
				rigidbody.MovePosition(rigidbody.position + Vector3.left * speed * Time.deltaTime);
			}
			if(Input.GetKey(KeyCode.S)){
				rigidbody.MovePosition(rigidbody.position + Vector3.down * speed * Time.deltaTime);
			}
			if(Input.GetKey(KeyCode.D)){
				rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
			}
		}
		else{
		}
	}
}
