Shader "Unlit/Fire"
{
    Properties
    {
        _ColorBase ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _ColorTop("Color", Color) = (0.6,0.6,0,1)
        _noise ("noise", 2D) = "white" {}
        _mask ("mask", 2D) = "white" {}
        _alpha ("alpha", 2D) = "white" {}
        _alphaStrength("alpha strength", Range(0,4)) = 1 
        _FireBase ("TornadoBase", Range(0, 8)) = 2.643478
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _VSpeed("V Speed", Range(-4,4)) = -2
    }
    SubShader
    {
        Tags {  "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off

        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform float4 _ColorBase;
            uniform float4 _ColorTop;
            uniform float _VSpeed;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform sampler2D _alpha; uniform float4 _alpha_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _FireBase;
            uniform float _alphaStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv, _mask);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;       
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 shape = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask)); //node_3193
                float4 time = _Time; //node_102
                float2 noiseAuv = (i.uv0+time.g*float2(0.15,_VSpeed)); //node_7544
                float4 noiseA = tex2D(_noise,TRANSFORM_TEX(noiseAuv, _noise)); //node_9548
                float2 noiseBuv = ((i.uv0*1.4)+time.g*float2(-0.4,-0.6)); //node_165
                float4 noiseB = tex2D(_noise,TRANSFORM_TEX(noiseBuv, _noise)); //node_7341
                float2 noiseCuv = ((i.uv0*2)+time.g*float2(0,-2));
                float4 noiseC = tex2D(_noise,TRANSFORM_TEX(noiseCuv, _noise));
                float3 finalClip = ((shape.r*2.0)*((((noiseA.rgb*2)*noiseB.rgb)*noiseC.rgb)*2+pow((1.0 - i.uv0.g),_FireBase))); //node_6139
                clip(finalClip - 0.5);
////// Lighting:
////// Emissive:
                float3 noisecCol = (((noiseA.rgb*2)*noiseB.rgb)*noiseC.rgb)*2;
                float3 emissive = lerp(_ColorBase.rgb,_ColorTop.rgb,i.uv0.g);
                float3 finalColor = emissive+(noisecCol.rgb);
                float4 opacityTex = tex2D(_alpha,TRANSFORM_TEX(i.uv0, _alpha)); //node_5185
                return fixed4(finalColor,opacityTex.r*_alphaStrength);
            }
            ENDCG
        }
    }
}
