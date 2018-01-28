// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Star"{
	SubShader{
		Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off
		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct data {
					float4 vertex : POSITION;
					fixed4 color: COLOR;
				};

				data vert (data v) {
					v.vertex = UnityObjectToClipPos(v.vertex);
					return v;
				}

				fixed4 frag(data f) : COLOR {
					return f.color;
				}
			ENDCG
		}
	}
}