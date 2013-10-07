using UnityEngine;
using System.Collections;

public class playerBase : MonoBehaviour {
	public float distMinSq=4.0f;
	public float bulletInterval=1.0f;
	public float bulletSpeed=2.0f;
	public float bulleteLifeTime = 2.4f;
	public GameObject bulletPrefab;
	private enum RNO{
		IDLE,
		MOVE,
		EXPLODE,
	}
	private RNO _rno;
	private const float MOVE_WIDTH = 20.0f;
	private const float MOVE_SPEED_MAX = 15.0f;
	private const float BANK_ANGLE_RATE = 500.0f;
	private GameScript mGame;
//	private TmSpriteAnim mAnm;
	private Vector3 mDestLocalPos;
	private float mBulletTimer;
	
	void Awake(){
		_rno = RNO.IDLE;
	}
	
	// Use this for initialization
	void Start () {
		mGame = GameScript.instance;
//		mAnm = GetComponent<TmSpriteAnim>();
//		mDestLocalPos = transform.localPosition;
		mBulletTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(_rno == RNO.MOVE){
			Vector3 vec = mDestLocalPos - transform.localPosition;
			float spd = 0.08f*Time.deltaTime;
			if(vec.magnitude<spd){
				_rno = RNO.IDLE;
				transform.localPosition = mDestLocalPos;
			}
			Vector3 dirVec = vec.normalized * spd;
			transform.localPosition += dirVec;
			setRot(Mathf.Atan2(dirVec.x,dirVec.y));
			return;
		}
		if(mBulletTimer>0){
			mBulletTimer -= Time.deltaTime;
		}
		float distSqMin = distMinSq;
		GameObject em = null;
		GameObject[] ems = mGame.emList;
		foreach(GameObject go in ems){
			float distSqc = (go.transform.position-transform.position).sqrMagnitude;
			if( distSqc < distSqMin){
				distSqMin = distSqc;
				em = go;
			}
		}
		if(em!=null){
			Vector3 sizeVec = em.transform.position - transform.position;
			if(sizeVec.sqrMagnitude < distMinSq){
				Vector3 emWorldSpd = em.GetComponent<enemyBase>().worldSpeed;
				sizeVec = getClossPoint(em.transform.position,emWorldSpd,transform.position,bulletSpeed) - transform.position;
				setRot(Mathf.Atan2(sizeVec.x,sizeVec.y));
			}
			if(mBulletTimer<=0.0f){
				mBulletTimer += Random.Range(bulletInterval*0.9f,bulletInterval*1.1f);
				GameObject bl = GameObject.Instantiate(bulletPrefab,transform.position,transform.rotation) as GameObject;
				bl.transform.parent = transform.parent;
				bulletParam param = new bulletParam(bulletSpeed,1.0f,bulleteLifeTime);
				bl.SendMessage("SM_SetBulletSpeed",param);
			}
		}
	}

	void OnTriggerEnter(Collider _coll){
		Debug.Log("OnCollisionEnter");
//		_coll.gameObject.SendMessage("reset");
//		Debug.Log("Hit!"+LayerMask.LayerToName(_coll.gameObject.layer));
		if(_coll.gameObject.layer==LayerMask.NameToLayer("layerDropItem")){
			Destroy(_coll.gameObject);
		}
	}
	
	//------------------------------------------------------
	public void SM_SetDestLocalPos(Vector3 _pos){
		mDestLocalPos = _pos;
		_rno = RNO.MOVE;
	}
	public void SM_SetDestGridId(Vector2 _posId){
		Vector3 gridScale = new Vector3(1.0f/GameScript.MESH_W,1.0f/GameScript.MESH_H,0.0f);
		Vector3 pos = Vector3.Scale(new Vector3(_posId.x,_posId.y),gridScale);
		pos -= (Vector3.one - gridScale)*0.5f;
		pos.z = 0.0f;
		SM_SetDestLocalPos(pos);
	}
	//------------------------------------------------------
	
	private void setRot(float ang){
		Quaternion tmpRot = Quaternion.identity;
		tmpRot.eulerAngles = new Vector3(0.0f,0.0f,-ang*180.0f/Mathf.PI);
		transform.rotation = tmpRot;
//		ringArrowObj.transform.rotation = sttRot * tmpRot;
	}
	
	private Vector3 getClossPoint(Vector3 p0,Vector3 s0, Vector3 p1,float spd){
		Vector3 retPos = p0;
#if true
		// p1がp0に到達するのにかかる時間t1 
		float t1 = (p1-p0).magnitude/spd;
		retPos += s0*t1*0.5f;
#else
		Vector2 p1NearestPos = TmMath.nearestPointOnLine(p0,p0+s0,p1,false);
		// p0がp1NearestPosに到達するのにかかる時間t0 
		float t0 = (p1NearestPos-new Vector2(p0.x,p0.y)).magnitude/s0.magnitude;
		// p1がspdの速度でp1NearestPosに到達するのにかかる時間t1 
		float t1 = (p1NearestPos-new Vector2(p1.x,p1.y)).magnitude/spd;
		if(t0>t1){
			retPos = new Vector3(p1NearestPos.x,p1NearestPos.y,p1.z);
		}else{
			retPos += s0*(t1-t0)/t1;
		}
#endif
		return retPos;
	}
}

