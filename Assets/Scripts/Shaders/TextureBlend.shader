Shader "Unlit/Shader1" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            // Automatically filled out by Unity
            struct MeshData { // Per vertex
                float4 vertex : POSITION; // Vertex position
                float3 normals : NORMAL; 
                // float4 color : COLOR; 
                // float4 tangent : TANGENT; 
                float2 uv0 : TEXCOORD0; // uv0 coords
                // float2 uv1  : TEXCOORD1; // uv1 coords
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                // float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Local space to clip space 
                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    }
}
