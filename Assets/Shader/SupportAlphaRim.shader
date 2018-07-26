// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SupportAlphaRim"
{
	Properties
	{
		_Color("Main Color",Color) = (0.6,0.6,0.6,1)
		_AlphaRange("Alpha Range",Range(0,1)) = 0
		_RimColor("Rim Color",Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }

		Cull back
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite OFF

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag           
#include "unitycg.cginc"
#include "unitylightingcommon.cginc"

		struct a2v
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 normalDir : Texcoord0;
		float3 worldPos : TEXCOORD1;
	};

	fixed4 _Color;
	float _AlphaRange;
	fixed4 _RimColor;

	v2f vert(a2v i)
	{
		v2f o;
		// 转化顶点位置
		o.pos = UnityObjectToClipPos(i.vertex);
		// 获取世界空间法线向量
		o.normalDir = mul(float4(i.normal,0),unity_WorldToObject).xyz;
		// 获取世界坐标系顶点位置
		o.worldPos = mul(unity_ObjectToWorld,i.vertex);
		return o;
	}

	fixed4 frag(v2f v) :COLOR
	{
		// 法线标准化
		float3 normal = normalize(v.normalDir);
		// 视角方向标准化
		float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - v.worldPos.xyz);
		// 点积
		float NdotV = saturate(dot(normal,viewDir));
		// 漫反射
		fixed3 diffuse = NdotV * _Color + UNITY_LIGHTMODEL_AMBIENT.rgb;
		float alpha = 1 - NdotV;
		// 边缘色
		fixed3 rim = _RimColor * alpha;
		// 混合输出
		return fixed4(diffuse + rim ,alpha * (1 - _AlphaRange) + _AlphaRange);
	}

		ENDCG
	}
	}
		// 如果需要阴影在此回滚 不需要注释掉即可
		//Fallback "Diffuse"
}