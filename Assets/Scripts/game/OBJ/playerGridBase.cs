using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerGridBase : MonoBehaviour {
	public GameObject playerBasePrefab;
	private const int MESH_W = GameScript.MESH_W;
	private const int MESH_H = GameScript.MESH_H;

	// Use this for initialization
	void Start () {
		for(int ii = 0; ii < 100; ++ii){
			int ix = Random.Range(0,MESH_W);
			int iy = Random.Range(0,MESH_H);
			Vector3 pos = new Vector3((float)ix/(float)MESH_W-0.5f,(float)iy/(float)MESH_H-0.5f,0.0f);
			GameObject plObj = GameObject.Instantiate(playerBasePrefab) as GameObject;
			plObj.transform.localPosition = pos;
//			plObj.transform.localScale = Vector3.Scale(transform.localScale,gridScale);
			plObj.transform.parent = transform.parent;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
}
