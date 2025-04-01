/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MP_MY_PTS
{

	/// <summary>
    /// gird point , position  and texture uv 
    /// </summary>
    public MP_MY_PTS()
	{
 
		index = -1;
		pt = new Vector2(0,0);
		uv = new Vector2(0,0);
        isEdgeIncluded = false;
        col = row = -1;
    }
 
 
	public Vector2 pt;
	public Vector2 uv; 
	public int index;
    public int col;
    public int row;

    /// <summary>
    /// if this point in blend edge , bulid blend mesh 
    /// </summary>
    public bool isEdgeIncluded;
}

/// <summary>
/// row for points
/// </summary>
public class MP_ROW_PTS
{
	public List<MP_MY_PTS> pRow = new List<MP_MY_PTS>(); 
}

/// <summary>
/// gird points 
/// </summary>
public class MP_COL_ROW_PTS
{
	public  List<MP_ROW_PTS> pCol = new List<MP_ROW_PTS>(); 
	public int foucs_col = -1;
	public int foucs_row = -1;
}
