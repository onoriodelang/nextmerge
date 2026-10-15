Shader "MergeSlash/Neon Cyber"
{
    Properties
    {
        _Color ("Armor color", Color) = (0.04,0.08,0.16,1)
        _NeonColor ("Energy color", Color) = (0.02,0.9,1,1)
        _Grid ("Circuit grid", Range(0,1)) = 0
        _GridScale ("Circuit spacing", Float) = 1
        _RimPower ("Edge sharpness", Range(1,8)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows addshadow
        #pragma target 3.0
        #include "UnityCG.cginc"
        fixed4 _Color, _NeonColor;
        half _Grid, _GridScale, _RimPower;
        struct Input { float3 worldPos; float3 worldNormal; float3 viewDir; };
        void surf(Input IN, inout SurfaceOutput o)
        {
            // World-space lines keep their size across differently scaled level meshes.
            float2 cell = IN.worldPos.xz * _GridScale;
            float2 edge = abs(frac(cell - 0.5) - 0.5);
            float2 width = max(fwidth(cell), float2(0.001, 0.001));
            float2 line = 1.0 - smoothstep(width, width * 2.0, edge);
            half circuit = max(line.x, line.y) * _Grid * saturate(abs(IN.worldNormal.y));
            half rim = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
            o.Albedo = _Color.rgb;
            o.Emission = _NeonColor.rgb * (rim * 0.65 + circuit * 0.45);
            o.Alpha = 1;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
