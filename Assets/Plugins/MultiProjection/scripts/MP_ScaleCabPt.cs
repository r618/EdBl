/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
/// 
using UnityEngine;
using System.Collections;

/// <summary>
/// when ctrol point is choosen , triger a scale animation
/// </summary>
public class MP_ScaleCabPt : MonoBehaviour {

	float scale = 1f;
	 
	// Use this for initialization
	void Start () 
	{
		
	}

	public void let_zoom()
	{
		scale = 5.0f;
	}
	

	void Update () {
		
		if(scale<1.001f) 
		{
			scale = 1f;
			Destroy(this);
		}
		else  scale -=Time.deltaTime*3;
		transform.localScale = new Vector3(scale,scale,scale);
	}
}
