Shader "Custom/StopMotionPainterUI"
{
  Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BrushTex ("Brush Texture (Grayscale/Normal)", 2D) = "bump" {} 
        
        [Header(Painting Effect)]
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _FrameRate ("Frame Rate (FPS)", Range(1, 60)) = 12.0
        _BrushScale ("Brush Scale", Range(0.1, 10)) = 1.0
        
        [Header(Centering)]
        // NOWOŚĆ: Tym suwakiem naprawisz uciekanie obrazka!
        // Dla szarej tekstury (clouds) ustaw 0.5
        // Dla czarnej tekstury z białymi kreskami ustaw ok. 0.1 - 0.2
        _Bias ("Centering Bias", Range(0, 1)) = 0.5
        
        [Header(Shape Modification)]
        _WobbleStrength ("Shape Wobble", Range(0, 20)) = 0.0
        
        // UI Masking Requirements
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BrushTex;
            float4 _ClipRect;

            float _DistortionStrength;
            float _FrameRate;
            float _BrushScale;
            float _WobbleStrength;
            float _Bias; // Nowa zmienna do centrowania

            float random(float seed)
            {
                return frac(sin(seed) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                
                // --- WOBBLE (Ruch Kształtu) ---
                // To jedyne miejsce, gdzie obrazek fizycznie się przesuwa.
                // Jak ustawisz Wobble na 0, obrazek będzie stał w miejscu.
                
                float timeStep = floor(_Time.y * _FrameRate);
                
                float rndX = random(timeStep);
                float rndY = random(timeStep + 13.5);
                
                float2 wobble = (float2(rndX, rndY) - 0.5) * _WobbleStrength;
                o.worldPosition.xy += wobble;

                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float timeStep = floor(_Time.y * _FrameRate);
                
                // Losujemy TYLKO pozycję tekstury pędzla (żeby pociągnięcia były w innym miejscu)
                float2 noiseOffset;
                noiseOffset.x = random(timeStep);
                noiseOffset.y = random(timeStep + 7.1);
                
                float2 brushUV = i.uv * _BrushScale + noiseOffset;
                fixed4 brushColor = tex2D(_BrushTex, brushUV);
                
                // --- STABILNE ZNIEKSZTAŁCENIE ---
                // Odejmujemy _Bias zamiast stałego 0.5.
                // Dzięki temu masz pełną kontrolę nad tym, w którą stronę "ciągnie" farba.
                
                float2 distortionDirection = brushColor.rg - _Bias;
                float2 distortion = distortionDirection * _DistortionStrength;
                
                fixed4 col = tex2D(_MainTex, i.uv + distortion);
                
                col *= i.color;
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                
                #ifdef UNITY_UI_ALPHACLIP
                clip (col.a - 0.001);
                #endif

                return col;
            }
            ENDCG
        }
    }
}