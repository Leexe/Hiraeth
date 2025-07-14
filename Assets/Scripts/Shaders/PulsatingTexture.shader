Shader "Unlit/PulsatingTexture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader{
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.283185307179586

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex); // object to world
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float GetWave( float2 coord ) {
                float waves = cos((coord - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                return waves * (coord);
            }

            float4 frag (Interpolators i) : SV_Target {
                float2 topDownProjection = i.worldPos.xz;

                // sample the texture
                fixed4 tex = tex2D(_MainTex, topDownProjection);

                return GetWave(tex);
            }
            ENDCG
        }
    }
}
