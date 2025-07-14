Shader "Unlit/WaveEffect" {
    Properties {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1
    }
    SubShader {
        // Subshader tags
        Tags { 
            "RenderType"="Transparent" // Tag to inform the render pipeline of what type it is 
            "Queue"="Transparent" // Changes the render order
        }

        Pass {
            Cull Off

            ZWrite Off // Disable writing to the depth buffer

            ZTest LEqual

            Blend One One // Additive blending
            // Blend DstColor Zero // Additive blending

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.283185307179586

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;

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

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Local space to clip space 
                o.normal = v.normals;
                o.uv = v.uv0;
                return o;
            }

            float InverseLerp(float a, float b, float v) {
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target {
                // float t = saturate ( InverseLerp(_ColorStart, _ColorEnd, i.uv.x) );

                // Triangle wave
                // float t = abs(frac(i.uv.x * 5) * 2 - 1);

                // Cos Wave
                float xOffset = cos (i.uv.x * TAU * 8) * 0.01;
                float waves = cos((i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;

                float fadeOutEffect = (1 - i.uv.y);
                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);
                
                return gradient * topBottomRemover * fadeOutEffect * waves; 
                // return t * gradient * colorRemover * fadeOutEffect;
            }
            ENDCG
        }
    }
}
