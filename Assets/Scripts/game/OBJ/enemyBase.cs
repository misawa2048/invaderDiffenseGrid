using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyBase : MonoBehaviour {
	enum RNO{
		IDLE,
		MOVE,
		EXPLODE,
	}
	public GameObject dropItemPrefab;
	private List <Vector2> mPosList;
	private RNO _rno;
	private TmSpriteAnim2D mAnm;
	private int mDistId;
	private float mLocalSpeedScalar;
	private Vector2 mLocalSpeed;
	private Vector3 mLocalPosOld;
	private Vector3 mWorldSpeedVec;
	public Vector3 worldSpeed { get{ return mWorldSpeedVec; } }
		
	// Use this for initialization
	void Start () {
		if(mPosList==null) return;
		
		_rno = RNO.MOVE;
		mAnm = GetComponent<TmSpriteAnim2D>();
		mAnm.SetMeshColor(new Color(Random.Range(0.3f,0.9f),Random.Range(0.3f,0.9f),Random.Range(0.3f,0.8f),1.0f));
		mDistId = 0;
		Vector2 pos = mPosList[mDistId];
		pos.y += (1.0f/(float)GameScript.MESH_H)*3.0f;
		transform.localPosition = mLocalPosOld = pos;
	}
	
	// Update is called once per frame
	void Update () {
		if(mPosList==null) return;

		Vector3 posOld = transform.position;
		mLocalPosOld = transform.localPosition;
		switch(_rno){
		case RNO.IDLE:
			break;
		case RNO.MOVE:
			if(mDistId < mPosList.Count){
				Vector2 nowPos = transform.localPosition;
				Vector2 nextPos = mPosList[mDistId];
				float spd = mLocalSpeedScalar*Time.deltaTime;
				if((nowPos-nextPos).magnitude < spd*2.0f){
					mDistId++;
				}else{
					Vector2 movePos = transform.localPosition;
					mLocalSpeed = (nextPos-nowPos).normalized * mLocalSpeedScalar;
					movePos += mLocalSpeed*Time.deltaTime;
					transform.localPosition = movePos;
					transform.rotation = Quaternion.Lerp(transform.rotation, TmMath.LookRotation2D(mLocalSpeed,Vector2.up),0.1f);
				}
			}else{
				Destroy(gameObject);
			}
			break;
		case RNO.EXPLODE:
			if((mAnm.animFlag & 1)!=0){
				Destroy(gameObject);
			}
			break;
		}
		mWorldSpeedVec = (transform.position - posOld)/Time.deltaTime;
	}

	void OnTriggerEnter(Collider _coll){
		_rno = RNO.EXPLODE;
		mAnm.PlayAnimation("explode");
		if(Random.value<0.1f){
			GameObject itemObj = GameObject.Instantiate(dropItemPrefab,transform.position,Quaternion.identity) as GameObject;
			itemObj.transform.parent = transform.parent;
			itemObj.SendMessage("SM_setSpeed",mLocalSpeed);
		}
	}
	
	public void SM_SetRoutePosList(List <Vector2> _gridPosList){
		mPosList = new List<Vector2>(_gridPosList);
		Vector2 pos = _gridPosList[_gridPosList.Count-1];
		pos.x = 0.0f;
		pos.y -= (1.0f/(float)GameScript.MESH_H);
		mPosList.Add(pos);
	}
	public void SM_SetMoveSpeed(float _spd){
		mLocalSpeedScalar = _spd;
	}

}