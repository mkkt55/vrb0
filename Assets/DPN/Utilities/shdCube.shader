// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/************************************************************************************

Copyright   :   Copyright 2015 DeePoon LLC. All Rights reserved.

************************************************************************************/

Shader "DeePoon/shdCube"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CubeMin ("CubeMin", Range(-10,10)) = -4
		_CubeMax ("CubeMax", Range(-10,10)) = 4
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }
		Tags { "Queue" = "Transparent" }
		ZWrite On
		ZTest Less
		AlphaTest Off
		Blend SrcAlpha OneMinusSrcAlpha, One One
		BlendOp Add, Add
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		struct Input
		{
			float2 uv_MainTex;
			float3 cv_Color;
		};
		sampler2D _MainTex;
		float _CubeMin;
		float _CubeMax;
		void vert(inout appdata_full vert_input, out Input vert_output)
		{
			UNITY_INITIALIZE_OUTPUT(Input,vert_output);
			float cube_range = _CubeMax - _CubeMin;
			vert_output.cv_Color
				= ( mul( unity_ObjectToWorld , vert_input.vertex ) - float3( _CubeMin , _CubeMin , _CubeMin ) )
				/ cube_range;
		}
		void surf( Input frag_input, inout SurfaceOutput frag_output )
		{
			frag_output.Albedo = tex2D(_MainTex, frag_input.uv_MainTex).rgb;
			frag_output.Albedo = frag_input.cv_Color.rgb;
			frag_output.Alpha = 0.7;
		}
		ENDCG
	} 
	Fallback "Diffuse"
}