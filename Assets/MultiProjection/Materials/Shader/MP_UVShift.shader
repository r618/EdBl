// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MultiPorjection/MP_UVShift" {
	Properties {
		
		_MainTex ("RenderTarget", 2D) = "white" {}
		_UVRect ("UVRect(x,y,width,height)", Vector) = (0,0,1,1) 
	    _RGB("RGB(red,green,blue)", Vector) = (0,0,0,0)
	}
	SubShader {
	 	
	 	 Tags { "RenderType"="Transparent" "Queue"="Transparent" }
     LOD 200
     Blend SrcAlpha OneMinusSrcAlpha
     ZTest Less
      Cull Off
       Lighting Off 
       ZWrite Off
        Fog { Mode Off }
      
		 Pass {
			CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag
			 
			 #include "UnityCG.cginc"
			   
			 sampler2D _MainTex;
		 
			 struct v2f {
			     float4  pos : SV_POSITION;
			     float2  uv : TEXCOORD0;
			 };
			 
			 float4 _MainTex_ST;
			 
			 float4 _UVRect;
			 float4 _RGB;
			 v2f vert (appdata_base v)
			 {
			     v2f o;
			     o.pos = UnityObjectToClipPos (v.vertex);
				 float2 uv = float2(v.texcoord.x*_UVRect.z + _UVRect.x, v.texcoord.y*_UVRect.w + _UVRect.y);
			     o.uv = TRANSFORM_TEX (uv, _MainTex);
			     return o;
			 }
			 
			 half4 frag (v2f i) : COLOR
			 {	 
				 return tex2D(_MainTex, i.uv)  + _RGB;
			 }
			 ENDCG		
			 } 		     
		 }
		 Fallback "VertexLit"
	}
