using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class touchGrid : MonoBehaviour {
	public GameObject posPanelObj;
	public GameObject posLineObj;
	private const int MESH_W = GameScript.MESH_W;
	private const int MESH_H = GameScript.MESH_H;
	private TmSystem mSys;
	private GameScript mGame;
	private Vector2 mTouchLocalPos;
	private Vector2 mSelPosId;
	private Mesh mMesh;
	private bool mTouchEnable;
	private float mTimer;
	private GameObject mNearestPlObj;
		
	// Use this for initialization
	void Start () {
		mSys = TmSystem.instance;
		mGame = GameScript.instance;
		mSelPosId = Vector2.one * -1.0f;
		mTouchLocalPos = Vector2.one * -1.0f;
		mTouchEnable = true;
		mTimer = 0.0f;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if((meshFilter!=null)&&(meshFilter.mesh!=null)){
			meshFilter.mesh = meshFilter.sharedMesh = TmUtils.CreateGridXY(MESH_W,MESH_H);
			mMesh = meshFilter.mesh;
		}
		posPanelObj.transform.localScale = new Vector3(1.0f/MESH_W,1.0f/MESH_H,1.0f);
		mMesh = TmUtils.SetMeshColor(mMesh,new Color(0.2f,0.5f,0.2f,0.2f));
		
	}
	
	// Update is called once per frame
	void Update () {
//		mTouchEnable = (mGame.command=="CMD16");
		
		float alpha = (Mathf.Sin(mTimer*4.0f)+1.0f)*0.4f+0.2f;
		mMesh = TmUtils.SetMeshColor(mMesh,new Color(0.2f,0.5f,0.2f,alpha));
		if(!mTouchEnable){
			mTimer = 0.0f;
			posPanelObj.SendMessage("SM_SetType",selGridOne.TYPE.OFF);
			return;
		}
		mTimer += Time.deltaTime;
		
		if((mSys.mw.buttonState==TmMouseWrapper.STATE.ON)||(mSys.mw.buttonState==TmMouseWrapper.STATE.DOWN)){
			if(mSys.mw.isOnDragTarget(gameObject)){
//				mTouchLocalPos = mSys.mw.mouseHit.textureCoord+Vector2.one*-0.5f;
				mTouchLocalPos = transform.worldToLocalMatrix.MultiplyPoint(mSys.mw.mouseHit.point);
				posPanelObj.SendMessage("SM_SetDestLocalPos",mTouchLocalPos);
				posPanelObj.SendMessage("SM_SetType",selGridOne.TYPE.ON);
				mSelPosId = new Vector2(Mathf.Floor((mTouchLocalPos.x+0.5f)*MESH_W),Mathf.Floor((mTouchLocalPos.y+0.5f)*MESH_H));
				
				// display touch cursor at touch grid.
				if(mSys.mw.buttonState==TmMouseWrapper.STATE.DOWN){
					mNearestPlObj = mGame.GetNrarestPl(mTouchLocalPos,new Vector2(0.5f/MESH_W,0.5f/MESH_H));
					if(mNearestPlObj==null){
						posLineObj.SendMessage("SM_SetSttPos",mTouchLocalPos);
					}
				}
				if(mNearestPlObj!=null){
					Vector2 tmpLocalPos = mNearestPlObj.transform.localPosition;
					posLineObj.SendMessage("SM_SetSttPos",tmpLocalPos);
				}
				posLineObj.SendMessage("SM_SetEndPos",mTouchLocalPos);
				posLineObj.SendMessage("SM_SetColor",(mNearestPlObj==null) ? Color.red : Color.white);
				posLineObj.renderer.enabled = true;
			}
		}else{
			posPanelObj.SendMessage("SM_SetType",selGridOne.TYPE.BLINK_OFF);
			if(mSys.mw.buttonState==TmMouseWrapper.STATE.UP){
				if(mNearestPlObj!=null){
					if((mSelPosId.x>=0)&&(mSelPosId.y>=0)){
						mNearestPlObj.SendMessage("SM_SetDestGridId",mSelPosId);
						mNearestPlObj = null;
					}
				}
			}
			
			posLineObj.renderer.enabled = false;
		}
	}
	
}
