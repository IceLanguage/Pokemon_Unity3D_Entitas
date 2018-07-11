Shader "chenjd/Grass" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_AlphaTex("Alpha (A)", 2D) = "white" {}
		_Height("Grass Height", float) = 3
		_Width("Grass Width", range(0, 0.1)) = 0.05
		_Color("Grass Color", Color) = (1, 1, 1, 1)
		_Force("Force On Grass",Vector) = (0, 0, 0, 1)
	}
	SubShader{
		Cull off
			Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }


		Pass
		{

			Cull OFF
			Tags{ "LightMode" = "ForwardBase" }
			AlphaToMask On


			CGPROGRAM

			#include "UnityCG.cginc" 
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityLightingCommon.cginc" // 用来处理光照的一些效果
			#pragma target 4.0

//			#pragma multi_compile_instancing
//			#pragma instancing_options procedural:setup
//
//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//			StructuredBuffer<float4> PositionBuffer;
//#endif
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _Color;//草的颜色	
			half _Height;//草的高度
			half _Width;//草的宽度
			vector _Force;//草的受力
			struct v2g
			{
				fixed4 pos : SV_POSITION;
				fixed3 norm : NORMAL;
				fixed2 uv : TEXCOORD0;
			};

			struct g2f
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
			static const half oscillateDelta = 0.05;

			
			v2g vert(appdata v)
			{
				
				v2g o;
				o.pos = v.vertex;
				o.norm = v.normal;
				o.uv = v.texcoord;
				
				return o;
			}

			g2f createGSOut() {
				g2f output;

				output.pos = fixed4(0, 0, 0, 0);
				output.norm = fixed3(0, 0, 0);
				output.uv= fixed2(0, 0);

				return output;
			}


			[maxvertexcount(30)]
			void geom(point v2g points[1], inout TriangleStream<g2f> triStream)
			{

				fixed4 root = points[0].pos;

				const int vertexCount = 12;

				half random = sin(UNITY_HALF_PI * frac(root.x) + UNITY_HALF_PI * frac(root.z));


				_Width = _Width + (random / 50);
				_Height = _Height + (random / 5);


				g2f v[vertexCount] = {
					createGSOut(), createGSOut(), createGSOut(), createGSOut(),
					createGSOut(), createGSOut(), createGSOut(), createGSOut(),
					createGSOut(), createGSOut(), createGSOut(), createGSOut()
				};

				//处理纹理坐标
				half currentV = 0;
				half offsetV = 1.f / ((vertexCount / 2) - 1);

				//处理当前的高度
				half currentHeightOffset = 0;
				half currentVertexHeight = 0;

				//风的影响系数
				half windCoEff = 0;

				//震荡强度
				half oscillationStrength = 2.5f;
				for (int i = 0; i < vertexCount; i++)
				{
					v[i].norm = fixed3(0, 0, 1);

					if (fmod(i, 2) == 0)
					{
						v[i].pos = fixed4(root.x - _Width, root.y + currentVertexHeight, root.z, 1);
						v[i].uv = fixed2(0, currentV);
					}
					else
					{
						v[i].pos = fixed4(root.x + _Width, root.y + currentVertexHeight, root.z, 1);
						v[i].uv = fixed2(1, currentV);

						currentV += offsetV;
						currentVertexHeight = currentV * _Height;
					}

					//风力影响***************************************************************************************************
					fixed2 wind = fixed2(sin(_Time.x * UNITY_PI * 3), sin(_Time.x * UNITY_PI * 3));
					wind.x += (sin(_Time.x + root.x / 25) + sin((_Time.x + root.x / 15) + 50)) * 0.5;
					wind.y += cos(_Time.x + root.z / 80);
					wind *= lerp(0.7, 1.0, 1.0 - random);
					
					half sinSkewCoeff = random;
					half lerpCoeff = (sin(oscillationStrength * _Time.x + sinSkewCoeff) + 1.0) / 2;
					fixed2 leftWindBound = wind * (1.0 - oscillateDelta);
					fixed2 rightWindBound = wind * (1.0 + oscillateDelta);

					wind = lerp(leftWindBound, rightWindBound, lerpCoeff);

					half randomAngle = lerp(-UNITY_PI, UNITY_PI, random);
					half randomMagnitude = lerp(0, 1., random);
					fixed2 randomWindDir = fixed2(sin(randomAngle), cos(randomAngle));
					wind += randomWindDir * randomMagnitude;

					half windForce = length(wind);

					v[i].pos.xz += wind.xy * windCoEff;
					v[i].pos.y -= windForce * windCoEff * 0.8;
					//------------------------------------------------------------------------------------------------------------
					 
					//外力影响****************************************************************************************************
					fixed2 forceFromOutside = fixed2(_Force.x, _Force.y);
					v[i].pos.xz += saturate(forceFromOutside.xy);
					if (v[i].pos.y > length(forceFromOutside))
					{
						v[i].pos.y -= length(forceFromOutside);
					}
					
					//------------------------------------------------------------------------------------------------------------


					v[i].pos = UnityObjectToClipPos(v[i].pos);

					if (fmod(i, 2) == 1) {

						windCoEff += offsetV;
					}

				}

				for (int p = 0; p < (vertexCount - 2); p++) {
					triStream.Append(v[p]);
					triStream.Append(v[p + 2]);
					triStream.Append(v[p + 1]);
				}
					
					

				
			}

//			void setup()
//			{
//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//				float4 data = PositionBuffer[unity_InstanceID];
//
//				float rotation = data.w * data.w * _Time.y * 0.5f;
//				rotate2D(data.xz, rotation);
//
//				unity_ObjectToWorld._11_21_31_41 = float4(data.w, 0, 0, 0);
//				unity_ObjectToWorld._12_22_32_42 = float4(0, data.w, 0, 0);
//				unity_ObjectToWorld._13_23_33_43 = float4(0, 0, data.w, 0);
//				unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
//				unity_WorldToObject = unity_ObjectToWorld;
//				unity_WorldToObject._14_24_34 *= -1;
//				unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
//#endif
//			}

			half4 frag(g2f IN) : COLOR
			{
				fixed4 color = tex2D(_MainTex, IN.uv)*_Color;
				fixed4 alpha = tex2D(_AlphaTex, IN.uv);

				fixed3 worldNormal = UnityObjectToWorldNormal(IN.norm);

				//ads
				fixed3 light;

				//ambient
				fixed3 ambient = ShadeSH9(half4(worldNormal, 1));

				//diffuse
				fixed3 diffuseLight = saturate(dot(worldNormal, UnityWorldSpaceLightDir(IN.pos))*0.1+1) * _LightColor0;

				//specular Blinn-Phong 
				fixed3 halfVector = normalize(UnityWorldSpaceLightDir(IN.pos) + WorldSpaceViewDir(IN.pos));
				fixed3 specularLight = pow(saturate(dot(worldNormal, halfVector)), 32) * _LightColor0;

				light = ambient + diffuseLight + specularLight;
				
				return float4(color.rgb * light, alpha.g);

			}
			ENDCG

		}
		
	}
}