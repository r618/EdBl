/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// screen gird class 
/// </summary>
public partial class MP_ScreenGrid : MonoBehaviour
{

	/// <summary>
    /// how the gird move ?
    /// </summary>
    public enum MOVE_WAY
	{
		SINGLE = 0,    // only move ctrol point
		FOUR_CONNER = 1,  //  move conners point, useful when global change position
        BY_COL = 2,     // by col 
		BY_ROW = 3,     // by row
        SINGLE_SMOOTH = 4, // move ctrol point and near points
    } //BY_EDGE = 4
	;

	/// <summary>
    /// blend area edge direction
    /// </summary>
    enum EDGE_WAY 
	{
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3,
	};


    /// <summary>
    /// EDGE class
    /// </summary>
    class EDGE
	{
		public bool isOpen = false; // open or not 
		public bool isUpdate = true; // change will update it
		public EDGE_WAY edge_type = EDGE_WAY.LEFT; //direction
	};
	EDGE [] edge_sides = new EDGE [4] ;


    /// <summary>
    /// move_way
    /// </summary>
    private MOVE_WAY move_way = MOVE_WAY.SINGLE;

	private MP_COL_ROW_PTS grid_array = new MP_COL_ROW_PTS ();

    /// <summary>
    /// will be update by SetGridCount
    /// </summary>
    private int COL = 16;
	private  int ROW = 12;

    /// <summary>
    /// four conners index for FOUR_CONNER move way
    /// </summary>
    private int[,] CONNERS_INDEXS = 
		new int[,] { { 0, 0 }, { 0, 12 }, { 16, 12 }, { 16, 0 } };

	 

	public GameObject line_pre;
	public GameObject point_pre;
	public GameObject edge_pre;
    public GameObject grid_pre;

    public bool showGrid = false;
    

    /// <summary>
    /// sub screen inside whole screen
    /// </summary>
    public int screen_width =512;
    public int screen_height=384;

    /// <summary>
    /// screen_id start from 0 to 4
    /// </summary>
    public int screen_id = 0;
    public Color GirdColor = Color.blue;


    /// <summary>
    /// rgb base shift
    /// </summary>
    Vector3 _base_shift_rgb = Vector3.zero;



    /// <summary>
    /// use for Black Level and Brightness Compensation
    /// https://resolume.com/manual/start?id=en/r4/output
    /// </summary>
    Vector3 _base_compensation_rgb = Vector3.zero;


    /// <summary>
    /// ctrol point is focus in this screen; 
    /// </summary>    
    private bool _isFocus;

    public bool isFocus
    {
        get { return _isFocus; }
        set
        {
           _isFocus = value;

        }
    }

    Vector2 move_shift = Vector2.zero;
    
    public void SetGridCount(int col, int row)
    {
        COL = col;
        ROW = row;
    } 

    /// <summary>
    /// translate grid point position [0,1] to camera world point.
    /// and base parent position
    /// </summary>
    /// <param name="pt_o1"></param>
    /// <returns></returns>
    public Vector3 getWorldPt (Vector2 pt_o1)
	{
		Vector3 ss_pt = new Vector3 (pt_o1.x * screen_width, pt_o1.y * screen_height, 0);        
        Vector3 new_pt = screen_cam.ScreenToWorldPoint (ss_pt);
        new_pt.z = 0f;
		return new_pt;
	}


	/// <summary>
    /// create a blend area edge
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Transform made_a_edge(string name)
	{
		Transform EdgeTrm = transform.Find(name);

		if(EdgeTrm==null)
		{
			GameObject  go = Instantiate(edge_pre) as GameObject;
			go.name = name;
			go.transform.SetParent(transform);
			EdgeTrm = go.transform;


            float gamma = PlayerPrefs.GetFloat(getGammaKeyStr(), 0.31f);
            update_gamma(gamma, false);

        }
		return EdgeTrm;
	}


    /// <summary>
    /// copy from global iblend_count
    /// </summary>
    public int blend_count = 4;


    /// <summary>
    /// made edge up 
    /// </summary>
    void made_edge_up()
	{


		List<Vector2> veclist = new List<Vector2> (); 
		int index = 0;

		for (int i = 0; i <  grid_array.pCol.Count; i++) { 
			
			for (int j = grid_array.pCol [i].pRow.Count-blend_count; j <  grid_array.pCol [i].pRow.Count; j++) {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                pts.index = index;
                index++;
                pts.isEdgeIncluded = true;
                veclist.Add(pts.pt);
            }
		}

		int blent_vec_count = veclist.Count;
		Vector3[] vertices = new Vector3[blent_vec_count];		 
		Vector2[] uvs = new Vector2[blent_vec_count];	
		for (int i = 0; i < veclist.Count; i++) {		
			Vector3 new_pt = getWorldPt (veclist [i]); 		 
			vertices [i] = new_pt; 
		}
		int tui = 0;
		for (int i = 0; i <  grid_array.pCol.Count; i++) { 
			for (int j = grid_array.pCol [i].pRow.Count-blend_count; j <  grid_array.pCol [i].pRow.Count; j++) {					

				float v = (j-(grid_array.pCol [i].pRow.Count-blend_count)) *1.0f / (blend_count-1);
				//print(v);
				uvs [tui] = new Vector2(v,0);
				tui++;
			}
		}


		List<int> vecIndecs = new List<int> (); 
		for (int i = 0; i <  grid_array.pCol.Count-1; i++) { 
			int next_i = i + 1;
			for (int j = grid_array.pCol [i].pRow.Count-blend_count; j <  grid_array.pCol [i].pRow.Count-1; j++) {					
				MP_MY_PTS p1, p2, p3, p4;
				int next_j = j + 1;
				p1 = grid_array.pCol [i].pRow [j];
				p2 = grid_array.pCol [next_i].pRow [j];
				p3 = grid_array.pCol [i].pRow [next_j];
				p4 = grid_array.pCol [next_i].pRow [next_j];

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p3.index);
				vecIndecs.Add (p4.index);

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p4.index);
				vecIndecs.Add (p2.index);
			}
		} 

		int[] indices = vecIndecs.ToArray(); 

		Transform EdgeTrm =made_a_edge("Edge_up");

		if(EdgeTrm)
		{
			MP_Edge esc = EdgeTrm.GetComponent<MP_Edge>();
			if(esc)
			{
				//print("asdasdasd");
				esc.UpdateMesh(vertices,uvs,indices);
			}
		}
	}

    /// <summary>
    /// made edge down 
    /// </summary>
    void made_edge_down()
	{


		List<Vector2> veclist = new List<Vector2> (); 
		int index = 0;
		for (int i = 0; i <  grid_array.pCol.Count; i++) { 
			for (int j = 0; j < blend_count; j++) {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                pts.index = index;
                index++;
                pts.isEdgeIncluded = true;
                veclist.Add(pts.pt);
            }
		}

		int blent_vec_count = veclist.Count;
		Vector3[] vertices = new Vector3[blent_vec_count];		 
		Vector2[] uvs = new Vector2[blent_vec_count];	
		for (int i = 0; i < veclist.Count; i++) {		
			Vector3 new_pt = getWorldPt (veclist [i]); 		 
			vertices [i] = new_pt; 
		}
		int tui = 0;
		for (int i = 0; i <  grid_array.pCol.Count; i++) { 
			for (int j = 0; j < blend_count; j++) {		

				float v =  (blend_count-1-j) *1.0f / (blend_count-1);
				uvs [tui] = new Vector2(v,0);
				tui++;
			}
		}


		List<int> vecIndecs = new List<int> (); 
		for (int i = 0; i <  grid_array.pCol.Count-1; i++) { 
			int next_i = i + 1;
			for (int j = 0; j < blend_count-1; j++) {		
				MP_MY_PTS p1, p2, p3, p4;
				int next_j = j + 1;
				p1 = grid_array.pCol [i].pRow [j];
				p2 = grid_array.pCol [next_i].pRow [j];
				p3 = grid_array.pCol [i].pRow [next_j];
				p4 = grid_array.pCol [next_i].pRow [next_j];

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p3.index);
				vecIndecs.Add (p4.index);

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p4.index);
				vecIndecs.Add (p2.index);
			}
		} 

		int[] indices = vecIndecs.ToArray(); 

		Transform EdgeTrm =made_a_edge("Edge_down");

		if(EdgeTrm)
		{
			MP_Edge esc = EdgeTrm.GetComponent<MP_Edge>();
			if(esc)
			{
				//print("asdasdasd");
				esc.UpdateMesh(vertices,uvs,indices);
			}
		}
	}

    /// <summary>
    /// made edge right 
    /// </summary>
    void made_edge_right()
	{
		List<Vector2> veclist = new List<Vector2> (); 
		int index = 0;
		for (int i = 0; i < blend_count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                pts.index = index;
                index++;
                pts.isEdgeIncluded = true;
                veclist.Add(pts.pt);
            }
		}

		int blent_vec_count = veclist.Count;
		Vector3[] vertices = new Vector3[blent_vec_count];		 
		Vector2[] uvs = new Vector2[blent_vec_count];	
		for (int i = 0; i < veclist.Count; i++) {		
			Vector3 new_pt = getWorldPt (veclist [i]); 		 
			vertices [i] = new_pt; 
		}
		int tui = 0;
		for (int i = 0; i < blend_count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {		

				float u =  (blend_count-1-i) *1.0f / (blend_count-1);
				uvs [tui] = new Vector2(u,0);
				tui++;
			}
		}


		List<int> vecIndecs = new List<int> (); 
		for (int i = 0; i < blend_count-1; i++) { 
			int next_i = i + 1;
			for (int j = 0; j < grid_array.pCol [i].pRow.Count-1; j++) {		
				MP_MY_PTS p1, p2, p3, p4;
				int next_j = j + 1;
				p1 = grid_array.pCol [i].pRow [j];
				p2 = grid_array.pCol [next_i].pRow [j];
				p3 = grid_array.pCol [i].pRow [next_j];
				p4 = grid_array.pCol [next_i].pRow [next_j];

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p3.index);
				vecIndecs.Add (p4.index);

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p4.index);
				vecIndecs.Add (p2.index);
			}
		} 

		int[] indices = vecIndecs.ToArray(); 

		Transform EdgeTrm =made_a_edge("Edge_right");

		if(EdgeTrm)
		{
			MP_Edge esc = EdgeTrm.GetComponent<MP_Edge>();
			if(esc)
			{
				//print("asdasdasd");
				esc.UpdateMesh(vertices,uvs,indices);
			}
		}
	}


    /// <summary>
    /// clear isEdgeIncluded
    /// </summary>
    void made_edge_clear_left()
    {
        for (int i = (grid_array.pCol.Count - blend_count); i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];               
                pts.isEdgeIncluded = false;                
            }
        }
    }

    void made_edge_clear_down()
    {
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < blend_count; j++)
            {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];              
                pts.isEdgeIncluded = false;
            }
        }
    }
    void made_edge_clear_up()
    {
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {

            for (int j = grid_array.pCol[i].pRow.Count - blend_count; j < grid_array.pCol[i].pRow.Count; j++)
            {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];               
                pts.isEdgeIncluded = false;
            }
        }

    }
    void made_edge_clear_right()
    {
        for (int i = 0; i < blend_count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                pts.isEdgeIncluded = false;
            }
        }
    }

    /// <summary>
    /// made edge left
    /// </summary>
    void made_edge_left()
	{
		List<Vector2> veclist = new List<Vector2> (); 
		int index = 0;
		for (int i = (grid_array.pCol.Count-blend_count); i < grid_array.pCol.Count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                pts.index = index;
				index++;
                pts.isEdgeIncluded = true;
				veclist.Add (pts.pt);
			}
		}

		int blent_vec_count = veclist.Count;
		Vector3[] vertices = new Vector3[blent_vec_count];		 
		Vector2[] uvs = new Vector2[blent_vec_count];	
		for (int i = 0; i < veclist.Count; i++) {		
			Vector3 new_pt = getWorldPt (veclist [i]); 		 
			vertices [i] = new_pt; 
		}
		int tui = 0;
		for (int i = (grid_array.pCol.Count-blend_count); i < grid_array.pCol.Count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {		

				float u = ( i -  (grid_array.pCol.Count-blend_count))*1.0f / (blend_count-1);
				uvs [tui] = new Vector2(u,0);
				tui++;
			}
		}


		List<int> vecIndecs = new List<int> (); 
		for (int i = (grid_array.pCol.Count-blend_count); i < grid_array.pCol.Count-1; i++) { 
			int next_i = i + 1;
			for (int j = 0; j < grid_array.pCol [i].pRow.Count-1; j++) {		
				MP_MY_PTS p1, p2, p3, p4;
				int next_j = j + 1;
				p1 = grid_array.pCol [i].pRow [j];
				p2 = grid_array.pCol [next_i].pRow [j];
				p3 = grid_array.pCol [i].pRow [next_j];
				p4 = grid_array.pCol [next_i].pRow [next_j];

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p3.index);
				vecIndecs.Add (p4.index);

				vecIndecs.Add (p1.index);
				vecIndecs.Add (p4.index);
				vecIndecs.Add (p2.index);
			}
		} 

		int[] indices = vecIndecs.ToArray(); 

		Transform EdgeTrm =made_a_edge("Edge_left");

		if(EdgeTrm)
		{
			MP_Edge esc = EdgeTrm.GetComponent<MP_Edge>();
			if(esc)
			{
				//print("asdasdasd");

				esc.UpdateMesh(vertices,uvs,indices);
			}
		}
	}

    /// <summary>
    /// delete edge by name 
    /// </summary>
    void delete_edge(string name)
	{
		Transform EdgeTrm = transform.Find(name);

		if(EdgeTrm)
		{
			Destroy(EdgeTrm.gameObject);
		}
	}


    /// <summary>
    /// Show Or Hide Grid , hide will delete grid
    /// </summary>
    /// <param name="g_showGrid"></param>
    public void ShowOrHideGrid(bool g_showGrid)
    {
        showGrid = g_showGrid;
        if (!showGrid)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform cld = transform.GetChild(i);
                if(cld.name=="holder")   Destroy(cld.gameObject);
                if (cld.name == "ctrlPt") Destroy(cld.gameObject);
            }
        }
        UpdateAll();
    }

    /// <summary>
    /// detect edge isOpen and create it or delete it
    /// </summary>
    void UpdateEdge()
    {
     
        for (int ti = 0; ti < edge_sides.Length; ti++)
        {

            EDGE e = edge_sides[ti];
            //if (!e.isUpdate) continue;
            if (!e.isOpen)
            {
                if (e.edge_type == EDGE_WAY.LEFT)
                {
                    delete_edge("Edge_left");
                    made_edge_clear_left();

                }
                else if (e.edge_type == EDGE_WAY.RIGHT)
                {
                    delete_edge("Edge_right");
                    made_edge_clear_right();

                }
                else if (e.edge_type == EDGE_WAY.UP)
                {
                    delete_edge("Edge_up");
                    made_edge_clear_up();
                }
                else if (e.edge_type == EDGE_WAY.DOWN)
                {
                    delete_edge("Edge_down");
                    made_edge_clear_down();
                }
            }
        }
        for (int ti = 0; ti < edge_sides.Length; ti++)
        {

            EDGE e = edge_sides[ti];
            //if (!e.isUpdate) continue;
            if (e.isOpen)
            {
                if (e.edge_type == EDGE_WAY.LEFT)
                {
                    made_edge_left();
                }
                else if (e.edge_type == EDGE_WAY.RIGHT)
                {
                    made_edge_right();
                }
                else if (e.edge_type == EDGE_WAY.UP)
                {
                    made_edge_up();
                }
                else if (e.edge_type == EDGE_WAY.DOWN)
                {
                    made_edge_down();
                }
            }
        }
        for (int ti = 0; ti < edge_sides.Length; ti++)
        {

            EDGE e = edge_sides[ti];           
            e.isUpdate = false;
        }
    }


	/// <summary>
    /// update array into a new mesh
    /// </summary>
    public void UpdateArray2Mesh ()
	{
		if (grid_array.pCol.Count < 2)
			return;
		if (grid_array.pCol [0].pRow.Count < 2)
			return;
		 
        
		List<Vector2> veclist = new List<Vector2> ();
        List<Vector2> uvlist = new List<Vector2>();
       
        List<MP_MY_PTS> vecPtsList = new List<MP_MY_PTS>();

        int index = 0;
		for (int i = 0; i < grid_array.pCol.Count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {				
                MP_MY_PTS pts = grid_array.pCol[i].pRow[j];
                
                    pts.index = index;
                    index++;
                    veclist.Add(pts.pt);
                    uvlist.Add(pts.uv);
                
			}
		}


        List<int> vecIndecs = new List<int>();
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {               

                int next_i = i + 1;
                int next_j = j + 1;
                if (next_i < grid_array.pCol.Count && next_j < grid_array.pCol[i].pRow.Count)
                {
                    MP_MY_PTS p1, p2, p3, p4;
                    p1 = grid_array.pCol[i].pRow[j];
                    p2 = grid_array.pCol[next_i].pRow[j];
                    p3 = grid_array.pCol[i].pRow[next_j];
                    p4 = grid_array.pCol[next_i].pRow[next_j];

                    if (!p1.isEdgeIncluded || !p2.isEdgeIncluded || !p3.isEdgeIncluded || !p4.isEdgeIncluded)
                    {
                        vecIndecs.Add(p1.index);
                        vecIndecs.Add(p3.index);
                        vecIndecs.Add(p4.index);

                        vecIndecs.Add(p1.index);
                        vecIndecs.Add(p4.index);
                        vecIndecs.Add(p2.index);
                    }
                }
            }
        }




		//create vertices 
		Vector3[] vertices = new Vector3[veclist.Count];
        Vector2[] uvs = uvlist.ToArray();


        for (int i = 0; i < veclist.Count; i++) {		
			Vector3 new_pt = getWorldPt (veclist [i]);
			vertices [i] = new_pt; 				
		}

        

        int[] indices = vecIndecs.ToArray();

        if (vecIndecs.Count == 0) Debug.LogError("error with vect indexs!!!");
        
        //    print(veclist.Count);
        //print(vecIndecs.Count);


        // Create the center mesh
        Mesh msh = new Mesh ();
		msh.Clear (); 
		msh.vertices = vertices;
		msh.uv = uvs;
		msh.triangles = indices;
		msh.RecalculateNormals ();
		msh.RecalculateBounds ();	 
		MeshFilter filter = gird_mesh.GetComponent (typeof(MeshFilter)) as MeshFilter;
        filter.transform.position = Vector3.zero;
        filter.mesh = msh;

        
        // Create the edge mesh
 
        List<int> vecIndecs_edge = new List<int>();

        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {

                int next_i = i + 1;
                int next_j = j + 1;
                if (next_i < grid_array.pCol.Count && next_j < grid_array.pCol[i].pRow.Count)
                {
                    MP_MY_PTS p1, p2, p3, p4;
                    p1 = grid_array.pCol[i].pRow[j];
                    p2 = grid_array.pCol[next_i].pRow[j];
                    p3 = grid_array.pCol[i].pRow[next_j];
                    p4 = grid_array.pCol[next_i].pRow[next_j];

                    if (p1.isEdgeIncluded && p2.isEdgeIncluded && p3.isEdgeIncluded && p4.isEdgeIncluded)
                    {
                        vecIndecs_edge.Add(p1.index);
                        vecIndecs_edge.Add(p3.index);
                        vecIndecs_edge.Add(p4.index);

                        vecIndecs_edge.Add(p1.index);
                        vecIndecs_edge.Add(p4.index);
                        vecIndecs_edge.Add(p2.index);
                    }
                }
            }
        }
        

        MeshFilter filter2 = edge_mesh.GetComponent(typeof(MeshFilter)) as MeshFilter;
        filter2.transform.position = Vector3.zero;
        int[] indices_edge = vecIndecs_edge.ToArray();
        if (vecIndecs_edge.Count > 0)
        {
            Mesh edg_msh = new Mesh();
            edg_msh.Clear();
            edg_msh.vertices = vertices;
            edg_msh.uv = uvs;
            edg_msh.triangles = indices_edge;
            edg_msh.RecalculateNormals();
            edg_msh.RecalculateBounds();
            filter2.mesh = edg_msh;
        }
        else filter2.mesh.Clear();



    }


    /// <summary>
    /// init a array from 0 to col
    /// </summary>
    void BulidArray ()
	{
		grid_array.foucs_col = COL / 2;
		grid_array.foucs_row = ROW / 2;
 

		for (int j = 0; j < COL + 1; j++) {
			MP_ROW_PTS rp = new MP_ROW_PTS ();
			for (int i = 0; i < ROW + 1; i++) {
				MP_MY_PTS pt = new MP_MY_PTS ();
				pt.pt = new Vector2 (j * 1f / COL, i * 1f / ROW);
                pt.col = j;
                pt.row = i; 
				pt.uv =  pt.pt;	

				rp.pRow.Add (pt);
			} 
			grid_array.pCol.Add (rp);
		}

	}

    /// <summary>
    /// init edges add fours edge
    /// </summary>
    void InitEages()
    {
        for (int ti = 0; ti < edge_sides.Length; ti++)
        {
            edge_sides[ti] = new EDGE();
        }
        edge_sides[0].edge_type = EDGE_WAY.LEFT;
        edge_sides[1].edge_type = EDGE_WAY.RIGHT;
        edge_sides[2].edge_type = EDGE_WAY.UP;
        edge_sides[3].edge_type = EDGE_WAY.DOWN; 
    }



    /// <summary>
    /// screen_cam attach to monitor
    /// </summary>
    Camera screen_cam;
    /// <summary>
    /// bulid a camera 
    /// </summary>
    void BulidCamera()
    {
        screen_cam = gameObject.transform.parent.GetComponent<Camera>();        
        screen_cam.targetDisplay = screen_id;
        transform.parent.name = "MP_Screen" + (screen_id + 1).ToString();
    }

    void Awake()
    {
      
        CONNERS_INDEXS = new int[,] { { 0, 0 }, { 0, ROW }, { COL, ROW }, { COL, 0 } };

        

        InitEages();
        BulidArray();        
        AddMesh();
        
    }
    /// <summary>
    /// init array and create mesh 
    /// </summary>
    void Start()
    {
        BulidCamera();
        Load();
        UpdateAll();

        MP_MultiScreen.Instance?.showInfor("");
    }


    /// <summary>
    /// gird_mesh for grid
    /// </summary>
    GameObject gird_mesh=null;


    /// <summary>
    /// edge_mesh for grid
    /// </summary>
    GameObject edge_mesh = null;

    /// <summary>
    /// add a mesh instance
    /// </summary>
    void AddMesh()
    {
        gird_mesh = Instantiate(grid_pre) as GameObject;
        gird_mesh.transform.SetParent(transform);
        gird_mesh.name = "MP_Grid_Center";

        edge_mesh = Instantiate(grid_pre) as GameObject;
        edge_mesh.transform.SetParent(transform);
        edge_mesh.name = "MP_Grid_Edge";
    }

    bool is_shine_pt = false;


	/// <summary>
    /// update ctrol point
    /// </summary>
    void updateCtrlPt ()
	{
        // make sure safe row and col
		if (grid_array.foucs_row < 0)
			grid_array.foucs_row = 0;
		if (grid_array.foucs_col < 0)
			grid_array.foucs_col = 0;
		if (grid_array.foucs_row > ROW)
			grid_array.foucs_row = ROW;
		if (grid_array.foucs_col > COL)
			grid_array.foucs_col = COL;

        if (!showGrid) return; // if close gird then not show all

        //clear old point
        for (int z = 0; z < transform.childCount; z++) {
			Transform holder = transform.GetChild (z);
			if (holder.name == "ctrlPt")
				Destroy (holder.gameObject);
		}


		Vector3 new_pt = getWorldPt (grid_array.pCol [grid_array.foucs_col].pRow [grid_array.foucs_row].pt); 
		GameObject ctrlpt = Instantiate (point_pre) as GameObject;
		new_pt.z = -1.1f;
		ctrlpt.transform.position = new_pt;
		ctrlpt.transform.SetParent (transform);
		ctrlpt.transform.name = "ctrlPt";

        //let it animation
		if (is_shine_pt) {
			is_shine_pt = false;
			MP_ScaleCabPt sc = ctrlpt.GetComponent<MP_ScaleCabPt> ();
			if (sc)
				sc.let_zoom ();
		}

	}

	/// <summary>
    /// gird lines
    /// </summary>
    public void UpdateLines ()
	{

        if (!showGrid) return;

        //delete old lines
        for (int z = 0; z < transform.childCount; z++) {
			Transform holder = transform.GetChild (z);
			if (holder.name == "holder")
				Destroy (holder.gameObject);
		}


		GameObject newholder = new GameObject ("holder");
		newholder.transform.SetParent (transform);

        //in horizontal
        for (int i = 0; i < grid_array.pCol.Count; i++) { 
			GameObject l = Instantiate (line_pre) as GameObject;
			l.transform.SetParent (newholder.transform);
			LineRenderer lr = l.GetComponent<LineRenderer> ();
			if (lr) {
                lr.startColor = lr.endColor = GirdColor;
                lr.startWidth = lr.endWidth = 0.02f;
                lr.positionCount = grid_array.pCol [i].pRow.Count;
				for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {				

					Vector3 new_pt = getWorldPt (grid_array.pCol [i].pRow [j].pt);
                    new_pt.z -= 0.00001f;

                    lr.SetPosition (j, new_pt);					 
				}
			}
		}
        //in vertical
        for (int j = 0; j < grid_array.pCol [0].pRow.Count; j++) {
			GameObject l = Instantiate (line_pre) as GameObject;
			l.transform.SetParent (newholder.transform);
			LineRenderer lr = l.GetComponent<LineRenderer> ();
			if (lr) {
                lr.startColor = lr.endColor = GirdColor;
                lr.positionCount=grid_array.pCol.Count;
                lr.startWidth = lr.endWidth = 0.02f;
                for (int i = 0; i < grid_array.pCol.Count; i++) {
					Vector3 new_pt = getWorldPt (grid_array.pCol [i].pRow [j].pt);
                    new_pt.z -= 0.00001f;
                    lr.SetPosition (i, new_pt);			 
				}
			}
		}
	}

    //

	/// <summary>
    /// only update vertices for save performace
    /// 
    /// </summary>
    void UpdateVecOnly ()
	{
 

		List<Vector2> veclist = new List<Vector2> (); 
 
		for (int i = 0; i < grid_array.pCol.Count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {				
 
				veclist.Add (grid_array.pCol [i].pRow [j].pt);
			}
		}


		Vector3[] vertices = new Vector3[veclist.Count];		 
		 
		for (int i = 0; i < veclist.Count; i++)
        {   
            Vector3 new_pt = getWorldPt  (veclist[i]);
			vertices [i] = new_pt; 
			 
		}

 


		MeshFilter filter = gird_mesh.GetComponent (typeof(MeshFilter)) as MeshFilter;
        filter.transform.position = Vector3.zero;
        filter.mesh.vertices = vertices;
		filter.mesh.RecalculateNormals();
		filter.mesh.RecalculateBounds();	 

	}

	/// <summary>
    /// update all point and gird and edge
    /// </summary>
    void UpdateAll ()
	{
        //UpdateVecOnly ();
        
		UpdateLines ();
		updateCtrlPt ();
		UpdateEdge();
        UpdateArray2Mesh();
    }
     

	int four_conner_index = 0;

    /// <summary>
    /// set foucs ctrol point
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public void set_ctrl_pt(int col,int row)
    {
        grid_array.foucs_col = col;
        grid_array.foucs_row = row;
        move_ctrl_pt();
    }

    /// <summary>
    /// loop four conners index
    /// </summary>
    /// <param name="isClockdir"></param>
    public void set_conner_pt(bool isClockdir)
    {
        if (isClockdir) four_conner_index++;
        else four_conner_index--;
        if (four_conner_index >= 4) four_conner_index = 0;
        else if (four_conner_index <0) four_conner_index = 3;
        move_ctrl_pt();
    }

    /// <summary>
    /// update move way from global
    /// </summary>
    /// <param name="_move_way"></param>
    public void set_move_way(MOVE_WAY _move_way)
    {
        move_way = _move_way;

        if (move_way == MOVE_WAY.FOUR_CONNER)
        {
            move_ctrl_pt();
        }      
    }


    /// <summary>
    /// find nearest point with mouse click
    /// </summary>
    /// <param name="mousex"></param>
    /// <param name="mousey"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public float getNearest_Col_row(float mousex, float mousey, out int col, out int row)
    {
        col = 0;
        row = 0;
        float min_distance = float.MaxValue;
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {
                Vector2 cpt = grid_array.pCol[i].pRow[j].pt;                
                Vector3 new_pt = getWorldPt(cpt) ;
                Vector3 screen_pt = new Vector3(mousex, mousey,0);
                Vector3 mouse_pt = screen_cam.ScreenToWorldPoint(screen_pt);
                float diff_distance = Mathf.Abs(mouse_pt.x - new_pt.x) + Mathf.Abs(mouse_pt.y - new_pt.y);
                if (min_distance > diff_distance)
                {
                    min_distance = diff_distance;
                    col = i;
                    row = j;                 
                }
            }
        }
        //MP_MultiScreen.Instance.showInfor(col + ":" + row);
        return min_distance;
    }


    /// <summary>
    /// when need point move
    /// </summary>
    void move_ctrl_pt ()
	{
		if (move_way == MOVE_WAY.FOUR_CONNER) {
			
			grid_array.foucs_col = CONNERS_INDEXS [four_conner_index, 0];
			grid_array.foucs_row = CONNERS_INDEXS [four_conner_index, 1];			
	 
		}  
		is_shine_pt = true;
		updateCtrlPt ();
	}

	/// <summary>
    /// by col 
    /// </summary>
    void move_gird_by_col ()
	{
		 
	 
		for (int j = 0; j < grid_array.pCol [grid_array.foucs_col].pRow.Count; j++) {		

			grid_array.pCol [grid_array.foucs_col].pRow [j].pt += move_shift;
			 
		} 
	}

	/// <summary>
    /// by row
    /// </summary>
    void move_gird_by_row ()
	{

		for (int i = 0; i < grid_array.pCol.Count; i++) { 
			 
				grid_array.pCol [i].pRow [grid_array.foucs_row].pt += move_shift;
			 

		} 
	}


	/// <summary>
    ///  gird position will slerp with four conners
    /// </summary>
    void move_gird_four_conner ()
	{


		grid_array.pCol [grid_array.foucs_col].pRow [grid_array.foucs_row].pt.y += move_shift.y;
		grid_array.pCol [grid_array.foucs_col].pRow [grid_array.foucs_row].pt.x += move_shift.x;

		Vector2[] conners = new Vector2[4];
		for (int c = 0; c < 4; c++) {
			int col1 = CONNERS_INDEXS [c, 0];		
			int row1 = CONNERS_INDEXS [c, 1];		
			conners [c] = grid_array.pCol [col1].pRow [row1].pt;
		}

		for (int i = 0; i < grid_array.pCol.Count; i++) { 
			for (int j = 0; j < grid_array.pCol [i].pRow.Count; j++) {				
				float a = i * 1.0f / COL;
				float b = j * 1.0f / ROW;										
				grid_array.pCol [i].pRow [j].pt = (1 - b) * (1 - a) * conners [0]
				+ (1 - b) * a * conners [3]
				+ (1 - a) * b * conners [1]
				+ a * b * conners [2];
			}
		}

	}

    /// <summary>
    /// move gird
    /// </summary>
    void move_gird ()
	{
		if (move_way == MOVE_WAY.SINGLE) {
			grid_array.pCol [grid_array.foucs_col].pRow [grid_array.foucs_row].pt.y += move_shift.y;
			grid_array.pCol [grid_array.foucs_col].pRow [grid_array.foucs_row].pt.x += move_shift.x;
		} else if (move_way == MOVE_WAY.FOUR_CONNER) {
			move_gird_four_conner ();		 
		} else if (move_way == MOVE_WAY.BY_COL) {
			move_gird_by_col ();		 
		} else if (move_way == MOVE_WAY.BY_ROW) {
			move_gird_by_row ();		 
		}
        else if (move_way == MOVE_WAY.SINGLE_SMOOTH)
        {
            move_gird_by_single_smooth();
        }

        for (int ti = 0; ti < edge_sides.Length; ti++)
        {

            EDGE e = edge_sides[ti];
            e.isUpdate = true;
        }

            UpdateAll ();
	}

    /// <summary>
    /// smooth move , you can change it with better move 
    /// </summary>
    void move_gird_by_single_smooth()
    {

        grid_array.pCol[grid_array.foucs_col].pRow[grid_array.foucs_row].pt.y += move_shift.y;
        grid_array.pCol[grid_array.foucs_col].pRow[grid_array.foucs_row].pt.x += move_shift.x;

        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {
                int diffi = Mathf.Abs(grid_array.foucs_col - i);
                int diffj = Mathf.Abs(grid_array.foucs_row - j);

                if ((diffi + diffj) >= 1) 
                {
                    Vector2 dir = new Vector2(diffi, diffj);
                    float sq = dir.magnitude;
                    sq = Mathf.Pow(sq, 1.5f);                   
                    float xx_strength = 0.8f / sq;                    
                    float yy_strength = 0.8f / sq;
                    Vector2 d1 = grid_array.pCol[i].pRow[j].pt;
                    d1 += new Vector2(xx_strength * move_shift.x, yy_strength * move_shift.y);
                    grid_array.pCol[i].pRow[j].pt = d1;
                }
            }
        }
    }


    /// <summary>
    /// load default grid ans reset edge
    /// </summary>
    public void LoadDefault()
    {
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {
                grid_array.pCol[i].pRow[j].pt = grid_array.pCol[i].pRow[j].uv;
            }
        }
        
        for (int ti = 0; ti < edge_sides.Length; ti++)
        {
            EDGE e = edge_sides[ti];
            e.isUpdate = true;
            e.isOpen = false;
        }

        _base_shift_rgb = Vector3.zero;
        _base_compensation_rgb = Vector3.zero;
        set_screen_color(Vector3.zero);
        UpdateArray2Mesh();
        UpdateVecOnly();
        UpdateLines();
        updateCtrlPt();

    }

    /// <summary>
    /// save key name
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    string getPosKeyStr(int col, int row)
    {
        string Save_key = "S" + screen_id + "P" + col + "_" + row;
        return Save_key;
    }
    /// <summary>
    /// save edge name
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    string getEdgeKeyStr(int edge_id)
    {
        string Save_key = "S" + screen_id + "E" + edge_id;
        return Save_key;
    }

    /// <summary>
    /// save color name
    /// </summary>
    /// <returns></returns>
    string getColorKeyStr(string RGB_name)
    {
        string Save_key = "S" + screen_id + "_" +RGB_name;
        return Save_key;
    }

    /// <summary>
    /// save gamma name
    /// </summary>
    /// <returns></returns>
    string getGammaKeyStr()
    {
        string Save_key = "S" + screen_id + "_" + "Gamma";
        return Save_key;
    }
    /// <summary>
    /// load data ,if not save before ignor it.
    /// </summary>
    public void Load()
    {
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {

                 
                string s_v = PlayerPrefs.GetString(getPosKeyStr(i,j), "");
                if (s_v.Length > 0)
                {
                    string[] vals = s_v.Split(',');
                    if (vals.Length == 2)
                    {
                        Vector2 ppt = new Vector2(float.Parse(vals[0]), float.Parse(vals[1]));
                        grid_array.pCol[i].pRow[j].pt = ppt;
                    }
                }
            }
        }

        for (int i = 0; i < edge_sides.Length; i++)
        {
            string saveKey = getEdgeKeyStr(i);
            if (PlayerPrefs.GetString(saveKey) == "1") 
            {
                edge_sides[i].isOpen = true;
                edge_sides[i].isUpdate = true;
              
            }
        }
        
        _base_shift_rgb = new Vector3(
            PlayerPrefs.GetFloat(getColorKeyStr("R"), 0),
            PlayerPrefs.GetFloat(getColorKeyStr("G"), 0),
            PlayerPrefs.GetFloat(getColorKeyStr("B"), 0)
            );

        set_screen_color(Vector3.zero);


        _base_compensation_rgb = new Vector3(
            PlayerPrefs.GetFloat(getColorKeyStr("R_C"), 0),
            PlayerPrefs.GetFloat(getColorKeyStr("G_C"), 0),
            PlayerPrefs.GetFloat(getColorKeyStr("B_C"), 0)
            );

        float gamma = PlayerPrefs.GetFloat(getGammaKeyStr(), 0.31f);
        update_gamma(gamma,false);



    }

    /// <summary>
    /// Save 
    /// </summary>
    public void Save()
    {
        for (int i = 0; i < grid_array.pCol.Count; i++)
        {
            for (int j = 0; j < grid_array.pCol[i].pRow.Count; j++)
            {                
                string s1 = string.Format("{0:000.000}", grid_array.pCol[i].pRow[j].pt.x);
                string s2 = string.Format("{0:000.000}", grid_array.pCol[i].pRow[j].pt.y);
                PlayerPrefs.SetString(getPosKeyStr(i, j), s1 + "," + s2);
            }
        }

        for (int i = 0; i < edge_sides.Length; i++)
        {
            string saveKey = getEdgeKeyStr(i);
            if (edge_sides[i].isOpen)
            {
                PlayerPrefs.SetString(saveKey, "1");
            }
            else
            {
                PlayerPrefs.SetString(saveKey, "0");
            }
        }

        
        PlayerPrefs.SetFloat(getColorKeyStr("R"), _base_shift_rgb.x);
        PlayerPrefs.SetFloat(getColorKeyStr("G"), _base_shift_rgb.y);
        PlayerPrefs.SetFloat(getColorKeyStr("B"), _base_shift_rgb.z);

        PlayerPrefs.SetFloat(getColorKeyStr("R_C"), _base_compensation_rgb.x);
        PlayerPrefs.SetFloat(getColorKeyStr("G_C"), _base_compensation_rgb.y);
        PlayerPrefs.SetFloat(getColorKeyStr("B_C"), _base_compensation_rgb.z);

        float gamma = get_gamma();
        if(gamma>0)  PlayerPrefs.SetFloat(getGammaKeyStr(), gamma);

  
    }

    /// <summary>
    /// update gamma to all child edge
    /// </summary>
    /// <param name="gamma"></param>
    public void update_gamma(float gamma,bool isShowGamaLine)
    {
        MP_Edge[] edges = GetComponentsInChildren<MP_Edge>();
        int ii = 0;      
        foreach (MP_Edge me in edges)
        {
                if(ii==0 && isShowGamaLine) me.UpdateGamma(gamma, true);
                else
                    me.UpdateGamma(gamma, false);
                ii++;
        }        
    }

    /// <summary>
    /// screen edges use same gamma . so return first gamma
    /// </summary>
    /// <returns></returns>
    public float get_gamma()
    {
        MP_Edge[] edges = GetComponentsInChildren<MP_Edge>();
        if (edges.Length > 0) return edges[0].gamma;
        return -1f;
    }

    /// <summary>
    /// set_move_gird with shift amount
    /// </summary>
    /// <param name="ms"></param>
    public void set_move_gird(Vector2 ms)
    {
        move_shift = ms;
        move_gird();
    }


    /// <summary>
    /// get rgb shift color
    /// </summary>
    /// <returns></returns>
    public Vector3 get_screen_color( )
    {
        return _base_shift_rgb;
    }

    /// <summary>
    /// get rgb center grid color
    /// </summary>
    /// <returns></returns>
    public Vector3 get_compsention_color()
    {
        return _base_compensation_rgb;
    }

    /// <summary>
    /// get rgb center grid color
    /// </summary>
    /// <returns></returns>
    public void set_compsention_color(Vector3 c_color)
    {
       _base_compensation_rgb  = c_color;
        Vector3 color = _base_shift_rgb + _base_compensation_rgb;
        Material m = gird_mesh.GetComponent<Renderer>().material;
        if (m)
            m.SetVector("_RGB", new Vector4(color.x, color.y, color.z, 0f));

    }

    /// <summary>
    /// update color
    /// </summary>
    /// <param name="color"></param>
    public void set_screen_color_directly(Vector3 color)
    {
        _base_shift_rgb = color;
        Material m = gird_mesh.GetComponent<Renderer>().material;
        if (m)
        {
            Vector3 color2 = _base_shift_rgb + _base_compensation_rgb;
            m.SetVector("_RGB", new Vector4(color2.x, color2.y, color2.z, 0f));
        }


        Material m2 = edge_mesh.GetComponent<Renderer>().material;
        if (m2) m2.SetVector("_RGB", new Vector4(color.x, color.y, color.z, 0f));

        if (MP_MultiScreen.Instance)
        {

            string r = string.Format("{0:0.000}", color.x);
            string g = string.Format("{0:0.000}", color.y);
            string b = string.Format("{0:0.000}", color.z);

            MP_MultiScreen.Instance.showInfor("RGB: " + "{ " + r + "," + g + "," + b + " }");
        }
    }


    /// <summary>
    /// update color by shift
    /// </summary>
    /// <param name="color_shift"></param>
    public void set_screen_color(Vector3 color_shift)
    {
        _base_shift_rgb += color_shift;
        set_screen_color_directly(_base_shift_rgb);
       
    }


    /// <summary>
    /// update UV
    /// </summary>
    /// <param name="uv"></param>
    public void set_mesh_uv(Vector4 uv,RenderTexture rt)
    {

        Material m = gird_mesh.GetComponent<Renderer>().material;
        m.mainTexture = rt;
        m.SetVector("_UVRect", uv);

        Material m2 = edge_mesh.GetComponent<Renderer>().material;
        m2.mainTexture = rt;
        m2.SetVector("_UVRect", uv);


    }

    void Update ()
    {
 

    }

    /// <summary>
    /// open or close edge
    /// </summary>
    /// <param name="id"></param>
    public void change_edge(int id)
	{
		edge_sides[id].isOpen =!edge_sides[id].isOpen;
		edge_sides[id].isUpdate = true;
        UpdateAll();
	}
}
