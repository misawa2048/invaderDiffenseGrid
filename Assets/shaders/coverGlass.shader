Shader "Custom/coverGlass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Background" "Queue" = "Geometry" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Color;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			float rate = clamp(cos((screenUV.y-0.5)*3.1416*1.5),0,1);
			o.Emission = c.rgb * _Color.rgb * rate;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
