using UnityEngine;
using System.Collections;

public class selButton : MonoBehaviour {
	private const float COL_RATE_SEL = 1.0f;
	private const float COL_RATE_ON = 0.75f;
	private const float COL_RATE_OFF = 0.35f;
	private TmSystem mSys;
	private GameScript mGame;
	private TmSpriteAnim mAnm;
	private Vector3 mDefLocalPos;
	private Vector3 mDestLocalPos;
	private GameScript.MyParts mParts = null;
	private Color mDefCol;
	private Vector3 mDefLocalScale;

	// Use this for initialization
	void Start () {
		mSys = TmSystem.instance;
		mGame = GameScript.instance;
		mAnm = GetComponent<TmSpriteAnim>();
		mAnm.enabled = false;
		mDefLocalPos = mDestLocalPos = transform.localPosition;
		mDefCol = renderer.material.color;
		mDefLocalScale = transform.localScale;
		if((transform.parent!=null)&&(transform.parent.tag==transform.tag)){ // child
			this.gameObject.collider.enabled = false;
			this.gameObject.renderer.enabled = true;
//			this.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.Lerp(transform.localPosition,mDestLocalPos,Time.deltaTime*20.0f);
		if((transform.parent!=null)&&(transform.parent.tag==transform.tag)){ // child
			this.gameObject.renderer.enabled = ((transform.localPosition-mDefLocalPos).magnitude>0.1f);
		}
		mAnm.enabled = (mSys.mw.isHover(gameObject) && (mParts.link != ""));
		
		bool isBro = isDescendant(gameObject, mSys.mw.hitTarget);
		float colRate = mSys.mw.isHover(gameObject) ? COL_RATE_SEL : (isBro ? COL_RATE_ON : COL_RATE_OFF);
		Color col = new Color(mDefCol.r*colRate,mDefCol.g*colRate,mDefCol.b*colRate,mDefCol.a);
		mAnm.SetMeshColor(col);
		transform.localScale = mDefLocalScale * (mSys.mw.isHover(gameObject) ? 1.05f : 1.0f);
		
		GameObject targetObj = mSys.mw.hitTarget;
		if((mSys.mw.buttonState==TmMouseWrapper.STATE.ON)||(mSys.mw.buttonState==TmMouseWrapper.STATE.DOWN)){
			if((targetObj!=null)&&(targetObj.tag==gameObject.tag)){
				mGame.command = "";
				if(targetObj==gameObject){
					popUpChild();
				}else if(!isDescendant(targetObj)){
					popOffChild();
				}
			}
		}else if(mSys.mw.buttonState==TmMouseWrapper.STATE.UP){
			if(targetObj==gameObject){
				mGame.command = ((mParts!=null)) ? mParts.link : "";
				Debug.Log("selButton.Up:"+mGame.command);
			}
			popOffChild();
		}
	}
	
	//------------------------------------------------------
	public void SM_ResetDistLocalPos(){
		mDestLocalPos = mDefLocalPos;
	}
	public void SM_SetDestLocalPos(Vector3 _pos){
		mDestLocalPos = _pos;
	}
	public void SM_SetPartsInfo(GameScript.MyParts _inParts){
		mParts = _inParts;
	}
	//------------------------------------------------------
	
	private bool isSelButton(){
		bool ret = false;
		if((transform.parent!=null)&&(transform.parent.tag==transform.tag)){ // child
			if((mSys.mw.buttonState==TmMouseWrapper.STATE.ON)&&(mSys.mw.isHover(gameObject))&&(mSys.mw.dragTarget==transform.parent.gameObject)){
				ret = true;
			}
		}else{
			if((mSys.mw.buttonState==TmMouseWrapper.STATE.ON)&&(mSys.mw.isOnDragTarget(gameObject))){
				ret = true;
			}
		}
		return ret;
	}
	
	private bool isBrother(GameObject _go, GameObject _baseObj=null){
		bool ret = false;
		if(_baseObj==null) _baseObj = this.gameObject;

		if((_go != null)&&(_go.transform.parent!=null)){
			if((_baseObj.transform.parent!=null)&&(_baseObj.transform.parent==_go.transform.parent)){
				ret = false;
			}
		}
		return ret;
	}
	
 
	private bool isDescendant(GameObject _go, GameObject _baseObj=null){
		bool ret = false;
		if(_baseObj==null) _baseObj = this.gameObject;

		if((_go != null)&&(_go.transform.parent!=null)){
			Transform ckTr = _go.transform.parent;
			do{
				if(ckTr == _baseObj.transform){
					ret = true;
					break;
				}
				ckTr = ckTr.parent;
			}while(ckTr!=null);
		}
		return ret;
	}
	
	private bool popUpChild(GameObject _parentObj=null){
		bool ret = true;
		if(_parentObj==null) _parentObj = gameObject;
		Transform tra = _parentObj.transform;
		if(tra.childCount>0){
			for(int ii = 0; ii < tra.childCount; ++ii){
				if(tra.GetChild(ii).gameObject.tag==tra.gameObject.tag){
					Vector3 mvPos = Vector3.up*0.9f;
					mvPos.x = ((float)ii-((float)(tra.childCount-1)*0.5f))*tra.GetChild(ii).localScale.x;
//					mvPos.Scale(tra.GetChild(ii).localScale);
//					tra.GetChild(ii).gameObject.SetActive(true);
					tra.GetChild(ii).gameObject.collider.enabled = true;
//					tra.GetChild(ii).gameObject.renderer.enabled = true;
					tra.GetChild(ii).SendMessage("SM_SetDestLocalPos",mvPos);
				}
			}
		}
		return ret;
	}

	private bool popOffChild(GameObject _parentObj=null){
		bool ret = true;
		if(_parentObj==null) _parentObj = gameObject;
		Transform tra = _parentObj.transform;
		if(tra.childCount>0){
			for(int ii = 0; ii < tra.childCount; ++ii){
				if(tra.GetChild(ii).gameObject.tag==tra.gameObject.tag){
//					tra.GetChild(ii).gameObject.SetActive(false);
					tra.GetChild(ii).gameObject.collider.enabled = false;
					tra.GetChild(ii).SendMessage("SM_ResetDistLocalPos");
				}
			}
		}
		return ret;
	}
	
	private bool popBrother(GameObject _myselfObj=null){
		return true;
	}
	private bool popOffBrother(GameObject _myselfObj=null){
		return true;
	}
}
