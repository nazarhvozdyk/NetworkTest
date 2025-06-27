Shader "Custom/FOVFillWorking"
{
    Properties
    {
        _ColorBase ("Base Color", Color) = (1, 0, 0, 0.2)
        _ColorFill ("Fill Color", Color) = (1, 0, 0, 1)
        _Radius ("Radius", Float) = 5
        _FillAmount ("Fill Amount", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float4 _ColorBase;
            float4 _ColorFill;
            float _Radius;
            float _FillAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.localPos.xz);
                float normalized = saturate(dist / _Radius);
                float mask = step(normalized, _FillAmount);

                float4 color = lerp(_ColorBase, _ColorFill, mask);
                return color;
            }
            ENDCG
        }
    }
}
