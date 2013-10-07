using UnityEngine;
using System.Collections;

public class frontLine : MonoBehaviour {
	private GameScript mGame;
	private Mesh mMesh;
	private float mFrontLineY;

	// Use this for initialization
	void Start () {
		mGame = GameScript.instance;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if((meshFilter!=null)&&(meshFilter.mesh!=null)){
			TmUtils.SetMeshColor(meshFilter.mesh,new Color(1.0f,0.3f,0.2f,0.5f));
		}
	}
	
	// Update is called once per frame
	void Update () {
		mFrontLineY = 0.5f;
		foreach(GameObject go in mGame.emList){
			if(mFrontLineY > go.transform.localPosition.y){
				mFrontLineY = go.transform.localPosition.y;
			}
		}
		if(mFrontLineY<-0.5f){ mFrontLineY = -0.5f; }
		Vector3 newPos = transform.localPosition;
		if(transform.localPosition.y > mFrontLineY){
			newPos.y = mFrontLineY;
		}else{
			newPos.y = Mathf.Lerp(newPos.y,mFrontLineY,Time.deltaTime);
		}
		transform.localPosition = newPos;
	}
}
