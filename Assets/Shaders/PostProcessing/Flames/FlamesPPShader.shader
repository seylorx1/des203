Shader "Hidden/Custom/FlamesPP" {
	HLSLINCLUDE
		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		#include "Assets/Shaders/_Shared/PerlinRepeat.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

		float _FlameAmount;
		float3 _FlameBaseColor;
		float3 _FlameMidColor;
		float3 _FlameTipColor;

		float FlameHeight(
			in float angle,
			in float flameSpinSpeed,
			in float flameCount,
			in float flameScale,
			in float warpCycleSpeed,
			in float warpCycleScale) {

			return
				sin((angle + _Time.y * flameSpinSpeed) * flameCount) *
				Noise(float2(angle / (2.0 * PI), _Time.y * warpCycleSpeed), warpCycleScale) *
				flameScale;
		}

		float FlameLayer(
			in float blurFactor,
			in float d,
			in float flameHeight) {

			return 1.0 - smoothstep(0.0, blurFactor, 0.6 - (d + flameHeight));
		}

		float4 Frag(VaryingsDefault i) : SV_Target{
			float d = length(i.texcoord - float2(0.5, 0.5)) * (0.7 + (0.3 * _FlameAmount)); //create circle
			float angle = atan2(i.texcoord.x - 0.5, i.texcoord.y - 0.5) + PI; //angle of fragment to centre

			float layer1 = FlameLayer(0.03, d, FlameHeight(
				angle,
				0.03,    //spin speed
				12.0,    //count
				0.1 * _FlameAmount,     //scale
				0.5,     //warp cycle speed
				20.0)   //warp cycle scale
			);

			float layer2 = FlameLayer(0.1, d, FlameHeight(
				angle,
				0.03,    //spin speed
				12.0,    //count
				0.1 * _FlameAmount,     //scale
				0.5,     //warp cycle speed
				20.0)   //warp cycle scale
			);

			float layer3 = FlameLayer(0.03, d, FlameHeight(
				angle,
				0.03,    //spin speed
				12.0,    //count
				0.25 * _FlameAmount, //scale
				0.5,     //warp cycle speed
				20.0)   //warp cycle scale
			);

			float4 colour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

			colour.rgb =
				lerp(
					colour.rgb, //Background
					lerp(
						_FlameTipColor, //"Yellow"
						lerp(
							_FlameMidColor, //"Orange"
							_FlameBaseColor, //"Deep Red"
							float3(layer1.xxx)),
						float3(layer2.xxx)),
					float3(layer3.xxx));

			return float4(colour);
		}
	ENDHLSL

	Subshader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			HLSLPROGRAM
				#pragma vertex VertDefault
				#pragma fragment Frag
			ENDHLSL
		}
	}
}