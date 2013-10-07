using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameScript : MonoBehaviour {
	public GameObject selButtonPartsPrefab;
	public GameObject playerBasePrefab;
	public GameObject touchGridObj;
	public const int MESH_W = 10;
	public const int MESH_H = 10;
	private TmSystem _sys;
	private static GameScript mInstance = null;
	public static GameScript instance{ get{ return mInstance; } }
	private Rect mHitRect;
	public Rect hitRect { get { return mHitRect; } }
	private string mSelCommand;
	public string command { get{ return mSelCommand; } set{ mSelCommand = value; } } 
	private GameObject[] mPlList;
	public GameObject[] plList { get { return mPlList; } }
	private GameObject[] mEmList;
	public GameObject[] emList { get { return mEmList; } }
		
// select "Edit > Project Settings > Script Execution Order",
// apply this Script, and change an order before "Default Time".
public class MyParts{
	public MyParts parent = null;
	public List<MyParts> childList = null;
	public string id = "";
	public string name = "";
	public string src = "";
	public string comment = "";
	public string link = "";
	public string text="";
	public GameObject partsObj = null;
}
	
	// データはBOMなしUTF8にしてAssets/xml/下に置く 
	private string _xmlPath = "xml/book0";
	private List<MyParts> _partsList;
	
	void Awake(){
		if(mInstance==null){
			mInstance = this;
		}
		BoxCollider bc = GetComponentInChildren<BoxCollider>();
		if(bc!=null){
			mHitRect = new Rect(bc.transform.position.x,bc.transform.position.y,bc.transform.localScale.x,bc.transform.localScale.y);
			mHitRect.x -= mHitRect.width*0.5f;
			mHitRect.y -= mHitRect.height*0.5f;
		}else{
			Debug.Log("no BoxCollider.");
		}
	}
	
	// Use this for initialization
	void Start () {
		_sys = TmSystem.instance;
		_sys.mw.setHitLayerMask(1<<LayerMask.NameToLayer("layerTouch"));
		
		_partsList = new List<MyParts>();
		XmlDocument doc = new XmlDocument();
		TextAsset data = Instantiate(Resources.Load(_xmlPath,typeof(TextAsset))) as TextAsset;
		doc.LoadXml(data.text);
		XmlNodeList nodeList = doc.SelectNodes("book/page");
		for(int ii = 0; ii < nodeList.Count; ++ii){
			MyParts parts = createPartsFromNode(nodeList[ii],null);
			_partsList.Add(parts);
		}
		createPlayers();
	}
	
	// Update is called once per frame
	void Update () {
		mEmList = GameObject.FindGameObjectsWithTag("tagEm");
		mPlList = GameObject.FindGameObjectsWithTag("tagPl");
	}
	
	public GameObject GetNrarestPl(Vector2 _localPos, Vector2 _range){
		GameObject retGgo = null;
		float minDistSq = _range.sqrMagnitude;
		foreach(GameObject go in mPlList){
			Vector2 goLocPos = new Vector2(go.transform.localPosition.x,go.transform.localPosition.y);
			float tmpDistSq = (goLocPos - _localPos).sqrMagnitude;
			if( tmpDistSq <= minDistSq){
				minDistSq = tmpDistSq;
				retGgo = go;
			}
		}
		return retGgo;
	}
	
	private MyParts createPartsFromNode(XmlNode _node ,MyParts _parent, GameObject _gameObject=null){
		MyParts parts = new MyParts();
		XmlAttributeCollection attr = _node.Attributes;
		if(attr["id"]!=null){       parts.id = attr["id"].Value;		}
		if(attr["name"]!=null){       parts.name = attr["name"].Value;		}
		if(attr["src"]!=null){      parts.src = attr["src"].Value;	}
		if(attr["comment"]!=null){  parts.comment = attr["comment"].Value;	}
		if(attr["link"]!=null){  parts.link = attr["link"].Value;	}
		parts.text = _node.InnerText;
		if(_gameObject==null){
			GameObject[] gos = GameObject.FindGameObjectsWithTag("tagButton");
			foreach(GameObject go in gos){
				if(go.name==parts.name){
					_gameObject = go;
					_gameObject.SendMessage("SM_SetPartsInfo",parts);
					break;
				}
			}
		}
		if(_gameObject==null){
			_gameObject = GameObject.Instantiate(selButtonPartsPrefab) as GameObject;
			_gameObject.SendMessage("SM_SetPartsInfo",parts);
		}
		parts.partsObj = _gameObject;
		if(parts.name !=""){
			_gameObject.name = parts.name;
		}
		if(_parent!=null){
			parts.parent = _parent;
			parts.partsObj.transform.position = parts.parent.partsObj.transform.position+Vector3.forward*0.1f;
			parts.partsObj.transform.rotation = parts.parent.partsObj.transform.rotation;
			parts.partsObj.transform.parent = parts.parent.partsObj.transform;
		}
		Debug.Log(_node.ChildNodes.Count+":"+parts.id);
		if(_node.ChildNodes.Count>0){
			parts.childList = new List<MyParts>();
			for(int ii = 0; ii < _node.ChildNodes.Count; ++ii){
				parts.childList.Add(createPartsFromNode(_node.ChildNodes[ii] ,parts));
			}
		}
		return parts;
	}
	
	private void createPlayers () {
		for(int ii = 0; ii < 10; ++ii){
			int ix = Random.Range(0,MESH_W);
			int iy = Random.Range(0,MESH_H);
			GameObject go = GameObject.Instantiate(playerBasePrefab) as GameObject;
			go.transform.parent = touchGridObj.transform;
			go.transform.localPosition = new Vector3(0.0f,-0.6f,0.0f);
			go.SendMessage("SM_SetDestGridId",new Vector2(ix,iy));
		}
	}
}
