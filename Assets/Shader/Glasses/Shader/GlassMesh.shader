Shader "LHShader/Grass" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Force("Force On Grass",Vector) = (0, 0, 0, 1)
		_Color("Grass Color", Color) = (1, 1, 1, 1)

	}
	SubShader
{
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }


	Pass
	{

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Tags{ "LightMode" = "ForwardBase" }
		//AlphaToMask On

		

		CGPROGRAM
		/*#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup*/

		
		#include "UnityCG.cginc" 
		#pragma vertex vert
		#pragma fragment frag
//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//			StructuredBuffer<float4> PositionBuffer;
//#endif
		#include "UnityLightingCommon.cginc" // 用来处理光照的一些效果
//		void setup()
//		{
//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//			float4 data = PositionBuffer[unity_InstanceID];
//			
//			float rotation = data.w * data.w * _Time.y * 0.5f;
//			rotate2D(data.xz, rotation);
//			
//			unity_ObjectToWorld._11_21_31_41 = float4(data.w, 0, 0, 0);
//			unity_ObjectToWorld._12_22_32_42 = float4(0, data.w, 0, 0);
//			unity_ObjectToWorld._13_23_33_43 = float4(0, 0, data.w, 0);
//			unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
//			unity_WorldToObject = unity_ObjectToWorld;
//			unity_WorldToObject._14_24_34 *= -1;
//			unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
//#endif
//		}
		sampler2D _MainTex;
		sampler2D _AlphaTex;
		fixed4 _Color;//草的颜色
		vector _Force;//草的受力
		struct v2f
		{
			fixed4 pos : SV_POSITION;
			fixed3 norm : NORMAL;
			fixed2 uv : TEXCOORD0;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 normal:NORMAL;
			fixed2 texcoord : TEXCOORD0;
		};

		static const half oscillateDelta = 0.005;
		static const half oscillationStrength = 4.5f;
		v2f vert(appdata v)
		{

			v2f o;
			fixed4 root = v.vertex;
			half windCoEff = root.y/10;
			fixed2 forceFromOutside = fixed2(_Force.x, _Force.y);
			half windForce = length(forceFromOutside);
			root.xz += forceFromOutside.xy * windCoEff;
			root.y -= windForce * windCoEff * 0.8;
			if (root.y < 0.01)
				root.y = 0.01;
			o.pos = UnityObjectToClipPos(root);
			o.uv = v.texcoord;
			o.norm = mul(v.normal, (float3x3)unity_WorldToObject);

			return o;
		}


		half4 frag(v2f IN) : COLOR
		{
			fixed4 color = tex2D(_MainTex, IN.uv)*_Color;
			fixed3 worldNormal = UnityObjectToWorldNormal(IN.norm);

			//ads
			fixed3 light;

			//ambient
			fixed3 ambient = ShadeSH9(half4(worldNormal, 1));

			//diffuse
			fixed3 diffuseLight = saturate(dot(worldNormal, UnityWorldSpaceLightDir(IN.pos))*0.1 + 1) * _LightColor0;

			//specular Blinn-Phong 
			fixed3 halfVector = normalize(UnityWorldSpaceLightDir(IN.pos) + WorldSpaceViewDir(IN.pos));
			fixed3 specularLight = pow(saturate(dot(worldNormal, halfVector)), 32) * _LightColor0;

			light = ambient + diffuseLight + specularLight;
			return float4(color.rgb*light,1);

		}
			ENDCG

	}

}
}