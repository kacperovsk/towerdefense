Shader "Universal Render Pipeline/2D/AuraEnergy"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        _GlowSpeed("Glow Speed", Float) = 1
        _GlowStrength("Glow Strength", Float) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _GlowSpeed;
            float _GlowStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = SampleSpriteTexture(i.uv) * _Color;

                float glow = sin(_Time.y * _GlowSpeed) * _GlowStrength + _GlowStrength;
                col.rgb += glow;

                return col;
            }
            ENDHLSL
        }
    }
}
