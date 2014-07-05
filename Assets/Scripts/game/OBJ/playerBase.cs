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
		TmSpriteAnim2D anm = GetComponent<TmSpriteAnim2D>();
		anm.SetMeshColor(new Color(0.5f,0.5f,0.6f,1.0f));
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
			transform.rotation = TmMath.LookRotation2D(dirVec,Vector2.up);
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
				transform.rotation = TmMath.LookRotation2D(sizeVec,Vector2.up);
			}
			if(mBulletTimer<=0.0f){
				mBulletTimer += Random.Range(bulletInterval*0.9f,bulletInterval*1.1f);
				GameObject bl = GameObject.Instantiate(bulletPrefab,transform.position,transform.rotation) as GameObject;
				bl.tag = "tagBullet";
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
	
	private Vector3 getClossPoint(Vector3 p0,Vector3 s0, Vector3 p1,float spd){
		Vector3 retPos = p0;
		float t0 = TmMath.CollideTime(p0,s0,p1,spd);
		if(t0>0.0f){
			retPos += s0*t0;
		}
		return retPos;
	}
}

