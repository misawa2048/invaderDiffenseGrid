using UnityEngine;
using System.Collections;

public class lineOne : MonoBehaviour {
	private Mesh mMesh;
	private Vector3[] mVertices;
	
	// Use this for initialization
	void Start () {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if((meshFilter!=null)&&(meshFilter.mesh!=null)){
			mMesh = TmUtils.CreateLineCircle(2);
			mVertices = mMesh.vertices.Clone() as Vector3[];
			meshFilter.mesh = meshFilter.sharedMesh = mMesh;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	//------------------------------------------------------
	void SM_SetSttPos(Vector2 _pos){ setMeshVtx(0, _pos); }
	void SM_SetEndPos(Vector2 _pos){ setMeshVtx(1, _pos); }
	//------------------------------------------------------

	
	bool setMeshVtx(int _idx, Vector3 _pos){
		bool ret = false;
		if((mMesh!=null)&&(mMesh.vertices.Length >=(_idx+1))){
			mVertices[_idx] = _pos;
			mMesh.vertices = mVertices;
			mMesh.RecalculateBounds(); 
			ret = true;
		}
		return ret;
	}
	
	public static Mesh createLineStrip(int _verts){
		Vector3[] vertices = new Vector3[(_verts)];
		int[] triangles = new int[(((_verts))/3+1)*3];
		Vector2[] uv = new Vector2[(_verts)];
		Color[] colors = new Color[(_verts)];
		Vector3[] normals = new Vector3[(_verts)];

		int cnt = 0;
		for(int ii = 0; ii < _verts; ++ii){
			float fx = Mathf.Cos(Mathf.PI*2.0f * ((float)ii / (float)_verts))*0.5f;
			float fy = Mathf.Sin(Mathf.PI*2.0f * ((float)ii / (float)_verts))*0.5f;
			vertices[cnt] = new Vector3(fx,fy,0.0f);
			triangles[cnt] = cnt;
			uv[cnt] = new Vector2(vertices[cnt].x+0.5f,vertices[cnt].y+0.5f);
			colors[cnt] = new Color(0.5f,0.5f,0.5f,1.0f);
			cnt++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize();
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.LineStrip,0);
		return mesh;
	}
}
