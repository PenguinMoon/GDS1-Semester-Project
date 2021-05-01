Shader "Custom/ScrollTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Opacity("Opacity", Range(0,1)) = 0.5
		_TextureColor("Texture Color", Color) = (1, 1, 1, 1)
		_ScrollXSpeed("X Scroll Speed", Range(-5, 5)) = 0
		_ScrollYSpeed("Y Scroll Speed", Range(-5, 5)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
			half _Opacity;
			fixed4 _TextureColor;
			fixed _ScrollXSpeed;
			fixed _ScrollYSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				// distort the UVs
				i.uv.x += _ScrollXSpeed * _Time.y;
				i.uv.y += _ScrollYSpeed * _Time.y;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

				col.a = _Opacity;
                return col;
            }
            ENDCG
        }
    }
}
