



Shader "LHShader/Cartoon"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Outline("Thick of Outline",range(0,0.1)) = 0.002//挤出描边的粗细
		_Factor("Factor",range(0,1)) = 0.5//挤出多远
		_ToonEffect("Toon Effect",range(0,1)) = 0.5//卡通化程度（二次元与三次元的交界线）
		_Steps("Steps of toon",range(0,9)) = 3//色阶层数
		_Color("Main Color",color) = (1,1,1,1)//物体的颜色
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass
		{
			Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			half _Outline;
			half _Factor;

			

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 pos:SV_POSITION;
			};

			fixed4 _MainTex_ST;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				fixed3 dir = normalize(v.vertex.xyz);
				fixed3 dir2 = v.normal;
				half D = dot(dir, dir2);//判断渲染点是背向几何中心还是正向几何中心
				dir = dir * sign(D);
				dir = dir * _Factor + dir2 * (1 - _Factor);//把该点位置朝向与法线方向按外部变量_Factor的比重混合，来控制挤出多远
				v.vertex.xyz += dir * _Outline;//把物体背面的点向外挤出
				o.pos = UnityObjectToClipPos(v.vertex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return fixed4(0, 0, 0, 1);
			}
			ENDCG
		}
		pass {
			Tags{ "LightMode" = "ForwardBase" }
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"  
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"  
			#include "UnityShaderVariables.cginc"

			half _Steps;
			half _ToonEffect;
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _Color;
			struct v2f {
				fixed4 pos:SV_POSITION;
				fixed3 lightDir:TEXCOORD0;
				fixed3 viewDir:TEXCOORD1;
				fixed3 normal:TEXCOORD2;
				fixed2 uv : TEXCOORD3;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.lightDir = _WorldSpaceLightPos0 - v.vertex;
				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}
			fixed4 frag(v2f i) :COLOR
			{
				fixed4 c = 1;
				

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 worldNormal = normalize(i.normal);
				fixed3 viewDir = normalize(i.viewDir);
				fixed3 lightDir = normalize(i.lightDir);
				//求出距离光源的距离
				half dist = length(i.lightDir);

				//获取光源方向
				fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 halflambert = dot(worldNormal, worldLight)*0.1 + 0.5;

				//利用漫反射光照公式计算漫反射
				fixed3 diffuse = _LightColor0.rgb*halflambert;
				half atten = 1 / (dist);//根据距光源的距离求出衰减
				half toon = floor(diffuse*atten*_Steps) / _Steps;
				diffuse = lerp(diffuse,toon,_ToonEffect);

				

				fixed4 color = fixed4(ambient + diffuse,1.0);

				c =  _LightColor0*(color)*tex2D(_MainTex, i.uv);//求出最终颜色
				return c* _Color;
			}
			ENDCG
		}
		//pass {//附加点光源的pass渲染
		//	Tags{ "LightMode" = "ForwardAdd" }
		//	Blend One One
		//	Cull Back
		//	ZWrite Off
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#include "UnityCG.cginc"

		//	fixed4 _LightColor0;
		//	fixed4 _Color;
		//	half _Steps;
		//	half _ToonEffect;

		//	struct v2f {
		//		fixed4 pos:SV_POSITION;
		//		fixed3 lightDir:TEXCOORD0;
		//		fixed3 viewDir:TEXCOORD1;
		//		fixed3 normal:TEXCOORD2;
		//	};

		//	v2f vert(appdata_full v) {
		//		v2f o;
		//		o.pos = UnityObjectToClipPos(v.vertex);
		//		o.normal = v.normal;
		//		o.viewDir = ObjSpaceViewDir(v.vertex);
		//		o.lightDir = _WorldSpaceLightPos0 - v.vertex;

		//		return o;
		//	}
		//	fixed4 frag(v2f i) :COLOR
		//	{
		//		fixed4 c = 1;

		//		fixed4 ambient =fixed4(UNITY_LIGHTMODEL_AMBIENT.xyz,1.0);
		//		c = _Color * _LightColor0*(ambient);//求出最终颜色
		//		return c;
		//	}
		//		ENDCG
		//}
	}
}
