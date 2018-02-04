Shader "Custom/OutlineShader" 
{
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		_Extrude("Extrude", Range(0,5)) = 0.1
		
		[Header(Diffuse)]
		_DiffuseColor("Diffuse Light Color", Color) = (1,1,1,1)
		_DiffuseIntensity("Diffuse Light Intensity", Range(0.0, 1.0)) = 1.0

		[Header(Ambient)]
		_AmbientLightColor("Ambient Light Color", Color) = (1,1,1,1)
		_AmbientLighIntensity("Ambient Light Intensity", Range(0.0, 1.0)) = 1.0

		[Header(Specular)]
		_SpecularColor("SpecularColor", Color) = (1,1,1,1)
		_SpecularStrengh("Specular Strenght", Range(0, 100)) = 0.0

		[Header(Emission)]
		[HDR]_EmiColor("Emission Color", Color) = (1,1,1,1)
		_EmiIntensity("Emission Intensity", Range(0.1, 5)) = 0.1	
		_EmissionTex("Emission Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	ENDCG

	SubShader
	{ 
		Tags{ "LightMode" = "ForwardBase" }
		Tags{ "RenderType" = "Opaque" }
		Tags{ "Queue" = "Transparent" }
	
		Pass
		{
			ZWrite Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};


			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

			half _Extrude;
			fixed4 _OutlineColor;

			v2f vert(appdata IN)
			{
				v2f o;
				IN.vertex.xyz *= _Extrude;
				o.pos = UnityObjectToClipPos(IN.vertex);
				return o;
			}

			fixed4 frag(v2f output) : SV_Target
			{
				return  _OutlineColor;
			}
			ENDCG
		}
			
		Pass
		{
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			float4 _LightColor0;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _SpecularColor;
			float _SpecularStrengh;

			float4 _AmbientLightColor;
			float _AmbientLighIntensity;

			float4 _DiffuseColor;
			float _DiffuseIntensity;

			float4 _EmiColor;
			float _EmiIntensity;
			sampler2D _EmissionTex;

			struct vertInput 
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};


			struct vertOutput 
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
			};

			vertOutput vert(vertInput v)
			{
				vertOutput o;

				o.worldPos = mul(unity_ObjectToWorld, v.pos);

				o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.));

				o.normal  = UnityObjectToWorldNormal(v.normal);

				o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(vertOutput output) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex, output.uv);

				float3 L = normalize(_WorldSpaceLightPos0.xyz);
				float3 V = normalize(_WorldSpaceCameraPos.xyz - output.worldPos.xyz);
				float3 N = normalize(output.worldNormal);
				
				//ambient
				float4 ambientLight = _AmbientLightColor * _AmbientLighIntensity;

				//diffuse
				float diffuseTerm = max(0.0f, dot(N, L) * _LightColor0);
				float4 diffuseLight = diffuseTerm * _DiffuseColor * _DiffuseIntensity * _LightColor0;

				//specular

				float3 halfVector = normalize(L + V);
				float dotH = max(0.0f, dot(N, halfVector));
				float4 specularLight = (pow(dotH, _SpecularStrengh) ) * _LightColor0 * _SpecularColor;
				 
				fixed4 light = ambientLight + diffuseLight + specularLight;
				texColor.rgb *= light.rgb;

				//Emission
				fixed4 emissionTerm = tex2D(_EmissionTex, output.uv).r * _EmiColor * _EmiIntensity;

				texColor.rgb += emissionTerm.rgb;

				return texColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
