Shader "Azerilo/URP Toon Two Sided"
    {
        Properties
        {
            [NoScaleOffset]Texture2D_BC800E8("Texture", 2D) = "white" {}
            Color_BAB95EAD("MainColor", Color) = (1, 1, 1, 0)
            [HDR]Color_3386DC50("AmbientColor", Color) = (0.6603774, 0.6603774, 0.6603774, 0)
            [HDR]Color_108E5520("ُُSpecularColor", Color) = (1, 1, 1, 0)
            Vector1_414362D2("Glossiness", Float) = 5
            [ToggleUI]Boolean_4CE87B0C("Specular", Float) = 0
            [HDR]Color_2295CD4B("RimColor", Color) = (1, 1, 1, 0)
            Vector1_816F2286("RimAmount", Range(0, 1)) = 0.7
            Vector1_1EF48D8E("RimThreshold", Range(0, 1)) = 0.1
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Opaque"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Geometry"
            }
            Pass
            {
                Name "Pass"
                Tags
                {
                    // LightMode: <None>
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma shader_feature _ _SAMPLE_GI
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_VIEWDIRECTION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_UNLIT
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 texCoord0;
                    float3 viewDirectionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpaceNormal;
                    float3 WorldSpaceViewDirection;
                    float3 AbsoluteWorldSpacePosition;
                    float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    float3 interp3 : TEXCOORD3;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.texCoord0;
                    output.interp3.xyz =  input.viewDirectionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.texCoord0 = input.interp2.xyzw;
                    output.viewDirectionWS = input.interp3.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
                SAMPLER(_SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_Sampler_3_Linear_Repeat);
    
                // Graph Functions
                
                void Unity_Normalize_float3(float3 In, out float3 Out)
                {
                    Out = normalize(In);
                }
                
                // d0475a86682563c846748f3f32730e3a
                #include "CustomLighting.hlsl"
                
                void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
                {
                    Out = dot(A, B);
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Power_float(float A, float B, out float Out)
                {
                    Out = pow(A, B);
                }
                
                void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_BC800E8, samplerTexture2D_BC800E8, IN.uv0.xy);
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_R_4 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.r;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_G_5 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.g;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_B_6 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.b;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_A_7 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.a;
                    float4 _Property_e4fcafea3a74648a96530061da6fc43f_Out_0 = Color_BAB95EAD;
                    float4 _Property_32d610c86cfd1c84b11090c469539dc4_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_3386DC50) : Color_3386DC50;
                    float3 _Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1;
                    Unity_Normalize_float3(IN.WorldSpaceNormal, _Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1);
                    half3 _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1;
                    half3 _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2;
                    half _CustomFunction_979c705271df2b8d97d55e1f8622e68f_DistanceAtten_3;
                    half _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4;
                    MainLight_half(IN.AbsoluteWorldSpacePosition, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_DistanceAtten_3, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4);
                    float _DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2);
                    float _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2;
                    Unity_Multiply_float(_DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4, _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2);
                    float _Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3;
                    Unity_Smoothstep_float(0, 0.01, _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2, _Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3);
                    float3 _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2;
                    Unity_Multiply_float((_Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3.xxx), _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2, _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2);
                    float3 _Add_df813e234338918fb559ff39b2816ffd_Out_2;
                    Unity_Add_float3((_Property_32d610c86cfd1c84b11090c469539dc4_Out_0.xyz), _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2, _Add_df813e234338918fb559ff39b2816ffd_Out_2);
                    float _Property_2c8ab9d6979bb082b134ce808c88e37e_Out_0 = Boolean_4CE87B0C;
                    float3 _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1;
                    Unity_Normalize_float3(IN.WorldSpaceViewDirection, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1);
                    float3 _Add_9a99371aefbbe4829b48f0a558371c96_Out_2;
                    Unity_Add_float3(_CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1, _Add_9a99371aefbbe4829b48f0a558371c96_Out_2);
                    float3 _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1;
                    Unity_Normalize_float3(_Add_9a99371aefbbe4829b48f0a558371c96_Out_2, _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1);
                    float _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1, _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2);
                    float _Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2;
                    Unity_Multiply_float(_Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3, _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2, _Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2);
                    float _Property_7bc7bcc3a19d2382a4ec50cf16febf4f_Out_0 = Vector1_414362D2;
                    float _Power_a282083dad0c4a8e978c6c60a3573223_Out_2;
                    Unity_Power_float(_Property_7bc7bcc3a19d2382a4ec50cf16febf4f_Out_0, 2, _Power_a282083dad0c4a8e978c6c60a3573223_Out_2);
                    float _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2;
                    Unity_Power_float(_Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2, _Power_a282083dad0c4a8e978c6c60a3573223_Out_2, _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2);
                    float _Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3;
                    Unity_Smoothstep_float(0.005, 0.01, _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2, _Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3);
                    float4 _Property_3be80d5cbf4ba68995f0fb0136ee9a56_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_108E5520) : Color_108E5520;
                    float4 _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2;
                    Unity_Multiply_float((_Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3.xxxx), _Property_3be80d5cbf4ba68995f0fb0136ee9a56_Out_0, _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2);
                    float4 _Branch_08581e56ec4df589bd6fc31eab16454e_Out_3;
                    Unity_Branch_float4(_Property_2c8ab9d6979bb082b134ce808c88e37e_Out_0, _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2, float4(0, 0, 0, 0), _Branch_08581e56ec4df589bd6fc31eab16454e_Out_3);
                    float3 _Add_920618947478be8dbf59e8575cc8db18_Out_2;
                    Unity_Add_float3(_Add_df813e234338918fb559ff39b2816ffd_Out_2, (_Branch_08581e56ec4df589bd6fc31eab16454e_Out_3.xyz), _Add_920618947478be8dbf59e8575cc8db18_Out_2);
                    float _Property_fff74132c3811e8b98ac169227dd460d_Out_0 = Vector1_816F2286;
                    float _Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2;
                    Unity_Subtract_float(_Property_fff74132c3811e8b98ac169227dd460d_Out_0, 0.01, _Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2);
                    float _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2;
                    Unity_Add_float(_Property_fff74132c3811e8b98ac169227dd460d_Out_0, 0.01, _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2);
                    float _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1, _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2);
                    float _Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2;
                    Unity_Subtract_float(1, _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2, _Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2);
                    float _Property_c1a99b09312298829c7ab28abc0de9b2_Out_0 = Vector1_1EF48D8E;
                    float _Power_dd4364642be2d883962c96774ecacf6f_Out_2;
                    Unity_Power_float(_DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2, _Property_c1a99b09312298829c7ab28abc0de9b2_Out_0, _Power_dd4364642be2d883962c96774ecacf6f_Out_2);
                    float _Multiply_79a997a262264b809262f42320caa99c_Out_2;
                    Unity_Multiply_float(_Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2, _Power_dd4364642be2d883962c96774ecacf6f_Out_2, _Multiply_79a997a262264b809262f42320caa99c_Out_2);
                    float _Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3;
                    Unity_Smoothstep_float(_Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2, _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2, _Multiply_79a997a262264b809262f42320caa99c_Out_2, _Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3);
                    float4 _Property_0f2d0fa4136d80859fe57638f077c61a_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_2295CD4B) : Color_2295CD4B;
                    float4 _Multiply_fca2a44acffa808a80262588a371c2ea_Out_2;
                    Unity_Multiply_float((_Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3.xxxx), _Property_0f2d0fa4136d80859fe57638f077c61a_Out_0, _Multiply_fca2a44acffa808a80262588a371c2ea_Out_2);
                    float3 _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2;
                    Unity_Add_float3(_Add_920618947478be8dbf59e8575cc8db18_Out_2, (_Multiply_fca2a44acffa808a80262588a371c2ea_Out_2.xyz), _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2);
                    float3 _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2;
                    Unity_Multiply_float((_Property_e4fcafea3a74648a96530061da6fc43f_Out_0.xyz), _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2, _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2);
                    float3 _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2;
                    Unity_Multiply_float((_SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.xyz), _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2, _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2);
                    surface.BaseColor = _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                	float3 unnormalizedNormalWS = input.normalWS;
                    const float renormFactor = 1.0 / length(unnormalizedNormalWS);
                
                
                    output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
                
                
                    output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
                    output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
                    output.uv0 =                         input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SHADOWCASTER
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
    
                // Graph Functions
                // GraphFunctions: <None>
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
    
                // Graph Functions
                // GraphFunctions: <None>
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
    
                ENDHLSL
            }
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Opaque"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Geometry"
            }
            Pass
            {
                Name "Pass"
                Tags
                {
                    // LightMode: <None>
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma shader_feature _ _SAMPLE_GI
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_VIEWDIRECTION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_UNLIT
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 texCoord0;
                    float3 viewDirectionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpaceNormal;
                    float3 WorldSpaceViewDirection;
                    float3 AbsoluteWorldSpacePosition;
                    float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    float3 interp3 : TEXCOORD3;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.texCoord0;
                    output.interp3.xyz =  input.viewDirectionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.texCoord0 = input.interp2.xyzw;
                    output.viewDirectionWS = input.interp3.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
                SAMPLER(_SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_Sampler_3_Linear_Repeat);
    
                // Graph Functions
                
                void Unity_Normalize_float3(float3 In, out float3 Out)
                {
                    Out = normalize(In);
                }
                
                // d0475a86682563c846748f3f32730e3a
                #include "CustomLighting.hlsl"
                
                void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
                {
                    Out = dot(A, B);
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Power_float(float A, float B, out float Out)
                {
                    Out = pow(A, B);
                }
                
                void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_BC800E8, samplerTexture2D_BC800E8, IN.uv0.xy);
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_R_4 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.r;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_G_5 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.g;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_B_6 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.b;
                    float _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_A_7 = _SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.a;
                    float4 _Property_e4fcafea3a74648a96530061da6fc43f_Out_0 = Color_BAB95EAD;
                    float4 _Property_32d610c86cfd1c84b11090c469539dc4_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_3386DC50) : Color_3386DC50;
                    float3 _Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1;
                    Unity_Normalize_float3(IN.WorldSpaceNormal, _Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1);
                    half3 _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1;
                    half3 _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2;
                    half _CustomFunction_979c705271df2b8d97d55e1f8622e68f_DistanceAtten_3;
                    half _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4;
                    MainLight_half(IN.AbsoluteWorldSpacePosition, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_DistanceAtten_3, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4);
                    float _DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2);
                    float _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2;
                    Unity_Multiply_float(_DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2, _CustomFunction_979c705271df2b8d97d55e1f8622e68f_ShadowAtten_4, _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2);
                    float _Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3;
                    Unity_Smoothstep_float(0, 0.01, _Multiply_a1493512f1c3448cab3ead5414694fe3_Out_2, _Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3);
                    float3 _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2;
                    Unity_Multiply_float((_Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3.xxx), _CustomFunction_979c705271df2b8d97d55e1f8622e68f_Color_2, _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2);
                    float3 _Add_df813e234338918fb559ff39b2816ffd_Out_2;
                    Unity_Add_float3((_Property_32d610c86cfd1c84b11090c469539dc4_Out_0.xyz), _Multiply_ac52bd81eeac0c888a30c1aa399454d3_Out_2, _Add_df813e234338918fb559ff39b2816ffd_Out_2);
                    float _Property_2c8ab9d6979bb082b134ce808c88e37e_Out_0 = Boolean_4CE87B0C;
                    float3 _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1;
                    Unity_Normalize_float3(IN.WorldSpaceViewDirection, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1);
                    float3 _Add_9a99371aefbbe4829b48f0a558371c96_Out_2;
                    Unity_Add_float3(_CustomFunction_979c705271df2b8d97d55e1f8622e68f_Direction_1, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1, _Add_9a99371aefbbe4829b48f0a558371c96_Out_2);
                    float3 _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1;
                    Unity_Normalize_float3(_Add_9a99371aefbbe4829b48f0a558371c96_Out_2, _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1);
                    float _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _Normalize_c482254b7b2bf2858f842de3585ce813_Out_1, _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2);
                    float _Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2;
                    Unity_Multiply_float(_Smoothstep_6142159f9f3a378f8cd7aaa738a87d24_Out_3, _DotProduct_660a71b6474e568f9b78d05be756faaf_Out_2, _Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2);
                    float _Property_7bc7bcc3a19d2382a4ec50cf16febf4f_Out_0 = Vector1_414362D2;
                    float _Power_a282083dad0c4a8e978c6c60a3573223_Out_2;
                    Unity_Power_float(_Property_7bc7bcc3a19d2382a4ec50cf16febf4f_Out_0, 2, _Power_a282083dad0c4a8e978c6c60a3573223_Out_2);
                    float _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2;
                    Unity_Power_float(_Multiply_89a000b3b9f0c9828879d660d76e8431_Out_2, _Power_a282083dad0c4a8e978c6c60a3573223_Out_2, _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2);
                    float _Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3;
                    Unity_Smoothstep_float(0.005, 0.01, _Power_f1740eb8fd3bce8ca40261bd52e6852d_Out_2, _Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3);
                    float4 _Property_3be80d5cbf4ba68995f0fb0136ee9a56_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_108E5520) : Color_108E5520;
                    float4 _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2;
                    Unity_Multiply_float((_Smoothstep_21ed78dc6c129886b1e4cef0d07ae352_Out_3.xxxx), _Property_3be80d5cbf4ba68995f0fb0136ee9a56_Out_0, _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2);
                    float4 _Branch_08581e56ec4df589bd6fc31eab16454e_Out_3;
                    Unity_Branch_float4(_Property_2c8ab9d6979bb082b134ce808c88e37e_Out_0, _Multiply_2ef17297a556e482ad531728180ddc4c_Out_2, float4(0, 0, 0, 0), _Branch_08581e56ec4df589bd6fc31eab16454e_Out_3);
                    float3 _Add_920618947478be8dbf59e8575cc8db18_Out_2;
                    Unity_Add_float3(_Add_df813e234338918fb559ff39b2816ffd_Out_2, (_Branch_08581e56ec4df589bd6fc31eab16454e_Out_3.xyz), _Add_920618947478be8dbf59e8575cc8db18_Out_2);
                    float _Property_fff74132c3811e8b98ac169227dd460d_Out_0 = Vector1_816F2286;
                    float _Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2;
                    Unity_Subtract_float(_Property_fff74132c3811e8b98ac169227dd460d_Out_0, 0.01, _Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2);
                    float _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2;
                    Unity_Add_float(_Property_fff74132c3811e8b98ac169227dd460d_Out_0, 0.01, _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2);
                    float _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2;
                    Unity_DotProduct_float3(_Normalize_1d3823eca3b1ef88aa4a9f7000492c84_Out_1, _Normalize_99ae85b369b92f8b9f4730e436186d53_Out_1, _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2);
                    float _Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2;
                    Unity_Subtract_float(1, _DotProduct_cfe9db6360757b8889f4c53105be4470_Out_2, _Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2);
                    float _Property_c1a99b09312298829c7ab28abc0de9b2_Out_0 = Vector1_1EF48D8E;
                    float _Power_dd4364642be2d883962c96774ecacf6f_Out_2;
                    Unity_Power_float(_DotProduct_a810a332b2a4f58c99315f74c404ee3d_Out_2, _Property_c1a99b09312298829c7ab28abc0de9b2_Out_0, _Power_dd4364642be2d883962c96774ecacf6f_Out_2);
                    float _Multiply_79a997a262264b809262f42320caa99c_Out_2;
                    Unity_Multiply_float(_Subtract_09150d897a96ef8bb189bf4d06fdf6de_Out_2, _Power_dd4364642be2d883962c96774ecacf6f_Out_2, _Multiply_79a997a262264b809262f42320caa99c_Out_2);
                    float _Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3;
                    Unity_Smoothstep_float(_Subtract_980f2e241cdc3b8e9d740220684ee155_Out_2, _Add_08e9ad395b58938dbe1de5e8c8e9b832_Out_2, _Multiply_79a997a262264b809262f42320caa99c_Out_2, _Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3);
                    float4 _Property_0f2d0fa4136d80859fe57638f077c61a_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_2295CD4B) : Color_2295CD4B;
                    float4 _Multiply_fca2a44acffa808a80262588a371c2ea_Out_2;
                    Unity_Multiply_float((_Smoothstep_4ecf25e5f03d548182e0b0491234c940_Out_3.xxxx), _Property_0f2d0fa4136d80859fe57638f077c61a_Out_0, _Multiply_fca2a44acffa808a80262588a371c2ea_Out_2);
                    float3 _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2;
                    Unity_Add_float3(_Add_920618947478be8dbf59e8575cc8db18_Out_2, (_Multiply_fca2a44acffa808a80262588a371c2ea_Out_2.xyz), _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2);
                    float3 _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2;
                    Unity_Multiply_float((_Property_e4fcafea3a74648a96530061da6fc43f_Out_0.xyz), _Add_9016ba5fc5619b86b5618940f117d6e9_Out_2, _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2);
                    float3 _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2;
                    Unity_Multiply_float((_SampleTexture2D_fe38205ee2f5d58cb1d86d741ee6981d_RGBA_0.xyz), _Multiply_e78e619217a8bf8494f328a500ca1eb3_Out_2, _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2);
                    surface.BaseColor = _Multiply_c8b68681b900548b8e1b66d8021888d6_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                	float3 unnormalizedNormalWS = input.normalWS;
                    const float renormFactor = 1.0 / length(unnormalizedNormalWS);
                
                
                    output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
                
                
                    output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
                    output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
                    output.uv0 =                         input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SHADOWCASTER
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
    
                // Graph Functions
                // GraphFunctions: <None>
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
    
                // Render State
                Cull off
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 Texture2D_BC800E8_TexelSize;
                float4 Color_BAB95EAD;
                float4 Color_3386DC50;
                float4 Color_108E5520;
                float Vector1_414362D2;
                float Boolean_4CE87B0C;
                float4 Color_2295CD4B;
                float Vector1_816F2286;
                float Vector1_1EF48D8E;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_BC800E8);
                SAMPLER(samplerTexture2D_BC800E8);
    
                // Graph Functions
                // GraphFunctions: <None>
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
    
                ENDHLSL
            }
        }

    }
