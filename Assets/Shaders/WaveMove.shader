Shader "Custom/WaveMovement"
{
    Properties 
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Speed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag 
                #pragma target 3.5

                #pragma multi_compile_fog

                #include "UnityCG.cginc"


				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};


                struct v2f
                {
                    float4 vertex: SV_POSITION;
                    float2 texcoord: TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Speed;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag (v2f i): SV_TARGET
                {
                    fixed4 col = tex2D(_MainTex, float2(i.texcoord.x + 5, i.texcoord.y));
                    if((col.r + col.g + col.b) < 1)
                    {
                        col.a = 0;
                    }
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }

            ENDCG
        }
    }
}