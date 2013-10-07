using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyGridBase : MonoBehaviour {
	public GameObject enemyBasePrefab;
	public GameObject posPanelPrefab;
	public float enemySpd=0.1f;
	private const int MESH_W = GameScript.MESH_W;
	private const int MESH_H = GameScript.MESH_H;
	private float mEmGenTimer;
	private Transform mParentTr;
	private float mEnemyInterval;
	private List<Vector2> mRouteIndexList;
	private List<Vector2> mRoutePosList;
	public List<Vector2> routeIndexList{ get {return mRouteIndexList; } }
	public List<Vector2> routePosList{ get {return mRoutePosList; } }
	
	// Use this for initialization
	void Start () {
		mEmGenTimer=0.0f;
		mRouteIndexList = createRouteIndexList(MESH_W,MESH_H,0.8f);
		mRoutePosList = new List<Vector2>();
		mParentTr = transform.parent;
		mEnemyInterval = enemySpd*5.0f;
		transform.position = mParentTr.position;
		transform.localScale = mParentTr.localScale;
		for(int ii = 0; ii < mRouteIndexList.Count; ++ii){
			Vector2 pos = Vector2.Scale(mRouteIndexList[ii],new Vector2(1.0f/(float)MESH_W,1.0f/(float)MESH_H));
			pos -= new Vector2((MESH_W-1.0f)/MESH_W*0.5f,(MESH_H-1.0f)/MESH_H*0.5f);
			mRoutePosList.Add(pos);
		}
		createEnemyRouteSign(mRouteIndexList);
	}
	
	// Update is called once per frame
	void Update () {
		mEmGenTimer += Time.deltaTime;
		if(mEmGenTimer > mEnemyInterval){
			mEmGenTimer -= mEnemyInterval;
			GameObject emObj = GameObject.Instantiate(enemyBasePrefab) as GameObject;
			emObj.transform.parent = mParentTr;
			emObj.SendMessage("SM_SetRoutePosList",routePosList);
			emObj.SendMessage("SM_SetMoveSpeed",enemySpd);
		}
	}

	List <Vector2> createRouteIndexList(int _meshW, int _meshH, float _xMoveRate){
		int[,] gridArr = new int[_meshW,_meshH];
		List <Vector2>posList = new List<Vector2>();
		int px = Random.Range(0,_meshW);
		int py = 0;
		while(py<_meshH){
			float rnd = Random.value;
			if(gridArr[px,py]==0){
				gridArr[px,py]=1;
				posList.Add(new Vector2(px,py));
			}
			if(rnd < _xMoveRate*0.5f){
				if((px>0)&&(gridArr[px-1,py]==0)) px -= 1;
			}else if(rnd < _xMoveRate){
				if((px<(_meshW-1))&&(gridArr[px+1,py]==0)) px += 1;
			}else{
				py += 1;
			}
		}
		posList.Reverse();
		
		return posList;
	}

	void createEnemyRouteSign(List <Vector2>indexList){
		float delay = 0.5f;
		Vector3 gridScale = new Vector3(1.0f/MESH_W,1.0f/MESH_H,0.0f);
		for(int ii = 0; ii < indexList.Count ; ++ii){
			int ix = (int)indexList[ii].x;
			int iy = (int)indexList[ii].y;
			Vector3 pos = mParentTr.position;
			pos.x -= mParentTr.lossyScale.x*(MESH_W-1.0f)/MESH_W*0.5f;
			pos.y -= mParentTr.lossyScale.y*(MESH_H-1.0f)/MESH_H*0.5f;
			pos.z += 0.3f;
			pos += Vector3.Scale(mParentTr.localScale, new Vector3(ix*gridScale.x,iy*gridScale.y,0.0f)); 
			GameObject go = GameObject.Instantiate(posPanelPrefab) as GameObject;
			go.transform.localPosition = pos;
			go.transform.localScale = Vector3.Scale(mParentTr.localScale,gridScale);
			go.transform.parent = mParentTr;
			go.SendMessage("SM_SetType",selGridOne.TYPE.FLASH_BLINK);
			go.SendMessage("SM_SetDelay",delay);
			delay += 0.005f/enemySpd;
		}
	}
}
