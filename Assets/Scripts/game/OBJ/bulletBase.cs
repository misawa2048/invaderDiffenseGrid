using UnityEngine;
using System.Collections;

	public class bulletParam{
		public bulletParam(float _speed, float _power, float _deleteTime){
			speed = _speed;
			power = _power;
			deleteTime = _deleteTime;
		}
		public float speed;
		public float power;
		public float deleteTime;
	}
public class bulletBase : MonoBehaviour {
	public enum TYPE{
		PL,
		EM,
	};

	private const float ALPHA_TIME = 0.1f;
	private TmSpriteAnim2D _anm;
	private Color mMeshColor;
	private float mDeleteTimer;
	private float mBulletSpeed=2.0f;
	

	// Use this for initialization
	void Start () {
		_anm = GetComponent<TmSpriteAnim2D>();
		mMeshColor = new Color(1.0f,1.0f,1.0f,1.0f);
		_anm.SetColor(mMeshColor);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.up*mBulletSpeed*Time.deltaTime);
		mDeleteTimer -= Time.deltaTime;
		float a = mDeleteTimer/ALPHA_TIME;
		mMeshColor.a = (a < 1.0f) ? a : 1.0f;
		_anm.SetColor(mMeshColor);
		if(mDeleteTimer <= 0.0f){
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider _coll){
		Destroy(gameObject);
	}
	
	//------------------------------------------------------
	public void SM_SetBulletSpeed(bulletParam _param){
		mBulletSpeed = _param.speed;
		mDeleteTimer = _param.deleteTime;
	}
	//------------------------------------------------------
}
