Shader "Hidden/Vertex Extrusion"
{
    Properties
    {
        _Color ("_Color", Color) = (1, 1, 1, 1)
        _Width ("_Width", Float) = 1
        _SrcBlend ("_SrcBlend", Int) = 0
        _DstBlend ("_DstBlend", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

        Stencil 
        {
            Ref 5 // compare this value to the current buffer
            Comp NotEqual
            Pass Zero // if comp passes then replace buffer with zero
        }

        // Note: ZTest and ZWrite are set in inspector
        // Cull
   

        Blend [_SrcBlend] [_DstBlend]

        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_instancing

        #include "UnityCG.cginc"
        
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
        UNITY_DEFINE_INSTANCED_PROP(half, _Width)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct appdata 
        {
            float4 positionOS : POSITION;
            float3 normalOS : NORMAL;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct v2f 
        {
            float4 positionCS : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        half4 frag(v2f i) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(i);
            return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
        }
        ENDCG

        Pass
        {
            Name "SCALE OBJECT"

            CGPROGRAM

            v2f vert (appdata v) 
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                half width = UNITY_ACCESS_INSTANCED_PROP(Props, _Width);

                v.positionOS.xyz += v.positionOS.xyz * width;
                o.positionCS = UnityObjectToClipPos(v.positionOS);
                
                return o;
            }
            ENDCG
        }

        Pass
        {
            Name "SCALE OBJECT NORMALIZED"

            CGPROGRAM

            v2f vert (appdata v) 
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                half width = UNITY_ACCESS_INSTANCED_PROP(Props, _Width);

                v.positionOS.xyz += normalize(v.positionOS.xyz) * width;
                o.positionCS = UnityObjectToClipPos(v.positionOS);
                return o;
            }
            ENDCG
        }

        Pass
        {
            Name "EXTRUDE ALONG NORMAL 1"

            CGPROGRAM

            v2f vert (appdata v) 
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                half width = UNITY_ACCESS_INSTANCED_PROP(Props, _Width);

                v.positionOS.xyz += v.normalOS * width;
                o.positionCS = UnityObjectToClipPos(v.positionOS);
                return o;
            }
            ENDCG
        }

        Pass
        {
            Name "EXTRUDE ALONG NORMAL 2"

            CGPROGRAM

            v2f vert (appdata v) 
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                half width = UNITY_ACCESS_INSTANCED_PROP(Props, _Width);

                o.positionCS = UnityObjectToClipPos(v.positionOS);

                float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normalOS));
                float2 offset = TransformViewToProjection(normal.xy);

                o.positionCS.xy += offset * o.positionCS.z * width;
                return o;
            }
            ENDCG
        }
    }
}
