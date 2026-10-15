Shader "Lipsar/Diffuse Simple"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_BrightColor("Light Color", Color) = (1, 1, 1, 1)
		_DarkColor("Dark Color", Color) = (1, 1, 1, 1)
		_K("Shadow Intensity", Range(0.0, 2.0)) = 1.0
		_P("Shadow Falloff",   Range(0.0, 2.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"RenderType" = "Opaque"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase 
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Color, _LightColor0, _BrightColor, _DarkColor;
			float _K, _P;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				LIGHTING_COORDS(2,3)
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;

				output.pos = UnityObjectToClipPos(v.vertex);
				output.normal = UnityObjectToWorldNormal(v.normal);

				output.texCoord = v.texCoord;

				UNITY_TRANSFER_FOG(output,output.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(output);
				return output;
			}

			float4 frag(vertexOutput input) : SV_Target
			{
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float lightDot = clamp(dot(input.normal, lightDir), -1, 1);
				lightDot = exp(-pow(_K*(1 - lightDot), _P));
				float4 light = lerp(_DarkColor, _BrightColor, lightDot);

				float4 albedo = tex2D(_MainTex, input.texCoord.xy);

				float shadowColor = LIGHT_ATTENUATION(input);
				fixed4 color = _Color * albedo * light * shadowColor;
				UNITY_APPLY_FOG(input.fogCoord, color);
				return color;
			}
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
}