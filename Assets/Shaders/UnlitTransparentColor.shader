Shader "Unlit/Color (Transparent)"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Off
            ZWrite Off
            Tags { "RenderType" = "Fade" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        Pass {
            Cull Off
            Tags { "RenderType" = "Transparent"}

            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.uv = v.uv;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the texture
                    fixed4 col = _Color;
                    
                    float scale = 0.52;
                    col.a =
                        saturate(
                            ceil(max(cos((i.uv.x * scale + abs(scale * 0.5 - 0.5)) * 3.1415 * 2), 0)) +
                            ceil(max(cos((i.uv.y * scale + abs(scale * 0.5 - 0.5)) * 3.1415 * 2), 0)));
                    
                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
        }
    }
}
