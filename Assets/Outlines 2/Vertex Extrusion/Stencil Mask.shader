Shader "Hidden/Stencil Mask"
{
    Properties {  _MainTex ("Texture", 2D) = "white" {} }

    SubShader
    {
        Pass
        {
            Name "STENCIL MASK"

            Stencil 
            {
                Ref 5 // compare this value to current buffer
                Comp Always
                Pass Replace // if comp passes then replace buffer with ref
            }

           
            ColorMask 0 // Don't write into color buffer
            ZWrite Off // Don't write into depth buffer
            ZTest Always // Always write stencil for mask

            // Note: ZTest and ZWrite are set in inspector
            // Cull

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            #pragma target 4.5

            struct appdata
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f 
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            { 
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.positionCS = UnityObjectToClipPos(v.positionOS);

                return o;
            }
            
            void frag () {}
            ENDHLSL
        }
    }
}