Shader "Unlit/RippleEffect" {
    Properties {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1
        _WaveAmp ("Wave Amplitude", Range(0,0.5)) = 0.25
    }
    SubShader {
        // Subshader tags
        Tags {
            "RenderType"="Opaque" // Tag to inform the render pipeline of what type it is
        }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.283185307179586

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;

            // Automatically filled out by Unity
            struct MeshData { // Per vertex
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                // float4 color : COLOR; // Vertex colors
                // float4 tangent : TANGENT; // tangent direction (xyz) tangent sign (w )
                float2 uv0 : TEXCOORD0; // uv0 coords
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float GetWave( float2 uv ) {
                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                float waves = cos((radialDistance - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                return waves * (1 - radialDistance);
            }

            Interpolators vert (MeshData v) {
                Interpolators o;

                float wave = cos((v.uv0.y - _Time.y * 0.1) * TAU * 5);
                // float wave2 = cos((v.uv0.x - _Time.y * 0.1) * TAU * 5);

                v.vertex.y = GetWave(v.uv0) * _WaveAmp;

                o.vertex = UnityObjectToClipPos(v.vertex); // Local space to clip space
                o.normal = v.normals;
                o.uv = v.uv0;
                return o;
            }

            float InverseLerp(float a, float b, float v) {
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target {
                return GetWave(i.uv);
            }
            ENDCG
        }
    }
}
