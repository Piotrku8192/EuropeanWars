Shader "Custom/ProvinceShader"
{
    Properties
    {
        [Header(Main settngs)]
        _Scale("Map scale", Float) = 0.5
        _Light("Light direction", Vector) = (0, -1, -1, 0)
        _LightMin("Light minimum value for contrast", Range(0, 1)) = 0.4
        _LightAdd("Add light intensity", Float) = 0

        [Space]

        _HeightMapTex("Height map", 2D) = "white" {}
        [Normal] _NormalMapTex("Normal map", 2D) = "bump" {}

        [Space]

        _GroundRepeat("Ground repeat densitiy", Float) = 0.1

        [Header(Fog of war settings)]
        [NoScaleOffset] _FogOfWarTex("Fog of war texture", 2D) = "white" {}
        _FogOfWarRepeat("Fog of war repeat density", Float) = 0.5
        _FogOfWarVelocity("Fog of war velocities, xy and zw", Vector) = (0.02, 0.02, 0.02, 0.02)
        _FogOfWarAlpha("Alpha multiplier for the fog", Float) = 0.5

        [Header(Grass texture settings)]
        [NoScaleOffset] _GrassTex("Grass texture", 2D) = "white" {}
        _GrassRepeat("Grass texture repeat factor", Float) = 1
        _GrassIntensity("Grass intensity", Range(0, 1)) = 0.3

        [Header(Rocks texture settings)]
        [NoScaleOffset] _RocksTex("Rocks texture", 2D) = "white" {}
        _RocksRepeat("Rocks texture repeat factor", Float) = 1
        _RocksIntensity("Rocks intensity", Range(0, 1)) = 0.6

        [Header(Snow texture settings)]
        [NoScaleOffset] _SnowTex("Snow texture", 2D) = "white" {}
        _SnowRepeat("Snow texture repeat factor", Float) = 1
        _SnowIntensity("Snow intensity", Range(0, 1)) = 0.8

        [Header(Mountains appearance settings)]
        _MountStartHeight("Mountains start height", Range(0, 1)) = 0.75
        _MountEndHeight("Mountains end height", Range(0, 1)) = 0.9

        [Space]

        _SnowStartHeight("Snow start height", Range(0, 1)) = 0.85
        _SnowEndHeight("Snow end height", Range(0, 1)) = 0.95

            // _MainTex ("Texture", 2D) = "white" {}
            [HideInInspector] _Color("Color", Color) = (1,1,1,1)
            [HideInInspector] _FogOfWar("_FogOfWar", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

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
                    float4 color : COLOR; // TODO: remove?
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0; // Streched across all sprites (0 to 1)
                    float2 world : TEXCOORD1; // Real engine coordinates (scaled)
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR; // TODO: remove?
                };

                float _Scale;
                sampler2D _HeightMapTex;
                sampler2D _NormalMapTex;
                float2 _HeightMapTex_TexelSize;
                float _GroundRepeat;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.world = mul(unity_ObjectToWorld, v.vertex).xy * _GroundRepeat * 0.01;
                    o.uv = float2(v.uv.x, -v.uv.y) * _HeightMapTex_TexelSize * _Scale;
                    o.uv.y = 1 - o.uv.y;
                    o.color = v.color; // TODO: remove?
                    return o;
                }

                fixed4 _Color;
                float _FogOfWar;

                float4 _Light;
                float _LightMin;
                float _LightAdd;

                sampler2D _FogOfWarTex;
                float _FogOfWarRepeat;
                float4 _FogOfWarVelocity;
                float _FogOfWarAlpha;

                sampler2D _GrassTex;
                float _GrassIntensity;
                float _GrassRepeat;

                sampler2D _RocksTex;
                float _RocksRepeat;
                float _RocksIntensity;

                sampler2D _SnowTex;
                float _SnowRepeat;
                float _SnowIntensity;

                float _MountStartHeight;
                float _MountEndHeight;
                float _SnowStartHeight;
                float _SnowEndHeight;

                fixed4 frag(v2f i) : SV_Target
                {
                    // SAMPLING
                    fixed4 col = _Color;
                    float height = tex2D(_HeightMapTex, i.uv).r; // Height is the same for every RGB channel
                    fixed4 normal = normalize(tex2D(_NormalMapTex, i.uv));
                    fixed4 fog1Tex = tex2D(_FogOfWarTex, (i.world * _FogOfWarRepeat) + (_FogOfWarVelocity.xy * _Time.y));
                    fixed4 fog2Tex = tex2D(_FogOfWarTex, -(i.world * _FogOfWarRepeat) + (_FogOfWarVelocity.zw * _Time.y) + 0.5);
                    fixed4 grassTex = tex2D(_GrassTex, i.world * _GrassRepeat);
                    fixed4 rocksTex = tex2D(_RocksTex, i.world * _RocksRepeat);
                    fixed4 snowTex = tex2D(_SnowTex, i.world * _SnowRepeat);

                    // FOG OF WAR
                    float fogFinal = max(fog1Tex.a, fog2Tex.a);
                    fogFinal *= _FogOfWarAlpha * _FogOfWar;

                    // LIGHT CALC
                    float lightIntensity = max(0, dot(normal, normalize(-_Light))); // Standard directional light calculation
                    lightIntensity = (lightIntensity - _LightMin) / (1 - _LightMin); // Remaping light: 0 to 1 -> _LightMin to 1 (can be negative)
                    lightIntensity += _LightAdd; // Fixed light addition

                    // LAYERS
                    float isMount = clamp((height - _MountStartHeight) / (_MountEndHeight - _MountStartHeight), 0, 1); // 1 is full mountain, 0 is no mountain
                    float isSnow = clamp((height - _SnowStartHeight) / (_SnowEndHeight - _SnowStartHeight), 0, 1); // 1 is full snow, 0 is no snow

                    fixed4 groundTex = lerp(grassTex, rocksTex, isMount);
                    groundTex = lerp(groundTex, snowTex, isSnow);

                    float groundIntensity = lerp(_GrassIntensity, _RocksIntensity, isMount);
                    groundIntensity = lerp(groundIntensity, _SnowIntensity, isSnow);

                    fixed4 layersCombined = lerp(col, groundTex, groundIntensity) * lightIntensity;

                    return lerp(layersCombined, fixed4(1, 1, 1, 1), fogFinal);
                }
                ENDCG
            }
        }
}
