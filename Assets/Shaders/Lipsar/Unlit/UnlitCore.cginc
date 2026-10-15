// Properties
sampler2D _MainTex;
fixed4 _Color;
float4 _MainTex_ST;

#if _GLOBALGRADIENT_ON
	fixed4 _ColorTopGradient;
	fixed4 _ColorBottomGradient;
	fixed  _GradientCenterX;
	fixed  _GradientCenterY;
	fixed  _GradientSize;
	fixed  _GradientAngle;
#endif

#if _LOCALGRADIENT_ON
	fixed4 _ColorLocalTopGradient;
	fixed4 _ColorLocalBottomGradient;
	fixed _ColorLocalGradienMixDelta;
#endif

#if _TRIPLANARMAPPING_ON
	fixed _Sharpness;
#endif

#if _CUBEMAP_ON
	uniform samplerCUBE _Cube;
	fixed _MixPower;
#endif

#if _SPECULAR_ON
	fixed4 _SpecularColor;
	fixed _Gloss;
#endif

#if _CUTOUT_ON
	fixed _Cutoff;
#endif

struct appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
	half3 normal : NORMAL;
	fixed4 color : COLOR;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
	half3  worldNormal : NORMAL;
	float3 worldPos : TEXCOORD1;
	fixed4 color : COLOR;
				
	SHADOW_COORDS(2)

	#if !_GLOBALFOG_ON
		UNITY_FOG_COORDS(3)
	#endif

	#if _CUBEMAP_ON
		half3 viewDir  : TEXCOORD4;
	#endif	
};

v2f vert(appdata v)
{
	v2f o;

	o.pos = UnityObjectToClipPos(v.vertex);
	o.worldNormal = UnityObjectToWorldNormal(v.normal);
	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	o.color = v.color;

	#if _CUBEMAP_ON
		float4x4 modelMatrix = unity_ObjectToWorld;
		o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;
	#endif

	TRANSFER_SHADOW(o); // shadows

	#if !_GLOBALFOG_ON
		UNITY_TRANSFER_FOG(o,o.pos);
	#endif

	return o;
}

float4 frag(v2f i) : SV_Target
{
	fixed3 normal = normalize(i.worldNormal);
	fixed4 shadowColor = saturate(SHADOW_ATTENUATION(i) / (1 - (UNITY_LIGHTMODEL_AMBIENT * (1 - SHADOW_ATTENUATION(i)))));
	fixed4 finalColor = _Color * shadowColor;

	#if _GLOBALGRADIENT_ON
		fixed angleRadians = _GradientAngle / 180.0 * 3.14159265359;
		fixed posGradRotated = (i.worldPos.x - _GradientCenterX) * sin(angleRadians) + 
			(i.worldPos.y - _GradientCenterY) * cos(angleRadians);
		fixed gradientTop = _GradientCenterY + _GradientSize * 0.5;
		half gradientFactor = saturate((gradientTop - posGradRotated) / _GradientSize);
        fixed3 gradColor = lerp(_ColorTopGradient, finalColor, gradientFactor);
        finalColor = fixed4(gradColor, 1);
	#endif

	#if _LOCALGRADIENT_ON
        fixed3 localGradColor = lerp(_ColorLocalTopGradient, finalColor, clamp(i.uv.y / _ColorLocalGradienMixDelta, -1, 1));
        finalColor = fixed4(localGradColor, 1); 
	#endif

	#if _TRIPLANARMAPPING_ON
		float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _MainTex);
		float2 uv_side = TRANSFORM_TEX(i.worldPos.zy, _MainTex);
		float2 uv_top = TRANSFORM_TEX(i.worldPos.xz, _MainTex);
		fixed4 col_front = tex2D(_MainTex, uv_front);
		fixed4 col_side = tex2D(_MainTex, uv_side);
		fixed4 col_top = tex2D(_MainTex, uv_top);

		fixed3 weights = normal;
		weights = abs(weights);
		weights = pow(weights, _Sharpness);
		weights = weights / (weights.x + weights.y + weights.z);

		col_front *= weights.z;
		col_side *= weights.x;
		col_top *= weights.y;

		finalColor *= col_front + col_side + col_top;
	#else
		finalColor *= tex2D(_MainTex, i.uv);
	#endif

	#if _CUBEMAP_ON
		fixed3 reflectedDir = reflect(i.viewDir, normal);
		finalColor *= lerp(finalColor, texCUBE(_Cube, reflectedDir), _MixPower);
	#endif

	#if _SPECULAR_ON
		float3 worldLight = UnityWorldSpaceLightDir(i.worldPos);
		float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));//Observation direction
		float3 halfDir = normalize(worldLight + viewDir);
		float3 specular = _LightColor0.rgb * _SpecularColor.rgb * pow(saturate(dot(halfDir, normal)), _Gloss);
		finalColor += float4(specular, 0);
	#endif

	#if _CUTOUT_ON
		clip(finalColor.a - _Cutoff);
	#endif

	#if !_GLOBALFOG_ON
		UNITY_APPLY_FOG(i.fogCoord, finalColor);
	#endif

	return finalColor;
}