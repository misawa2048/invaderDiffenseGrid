using UnityEngine;
using System.Collections;

public class dropItem : MonoBehaviour {
	private Vector3 mSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = transform.localPosition+mSpeed*Time.deltaTime*0.5f;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		if((screenPos.x < 0.0f)||(screenPos.y < 0.0f)||(screenPos.x > Screen.width)||(screenPos.y > Screen.height)){
			Destroy(gameObject);
		}
	}
	
	private void SM_setSpeed(Vector2 _spd){
		mSpeed = _spd;
	}
}
