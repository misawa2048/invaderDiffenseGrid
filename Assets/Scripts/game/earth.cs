using UnityEngine;
using System.Collections;

public class earth : MonoBehaviour {
	public GameObject stratosphereObj;
	public float radius=1.0f;
	public Vector3 rot;
	private Vector3 mDefPos;
	private Vector3 mDefLocalScale;
	private Vector3 mDefStratosphereScale;
	void Awake(){
		mDefPos = transform.position;
		mDefLocalScale = transform.localScale;
		mDefStratosphereScale = stratosphereObj.transform.localScale;
//		renderer.material = renderer.sharedMaterial;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rot*Time.deltaTime);
		if(radius > mDefLocalScale.y){
			radius *= TmUtils.LeapBreak(radius,0.995f,Time.deltaTime*5.0f);
			transform.localScale = Vector3.one * radius;
		}
		transform.position = mDefPos - Vector3.up*radius*0.5f + Vector3.Scale(mDefLocalScale,Vector3.up*0.5f);
		stratosphereObj.transform.position = transform.position + Vector3.back*radius*0.5f;
		stratosphereObj.transform.localScale = mDefStratosphereScale * radius;
		
	}
}
