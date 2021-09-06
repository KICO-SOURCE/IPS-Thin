Shader "CrossSection/BoneShader" {
	Properties{
		_Color("Color", Color) = (0.9803922,0.9176471,0.8313726,1)
		
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}

	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry" "LightMode" = "ForwardBase" }
		Pass {
			Cull Off

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 vertex: SV_POSITION;
				float3 normal: NORMAL;
				fixed4 diff : COLOR0;
            };

			fixed4 _Color;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				// the only difference from previous shader:
				// in addition to the diffuse lighting from the main light,
				// add illumination from ambient or light probes
				// ShadeSH9 function from UnityCG.cginc evaluates it,
				// using world space normal
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
				return o;
			}

            
			fixed4 frag(v2f i) : SV_Target
			{
				return _Color * i.diff;
			}
			ENDCG
		}
	}
	//FallBack "Diffuse"
}