/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
using UnityEngine;
using System.Collections;

public class MP_Edge : MonoBehaviour
{

	float _gamma = 0.5f;
    

    public float  gamma
    {
        get { return _gamma; }
        set { _gamma = value; }
    }

    /// <summary>
    /// alphas is a blend value [0,255]
    /// </summary>
    byte[] alphas = new byte[256];
	Texture2D texture;
	public bool draw_line = false;


	/// <summary>
    /// create a texture to show it
    /// </summary>
    void rebulid_alpha ()
	{
		create_cue ();
		var cols = texture.GetPixels32 (0);

		for (var i = 0; i < cols.Length; ++i) {
			byte xx = alphas [i];		
			Color32 cc = new Color32 (0, 0, 0, xx);
			cols [i] = cc;
		}
		texture.SetPixels32 (cols, 0);

		texture.Apply (false);


	}

	public GameObject line_pre;

	/// <summary>
    /// show lines
    /// </summary>
    void MadeGammaCue ()
	{

 
   
		GameObject l = Instantiate (line_pre) as GameObject;
		l.transform.SetParent (transform);
		l.name = "Edge_cue";

		LineRenderer lr = l.GetComponent<LineRenderer> ();
		if (lr) {
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.startWidth = lr.endWidth = 0.05f;
            lr.positionCount = alphas.Length;
			for (var i = 0; i < alphas.Length; i++) {
				Vector3 new_pt = new Vector3 (i/255f, alphas [i] /255f, 0);
                Vector3 ss_pt = new Vector3(new_pt.x , new_pt.y , 0);
                Vector3 new_pt2 = Vector3.zero;
                MP_ScreenGrid ms = transform.parent.GetComponent<MP_ScreenGrid>();
                if (ms) new_pt2 = ms.getWorldPt(ss_pt);
                new_pt2.z = -0.1f;
                lr.SetPosition (i, new_pt2);	
			}
		}
		 
	}

    /// <summary>
    /// create curve with besizer , if you can found better blend curve , you can change it.
    /// </summary>
    void create_cue ()
	{
		
		Vector3 p1 = Vector3.zero;
		Vector3 p2 = new Vector3 (0, 1, 0);
		Vector3 p3 = new Vector3 (0.5f, gamma, 0);
		Vector3 ct0 = (p2 - p3) * 1.0f;
		Vector3 ct1 = (p1 - p3) * 1.0f;

		MP_Bezier bs = new MP_Bezier (p1, ct0, ct1, p2, 256);	
		for (var i = 0; i < alphas.Length; ++i) {
			float s = i * 1.0f / (alphas.Length - 1);
			alphas [i] = (byte)(255f * bs.GetPos (s).y);
			//print (alphas [i]);
		}


	}
	

	void Awake ()
	{
		texture = new Texture2D (256, 1);
		texture.wrapMode = TextureWrapMode.Clamp;
		 
		rebulid_alpha ();
		Renderer r = GetComponent<Renderer> ();
		if (r) {
			r.material.mainTexture = texture;
		}
	}

	/// <summary>
    /// update then into new mesh
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="uvs"></param>
    /// <param name="indices"></param>
    public void UpdateMesh (Vector3[] vertices, Vector2[] uvs, int[] indices)
	{
		Mesh msh = new Mesh ();
		msh.name = "EdgeGrid";
		msh.Clear ();
	 
		msh.vertices = vertices;
		msh.uv = uvs;
		msh.triangles = indices;

		msh.RecalculateNormals ();
		msh.RecalculateBounds ();	 

		MeshFilter filter = GetComponent (typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
	}

 

    /// <summary>
    /// update gamma 
    /// </summary>
    /// <param name="g_gamma"></param>
    /// <param name="show_line"></param>
    public void UpdateGamma(float g_gamma,bool show_line)
    {
        gamma = Mathf.Clamp01(g_gamma);
        rebulid_alpha();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        if (show_line)
        {
            MadeGammaCue();            
        }

        
         
    }
}
