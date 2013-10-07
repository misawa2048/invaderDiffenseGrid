Shader "Earth"
{
	Properties 
	{
_RimColor("_RimColor", Color) = (0.5522388,0.6054692,1,1)
_RimPower("_RimPower", Range(0.1,3) ) = 1.707772
_Glossiness("_Glossiness", Range(0.1,1) ) = 0.205
_SpecularColor("_SpecularColor", Color) = (1,1,1,1)
_MainTex("main tex", 2D) = "black" {}
_nml_o("_nml_o", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{ "Queue"="Geometry" "RenderType"="Opaque" "IgnoreProjector"="False" }

		
Cull Back
ZWrite On
ZTest LEqual
Blend SrcAlpha OneMinusSrcAlpha
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


float4 _RimColor;
float _RimPower;
float _Glossiness;
float4 _SpecularColor;
sampler2D _MainTex;
sampler2D _nml_o;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_MainTex;
float3 viewDir;
float2 uv_nml_o;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 0.5;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Pow0=pow(Fresnel0,_RimPower.xxxx);
float4 Multiply0=_RimColor * Pow0;
float4 Sampled2D1=tex2D(_nml_o,IN.uv_nml_o.xy);
float4 Multiply1=_SpecularColor * Sampled2D1.aaaa;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Sampled2D0;
o.Emission = Multiply0;
o.Alpha = 1-Fresnel0;
o.Specular = _Glossiness.xxxx;
o.Gloss = Multiply1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}