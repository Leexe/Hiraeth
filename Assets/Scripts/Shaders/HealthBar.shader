Shader "Unlit/HealthBar" {
    Properties {
        _ColorFull ("Full Color", Color) = (0,1,0,1)
        _ColorEmpty ("Empty Color", Color) = (1,0,0,1)
        _FullValue ("Empty Color", Range(0,1)) = 0.8
        _EmptyValue ("Empty Color", Range(0,1)) = 0.2
        _Health ("Health", Range(0,1)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float4 _ColorFull;
			float4 _ColorEmpty;
			float _FullValue;
			float _EmptyValue;
			float _Health;

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv0;
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target {
				float4 col = (0,0,0,1);
				if (i.uv.x <= _Health) {
					if (_Health >= _FullValue) {
						col = _ColorFull;
					}
					else if (_Health <= _EmptyValue) {
						col = _ColorEmpty;
					}
					else {
						col = lerp(_ColorEmpty, _ColorFull, (_Health - _EmptyValue) / (1 - _EmptyValue - (1 - _FullValue)));
					}
				}
                return float4(col.xyz,1);
            }
            ENDCG
        }
    }
}
