Shader "Custom/ProvinceShader"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _GroundTex ("Ground texture", 2D) = "white" {}
        _NormalTex ("Normal texture", 2D) = "blue" {}
        _HeightTex ("Height texture", 2D) = "white" {}
        _GradientTex ("Gradient texture", 2D) = "white" {}
        _HeightIntensity ("Height intensity", Range(0, 1)) = 0.1
        _GroundIntensity ("Ground intensity", Range(0, 1)) = 0.1
        _GroundTiling ("Ground tiling", Float) = 1
        _LightVector ("Light vector", Vector) = (0, -1, 0)
        _OutlineColor ("Outline color", Color) = (0,0,0,1)
        _OutlineSelectedColor ("Outline selected color", Color) = (1,1,1,1)
        _OutlineRange ("Outline range", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldUV = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            sampler2D _NormalTex;
            sampler2D _GroundTex;
            sampler2D _HeightTex;
            sampler2D _GradientTex;
            float _HeightIntensity;
            float _GroundIntensity;
            float _GroundTiling;
            float3 _LightVector;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float4 _OutlineSelectedColor;
            float _OutlineRange;

            static float2 offsets[20] = 
            {
                float2(+0, -1),
                float2(+0, +1),
                float2(-1, +0),
                float2(+1, +0),
                float2(-1, -1),
                float2(-1, +1),
                float2(+1, -1),
                float2(+1, +1),
                float2(+0, -2),
                float2(+0, +2),
                float2(-2, +0),
                float2(+2, +0),
                float2(-1, +2),
                float2(+1, +2),
                float2(+2, -1),
                float2(+2, +1),
                float2(-1, -2),
                float2(+1, -2),
                float2(-2, -1),
                float2(-2, +1)
			};

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= fixed4(i.color.r, i.color.g, i.color.b, 1);
                float height = tex2D(_HeightTex, i.uv).r;
                fixed4 heightCol = tex2D(_GradientTex, float2(height, 0.5));
                float heightMult = heightCol.a;
                heightCol.a = col.a;
                col = lerp(col, heightCol, _HeightIntensity * heightMult);
                fixed4 ground = tex2D(_GroundTex, i.worldUV * _GroundTiling);
                ground.a = col.a;
                col = lerp(col, ground, _GroundIntensity);
                float3 normal = normalize(tex2D(_NormalTex, i.uv).xyz * 2 - 1);
                float diffuse = max(dot(normal, _LightVector), 0);
                col = lerp(fixed4(0, 0, 0, col.a), col, 1 - diffuse);

                fixed4 outlineColor = lerp(_OutlineSelectedColor, _OutlineColor, i.color.a);

                if (col.a < 1)
                    return col;

                [loop]
                for (int j = 0; j < 20; j++)
                {
                    float2 coord = i.uv + offsets[j] * _MainTex_TexelSize * _OutlineRange;

                    if (coord.x < 0 || coord.x > 1 || coord.y < 0 || coord.y > 1 || tex2D(_MainTex, coord).a < 1)
                        return outlineColor;
				}

                fixed4 f = tex2D(_MainTex, i.uv + offsets[0] * _MainTex_TexelSize * _OutlineRange);

                if (f.a < 1)
                        return outlineColor;

                return col;
            }
            ENDCG
        }
    }
}
