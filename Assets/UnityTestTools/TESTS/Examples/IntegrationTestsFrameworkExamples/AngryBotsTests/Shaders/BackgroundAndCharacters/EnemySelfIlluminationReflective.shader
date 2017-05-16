// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


/*

the 

EnemySelfIlluminationReflective 

is a cheaper and less accurate version of the
CharacterSelfIlluminationReflective shader but pretty
much the same on highest quality setting

*/

Shader "AngryBots/Character/EnemySelfIlluminationReflective" {
	
	Properties {
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cube ("Cube", CUBE) = "black" {}
		_SelfIllumStrength ("_SelfIllumStrength", Range(0.0, 1.5)) = 1.0
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"		
	#include "AngryInclude.cginc"
				
	uniform half4 _MainTex_ST;
	uniform sampler2D _MainTex;
	uniform samplerCUBE _Cube;
	uniform fixed _SelfIllumStrength;
	uniform sampler2D _BumpMap;
	// uniform float4x4 _CameraToWorld;
	
	half3 VertexLightsWorldSpace (half3 WP, half3 WN)
	{
		half3 lightColor = half3(0.0,0.0,0.0);

		// preface & optimization
		half3 toLight0 = mul(unity_CameraToWorld, unity_LightPosition[0] * half4(1,1,-1,1)).xyz - WP;
		half3 toLight1 = mul(unity_CameraToWorld, unity_LightPosition[1] * half4(1,1,-1,1)).xyz - WP;
		half2 lengthSq2 = half2(dot(toLight0, toLight0), dot(toLight1, toLight1));

		half2 atten2 = half2(1.0,1.0) + lengthSq2 * half2(unity_LightAtten[0].z, unity_LightAtten[1].z);
		atten2 = 1.0 / atten2;
					
		// light #0
		half diff = saturate (dot (WN, normalize(toLight0)));
		lightColor += unity_LightColor[0].rgb * (diff * atten2.x);

		// light #1
		diff = saturate (dot (WN, normalize(toLight1)));
		lightColor += unity_LightColor[1].rgb * (diff * atten2.y);

		return lightColor * 1.75 + 0.2;
	}	
				
	ENDCG
	
	SubShader {
		LOD 300
		Lighting on
		Tags { "RenderType"="Opaque" "Reflection" = "RenderReflectionOpaque" "Queue"="Geometry" }	
				
		Pass {
	
			CGPROGRAM
	
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			
			struct v2f_full
			{
				half4 pos : POSITION;
				half3 color : TEXCOORD0;
				half2 uv : TEXCOORD1;
				half3 viewDir : TEXCOORD2;
				half3 tsBase0 : TEXCOORD3;
				half3 tsBase1 : TEXCOORD4;
				half3 tsBase2 : TEXCOORD5;
			};
			
			v2f_full vert (appdata_full v)
			{
				v2f_full o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldNormal = mul((half3x3)unity_ObjectToWorld, v.normal.xyz);
				
				o.color = VertexLightsWorldSpace(worldPos, worldNormal);
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.viewDir = (_WorldSpaceCameraPos.xyz - worldPos);
				
				WriteTangentSpaceData(v, o.tsBase0,o.tsBase1,o.tsBase2);				
				
				return o; 
			}
			
			fixed4 frag (v2f_full i) : COLOR 
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				half3 nrml = UnpackNormal(tex2D(_BumpMap, i.uv.xy));
				half3 bumpedNormal = half3(dot(i.tsBase0,nrml), dot(i.tsBase1,nrml), dot(i.tsBase2,nrml));		
				half3 reflDir = reflect(i.viewDir, bumpedNormal);
				fixed4 refl = texCUBE (_Cube, reflDir);
				fixed4 outColor = half4((i.color + tex.a * _SelfIllumStrength) * tex, 1.0);
				outColor += refl * 3.0 * saturate(tex.g - 0.25) * (1.0 - tex.a);
				return outColor;	
			}
			
			ENDCG
		}
	}		
	
	SubShader {
		LOD 190
		Lighting on
		Tags { "RenderType"="Opaque" "Reflection" = "RenderReflectionOpaque" "Queue"="Geometry" }	
				
		Pass {
	
			CGPROGRAM
	
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
						
			struct v2f
			{
				half4 pos : POSITION;
				half4 color : TEXCOORD0;
				half2 uv : TEXCOORD1;
				half3 reflDir : TEXCOORD2;
			};
					
			v2f vert (appdata_base v)
			{
				v2f o;			
				o.color = (unity_LightColor[0] + unity_LightColor[1]) * 0.2; // heuristic for the lighting, works 'ok' in AngryBots, but is not a general solution
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.reflDir = WorldSpaceViewDir (v.vertex);
				o.reflDir = reflect (o.reflDir, mul((half3x3)unity_ObjectToWorld, v.normal.xyz));
				return o; 
			}
			
			fixed4 frag (v2f i) : COLOR 
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				fixed4 refl = texCUBE (_Cube, i.reflDir);
				fixed4 outColor = (i.color + tex.a * _SelfIllumStrength) * tex;
				outColor += refl * 3.0 * saturate(tex.g - 0.25) * (1.0 - tex.a);
				return outColor;				
			}
			
			ENDCG
		}	
	}
}
