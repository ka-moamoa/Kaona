Shader "Custom/RippleShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _RippleParams ("Ripple Parameters", Vector) = (0.5, 0.5, 1.0, 1.0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _RippleParams;

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_MainTex;
            uv -= _RippleParams.xy;
            float dist = length(uv);

            // Add ripple effect based on distance and time
            float ripple = sin(dist * _RippleParams.z - _Time.y * _RippleParams.w);
            
            // Apply ripple effect to the texture
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex + ripple * uv).rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
