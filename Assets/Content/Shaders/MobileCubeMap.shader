    Shader "Lipsar/Cubemap"
    {
        Properties {
          _MainTex ("Texture", 2D) = "white" {}
          _Cube ("Cube env tex", CUBE) = "black" {}
          _MixPower ("Mix Power", Range (0.01, 1)) = 0.5
        }
        SubShader {
          Tags { "RenderType" = "Opaque" }
          CGPROGRAM
          #pragma surface surf Lambert
          struct Input {
              float2 uv_MainTex;
              float3 worldRefl;
          };
          sampler2D _MainTex;
          samplerCUBE _Cube;
          fixed _MixPower;
     
          void surf (Input IN, inout SurfaceOutput o) {
              o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
              o.Emission = texCUBE (_Cube, IN.worldRefl).rgb * tex2D (_MainTex, IN.uv_MainTex).a * _MixPower;
          }
          ENDCG
        }
        Fallback "Diffuse"
    }
