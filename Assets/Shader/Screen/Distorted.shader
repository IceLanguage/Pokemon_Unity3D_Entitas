// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//https://blog.csdn.net/cexgiant/article/details/52756202

Shader "LHShader/Distorted"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Distorted("Distorted",float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _MainTex_TexelSize;
			fixed _Distorted;

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				fixed4 vertex : SV_POSITION;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 tuv = i.uv;

				//这里将当前纹理坐标平移至中心点，表示按中心旋转，
				//如果有需要，可以算出3d坐标在屏幕上的位置，然后根据屏幕位置做平移
				fixed2 uv = fixed2(tuv.x - 0.5, tuv.y - 0.5);
				//通过距离计算出当前点的旋转弧度PI/180=0.1745
				fixed angle = _Distorted * 0.1745 / (length(uv) + 0.1);
				fixed sinval, cosval;
				sincos(angle, sinval, cosval);
				//构建旋转矩阵
				fixed2x2 mat = fixed2x2(cosval, -sinval, sinval, cosval);
				//旋转完成后，平移至原位置
				uv = mul(mat, uv) + 0.5;

				// sample the texture
				fixed4 col = tex2D(_MainTex, uv);


				return col;
			}
			ENDCG
		}
	}
}
