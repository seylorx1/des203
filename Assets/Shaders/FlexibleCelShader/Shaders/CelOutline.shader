// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

Shader "FlexibleCelShader/Cel Outline"
{
	Properties
	{
		[MaterialToggle] _ApplyPointLighting("Render Point Lights", Float) = 1.0
		_PointLightSaturation("Point Light Saturation", Range(0, 1)) = 0.5
		_PointLightScalar("Point Light Scalar", Range(0, 2)) = 0.5

		_Color("Global Color Modifier", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
		_NormalTex("Normal", 2D) = "bump" {}
		_EmmisTex("Emission", 2D) = "black" {}

		_RampLevels("Ramp Levels", Range(2, 50)) = 2
		_LightScalar("Light Scalar", Range(0, 10)) = 1

		_HighColor("High Light Color", Color) = (1, 1, 1, 1)
		_HighIntensity("High Light Intensity", Range(0, 10)) = 1.5

		_LowColor("Low Light Color", Color) = (1, 1, 1, 1)
		_LowIntensity("Low Light Intensity", Range(0, 10)) = 1

		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineSize("Outline Size", float) = 10

		_RimColor("Hard Edge Light Color", Color) = (1, 1, 1, 1)
		_RimAlpha("Hard Edge Light Brightness", Range(0, 1)) = 0
		_RimPower("Hard Edge Light Size", Range(0,1)) = 0
		_RimDropOff("Hard Edge Light Dropoff", range(0, 1)) = 0

		_FresnelColor("Soft Edge Light Color", Color) = (1,1,1,1)
		_FresnelBrightness("Soft Edge Light Brightness", Range(0, 1)) = 0
		_FresnelPower("Soft Edge Light Size", Range(0, 1)) = 0
		_FresnelShadowDropoff("Soft Edge Light Dropoff", range(0, 1)) = 0
	}

		SubShader
		{

			// This pass renders the object
			Cull back
			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
				#include "AutoLight.cginc"

				float3 rgb2hsl(in float3 c) {
					float h = 0.0;
					float s = 0.0;
					float l = 0.0;
					float r = c.r;
					float g = c.g;
					float b = c.b;
					float cMin = min(r, min(g, b));
					float cMax = max(r, max(g, b));

					l = (cMax + cMin) / 2.0;
					if (cMax > cMin) {
						float cDelta = cMax - cMin;

						//s = l < .05 ? cDelta / ( cMax + cMin ) : cDelta / ( 2.0 - ( cMax + cMin ) ); Original
						s = l < 0.0 ? cDelta / (cMax + cMin) : cDelta / (2.0 - (cMax + cMin));

						if (r == cMax) {
							h = (g - b) / cDelta;
						}
						else if (g == cMax) {
							h = 2.0 + (b - r) / cDelta;
						}
						else {
							h = 4.0 + (r - g) / cDelta;
						}

						if (h < 0.0) {
							h += 6.0;
						}
						h = h / 6.0;
					}
					return float3(h, s, l);
				}

				float3 hsl2rgb(in float3 c) {
					float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0,4.0,2.0),6.0) - 3.0) - 1.0, 0.0, 1.0);
					return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
				}

				struct v2f
				{
					float2 uv : TEXCOORD0;

					float2 uv_lm : TEXCOORD1;

					SHADOW_COORDS(1)
					float3 worldNormal : TEXCOORD2;
					float3 worldTangent : TEXCOORD3;
					float3 worldBitangent : TEXCOORD4;
					float4 worldPos : TEXCOORD5;
					float4 pos : SV_POSITION;
				};

				v2f vert(appdata_full v)
				{
					v2f o;

					// UV data
					o.uv = v.texcoord;

					// Position data
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos = mul(UNITY_MATRIX_VP, o.worldPos);

					// Normal data
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldTangent = UnityObjectToWorldNormal(v.tangent);
					o.worldBitangent = cross(o.worldTangent, o.worldNormal);

					// Compute shadows data
					TRANSFER_SHADOW(o);

					// Apply Lightmap
					o.uv_lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

					return o;
				}

				float	  _ApplyPointLighting;
				float	  _PointLightSaturation;
				float	  _PointLightScalar;
				float4    _Color;
				sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				sampler2D _NormalTex;
				uniform float4 _NormalTex_ST;
				sampler2D _EmmisTex;
				uniform float4 _EmmisTex_ST;
				int       _RampLevels;
				float     _LightScalar;
				float     _HighIntensity;
				float4    _HighColor;
				float     _LowIntensity;
				float4    _LowColor;
				float     _RimPower;
				float	  _RimAlpha;
				float4    _RimColor;
				float     _RimDropOff;
				float     _FresnelBrightness;
				float     _FresnelPower;
				float4    _FresnelColor;
				float     _FresnelShadowDropoff;

				fixed4 frag(v2f i) : SV_Target {
					_RampLevels -= 1;

					// Get view direction && light direction for rim lighting
					float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

					// Sample textures
					fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
					fixed3 tangentNormal = tex2D(_NormalTex, i.uv * _NormalTex_ST.xy + _NormalTex_ST.zw) * 2 - 1;
					fixed4 emmision = tex2D(_EmmisTex, i.uv * _EmmisTex_ST.xy + _EmmisTex_ST.zw);

					// Get normal
					float3 worldNormal = float3(i.worldTangent * tangentNormal.r + i.worldBitangent * tangentNormal.g + i.worldNormal * tangentNormal.b);

					// Rim Lighting
					half factor = dot(viewDirection, worldNormal);
					half fresnelFactor = 1 - min(pow(max(1 - factor, 0), (1 - _FresnelPower) * 10), 1);

					// Get shadow attenuation
					fixed shadow = SHADOW_ATTENUATION(i);

					// Calculate light intensity
					float intensity = dot(worldNormal, lightDirection);
					intensity = clamp(intensity * _LightScalar, 0, 1);

					// Factor in the shadow
					intensity *= shadow;

					// Determine level
					float rampLevel = round(intensity * _RampLevels);

					// Get light multiplier based on level
					float lightMultiplier = _LowIntensity + ((_HighIntensity - _LowIntensity) / (_RampLevels)) * rampLevel;

					// Get color multiplier based on level
					float4 highColor = (rampLevel / _RampLevels) * _HighColor;
					float4 lowColor = ((_RampLevels - rampLevel) / _RampLevels) * _LowColor;
					float4 mixColor = (highColor + lowColor) / 2;

					// Apply light multiplier and color
					col *= lightMultiplier;
					col *= _Color * mixColor;

					// Apply soft Fresnel
					float rampPercentSoftFresnel = 1 - ((1 - rampLevel / _RampLevels) * (1 - _FresnelShadowDropoff));
					col.rgb = col.rgb + _FresnelColor * (_FresnelBrightness * 10 - fresnelFactor * _FresnelBrightness * 10) * rampPercentSoftFresnel;

					// Apply hard rim lighting
					_RimAlpha *= 1 - ((1 - rampLevel / _RampLevels) * (1 - _RimDropOff));
					if (factor <= _RimPower) {
						col.rgb = _RimColor.rgb * _RimAlpha + col.rgb * (1 - _RimAlpha);
					}

					// Apply emmision lighting
					half eIntensity = max(emmision.r, emmision.g);
					eIntensity = max(eIntensity, emmision.b);
					col = emmision * eIntensity + col * (1 - eIntensity);

					if (_ApplyPointLighting == 1) {

						//Calculate dynamic point lighting
						float3 dynamicPointLighting = Shade4PointLights(
							unity_4LightPosX0,
							unity_4LightPosY0,
							unity_4LightPosZ0,
							unity_LightColor[0],
							unity_LightColor[1],
							unity_LightColor[2],
							unity_LightColor[3],
							unity_4LightAtten0 * 0.5,
							i.worldPos.xyz,
							worldNormal
						);	 

						float3 dynamicPointLighting_hsl = rgb2hsl(dynamicPointLighting);

						float3 dynamicPointLighting_banding = hsl2rgb(float3(
							dynamicPointLighting_hsl.x,
							_PointLightSaturation,
							round(dynamicPointLighting_hsl.z * (_RampLevels + 3)) / (_RampLevels + 3)));
							
						return float4(col + (_PointLightScalar * dynamicPointLighting_banding), 1.0);
					}
					return float4(col.rgb, 1.0);

				}

				ENDCG
			} // End Main Pass

			// This Pass Renders the outlines
			Cull Front
			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				float _OutlineSize;
				v2f vert(appdata v)
				{
					v2f o;
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					half3 worldNormal = UnityObjectToWorldNormal(v.normal);
					worldPos.xyz = worldPos.xyz + worldNormal * _OutlineSize * 0.001;
					o.vertex = mul(UNITY_MATRIX_VP, worldPos);
					return o;
				}

				float4 _OutlineColor;
				fixed4 frag(v2f i) : SV_Target
				{
					return _OutlineColor;
				}

				ENDCG
			}// End Outline Pass

			// Shadow casting
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}

			CustomEditor "CelCustomEditor"
}