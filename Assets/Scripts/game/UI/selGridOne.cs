using UnityEngine;
using System.Collections;

public class selGridOne : MonoBehaviour {
	public enum TYPE{
		OFF,
		ON,
		BLINK_OFF,
		BLINK,
		FADE_OUT,
		FADE_IN,
		FLASH_OFF,
		FLASH_BLINK
	};
	
	private TYPE mType;
	private TYPE mTypeOld;
	private int[] mRno;
	private float mTimer;
	private float mAlpha;
	private float mDelay;
	private Mesh mMesh;
	
	void Awake(){
		mType = mTypeOld = TYPE.OFF;
		mRno = new int[2];
		mTimer = 0.0f;
		mAlpha = 0.0f;
		mDelay = 0.0f;
		mTimer += Time.deltaTime;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if((meshFilter!=null)&&(meshFilter.mesh!=null)){
			mMesh = meshFilter.mesh;
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(mTypeOld!=mType){
			mRno[0] = mRno[1] = 0;
		}
		switch(mType){
															
			case TYPE.OFF:          r00MoveTypeOff();         break;
			case TYPE.ON:           r00MoveTypeOn();          break;
			case TYPE.BLINK_OFF:    r00MoveTypeBlinkOff();    break;
			case TYPE.BLINK:        r00MoveTypeBlink();       break;
			case TYPE.FADE_OUT:     r00MoveTypeFadeOut();     break;
			case TYPE.FADE_IN:      r00MoveTypeFadeIn();      break;
			case TYPE.FLASH_OFF:    r00MoveTypeFlashOff();    break;
			case TYPE.FLASH_BLINK:  r00MoveTypeFlashBlink();  break;
		}
		mTypeOld = mType;
	}
	
	//------------------------------------------------------
	public void SM_SetType(TYPE _selType){
		mType = _selType;
	}
	public void SM_SetDelay(float _time){
		mDelay = _time;
	}
	public void SM_SetDestLocalPos(Vector2 _pos){
		_pos += Vector2.one*0.5f;
		_pos.Scale(new Vector2(GameScript.MESH_W,GameScript.MESH_H));
		Vector2 selPosId = new Vector2(Mathf.Floor(_pos.x),Mathf.Floor(_pos.y));
		SM_SetDestGridId(selPosId);
	}
	public void SM_SetDestGridId(Vector2 _posId){
		Vector3 gridCood = _posId + Vector2.one * 0.5f;
		gridCood.Scale(new Vector2(1.0f/GameScript.MESH_W,1.0f/GameScript.MESH_H));
		gridCood.x -= 0.5f;
		gridCood.y -= 0.5f;
		transform.localPosition = gridCood;
	}
	//------------------------------------------------------
	
	//-----------------------------------------------------------
	bool r00MoveTypeOff(){
		gameObject.renderer.enabled = false;
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeOn(){
		gameObject.renderer.enabled = true;
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeBlinkOff(){
		switch(mRno[0]){
		case 0:
			if(mTypeOld==TYPE.OFF){
				mRno[0]=2;
				mRno[1]=0; // カウンタとして使用 
			}else{
				mRno[0]=1;
				mRno[1]=0; // カウンタとして使用 
				gameObject.renderer.enabled = true;
				mTimer = 0.0f;
				mAlpha = 0.5f;
				mMesh = TmUtils.SetMeshColor(mMesh,new Color(1.0f,1.0f,0.0f,mAlpha));
			}
			break;
		case 1:
			mTimer += Time.deltaTime;
			if(mTimer > 0.1f){
				mTimer -= 0.1f;
				mRno[1]++;
				gameObject.renderer.enabled = (mRno[1]%2 == 0);
				if(mRno[1] >= 8){
					gameObject.renderer.enabled = false;
					mRno[0] = 2;
					mRno[1] = 0;
				}
			}
			break;
		}
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeBlink(){
		switch(mRno[0]){
		case 0:
			mRno[0]=1;
			gameObject.renderer.enabled = true;
			mTimer = 0.0f;
			break;
		case 1:
			mTimer += Time.deltaTime;
			mAlpha = (Mathf.Sin(mTimer*4.0f)+1.0f)*0.4f+0.2f;
			mMesh = TmUtils.SetMeshColor(mMesh,new Color(1.0f,0.0f,0.0f,mAlpha));
			break;
		}
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeFadeOut(){
		gameObject.renderer.enabled = false;
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeFadeIn(){
		gameObject.renderer.enabled = true;
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeFlashOff(){
		gameObject.renderer.enabled = false;
		return true;
	}
	//-----------------------------------------------------------
	bool r00MoveTypeFlashBlink(){
		const float FLASH_BLINK_TIME = 2.0f;
		const float FLASH_BLINK_ALPHA_TIME = 1.5f;
		switch(mRno[0]){
		case 0:
			mDelay -= Time.deltaTime;
			if(mDelay <= 0.0f){
				mRno[0]=1;
				mTimer = FLASH_BLINK_TIME + mDelay;
				gameObject.renderer.enabled = true;
			}else{
				gameObject.renderer.enabled = false;
			}
			break;
		case 1:
			mTimer -= Time.deltaTime;
			if(mTimer <= 0.0f){
				gameObject.renderer.enabled = true;
				mTimer += FLASH_BLINK_TIME;
			}
			mAlpha = (mTimer-(FLASH_BLINK_TIME-FLASH_BLINK_ALPHA_TIME))/FLASH_BLINK_ALPHA_TIME;
			if(mAlpha > 0.0f){
				mMesh = TmUtils.SetMeshColor(mMesh,new Color(1.0f,0.0f,0.0f,mAlpha));
			}else{
				gameObject.renderer.enabled = false;
			}
			break;
		}
		return true;
	}
}
