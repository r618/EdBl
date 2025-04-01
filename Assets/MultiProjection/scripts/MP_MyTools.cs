/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
using UnityEngine;
using System.Collections;
 
	public class MP_MyTools
	{
		public static Vector3 V2To3 (Vector2 pos)
		{
			return new Vector3 (pos.x, pos.y, 0);
		}

		public static Vector3 V2To3 (Vector2 pos, float z)
		{
			return new Vector3 (pos.x, pos.y, z);
		}

		public static float getR(float range)
		{
			return Random.Range(-range,range);
		}
		public static float getR(float range1,float range2)
		{
			return Random.Range(range1,range2);
		}

		public static float ParseFloat(string val)
		{
			float v = 0f;
			if(!float.TryParse(val,out v))
			{
				v =0;
			}	
			return v;
		}

		public static int ParseInt(string val)
		{
			int v = 0;
			if(!int.TryParse(val,out v))
			{
				v =0;
			}	
			return v;
		}
	}
