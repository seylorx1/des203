Shader "Unlit/Image Sequence"
{
    Properties
    {
        _Color("Main Color", Color) = (1.0,1.0,1.0,1.0)
        _Sequence("Image Sequence", 2DArray) = "" {}
        _SequenceLength("Sequence Length", Float) = 0.0
        _SequenceOffset("Time Offset", Float) = 0.0
        _FPS("Target FPS", Float) = 15.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma require 2darray
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            UNITY_DECLARE_TEX2DARRAY(_Sequence);
            float4 _Sequence_ST;

            float _SequenceLength;
            float _SequenceOffset;
            float _FPS;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex.xyz);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _Sequence);
                o.uv.z = floor((_SequenceOffset + (_Time.y * _FPS)) % _SequenceLength);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_Sequence, i.uv) * _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
