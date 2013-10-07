using UnityEngine;
using System.Collections;

public class bgStars : MonoBehaviour {
	public GameObject earthObj;
	private const int NUM_STARS = 1000;
	private Vector3 mDefScale;
	
	// Use this for initialization
	void Start () {
//		createStars();
		mDefScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = mDefScale + earthObj.transform.localScale*5.0f;
	}
	
	void createStars(){
		Vector3[] vertices = new Vector3[NUM_STARS];
		int[] triangles = new int[(NUM_STARS/3+1)*3];
		Vector2[] uv = new Vector2[NUM_STARS];
		Color[] colors = new Color[NUM_STARS];
		Vector3[] normals = new Vector3[NUM_STARS];

		for(int ii = 0; ii < NUM_STARS; ++ii){
			vertices[ii] = new Vector3(Random.value-0.5f,Random.value-0.5f,0.0f);
			triangles[ii] = ii;
			uv[ii] = vertices[ii];
			colors[ii] = new Color(Random.value*0.8f+0.2f,Random.value*0.8f+0.2f,Random.value*0.8f+0.2f,Random.value*0.6f+0.2f);
		}
		
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		meshFilter.sharedMesh = meshFilter.mesh = mesh;
		mesh.name = "Points"+NUM_STARS.ToString();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.SetIndices(mesh.GetIndices(0),MeshTopology.Points,0);
		mesh.RecalculateNormals ();	// 法線の再計算
		mesh.RecalculateBounds ();	// バウンディングボリュームの再計算
		mesh.Optimize ();
		
//		AssetDatabase.CreateAsset (mesh, "Assets/" + mesh.name + ".asset");
//		AssetDatabase.SaveAssets ();
	}
	
}
