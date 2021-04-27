﻿Shader "Unlit/StencilShader"
{
    Properties
    {
        _Camera1Tex ("Camera 1 Render Texture", 2D) = "white" {}
        _Camera2Tex ("Camera 2 Render Texture", 2D) = "white" {}
        _CutoutTex ("Cutout Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Camera1Tex;
            float4 _Camera1Tex_ST;
            sampler2D _Camera2Tex;
            float4 _Camera2Tex_ST;
            sampler2D _CutoutTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Camera1Tex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_Camera1Tex, i.uv);
                if (tex2D(_CutoutTex, i.uv).r == 1){
                    col = tex2D(_Camera2Tex, i.uv);
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
