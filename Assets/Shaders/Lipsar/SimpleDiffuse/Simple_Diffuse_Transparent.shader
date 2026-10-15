Shader "Lipsar/Simple Diffuse Transparent"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[Toggle]  _Cutout ("Enable Cutout", Float) = 1
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
		[Header(World Space Texture Position)][Space(10)]
		[Toggle] _TriplanarMapping ("Triplanar Mapping", Float) = 0
        _Sharpness ("Blend sharpness", Range(1, 64)) = 1
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]
		
		[Header(Basic Settings)][Space(10)]
		_Color("Color", Color) = (1, 1, 1, 1)
		_BrightColor("Light Color", Color) = (1, 1, 1, 1)
		_DarkColor("Dark Color", Color) = (1, 1, 1, 1)
		_K("Shadow Intensity", Range(0.0, 2.0)) = 1.0
		_P("Shadow Falloff",   Range(0.0, 2.0)) = 1.0
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]
		
		[Header(World Space Gradient)][Space(10)]
		[Toggle] _GlobalGradient ("Height Gradient", Float) = 0
        _ColorTopGradient("Top Color", Color) = (0.5,0.5,0.5,0.5)
        //_ColorBottomGradient("Bottom Color", Color) = (0.5,0.5,0.5,0.5)
		_GradientCenterX("Center X", Float) = 0
        _GradientCenterY("Center Y", Float) = 0
        _GradientSize("Size", Float) = 10.0
        _GradientAngle("Gradient Angle", Range(0, 360)) = 0
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]
		
		[Header(Local Space Gradient)][Space(10)]
		[Toggle] _LocalGradient ("Local Gradient", Float) = 0
        _ColorLocalTopGradient("Bottom Color", Color) = (0.5,0.5,0.5,0.5)
        //_ColorLocalBottomGradient("Bottom Color", Color) = (0.5,0.5,0.5,0.5)
        _ColorLocalGradienMixDelta ("Mix Delta", Range (0.01, 5)) = 0.5
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]

		[Header(Cubemap)][Space(10)]
		[Toggle] _Cubemap ("Enable", Float) = 0
        _Cube("Reflection Map", Cube) = "" {}
        _MixPower ("Mix Power", Range (0.01, 1)) = 0.5
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]

		[Header(Specular)][Space(10)]
		[Toggle] _Specular("Enable", Float) = 0
        [HDR]_SpecularColor("Specular Color",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(1,100)) = 10
		[Header(_________________________________________________________________________________________________________________________________________________________________________)]

		[Header(Global Fog)][Space(10)]
		[Toggle] _Globalfog("Disable", Float) = 0

	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"LightMode" = "ForwardBase"
			"PassFlags" = "OnlyDirectional"
		}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase // shadows
			
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"
			#include "SimpleDuffuseCore.cginc" 

			#pragma shader_feature _GLOBALGRADIENT_ON
            #pragma shader_feature _LOCALGRADIENT_ON
            #pragma shader_feature _TRIPLANARMAPPING_ON
            #pragma shader_feature _CUBEMAP_ON
            #pragma shader_feature _SPECULAR_ON
            #pragma shader_feature _GLOBALFOG_ON
			#pragma shader_feature _CUTOUT_ON
			ENDCG
		}

		// Shadow pass
		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "Lipsar.Editor.CustomShaderEditor"
}