// Made with Amplify Shader Editor v1.9.5.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		[NoScaleOffset]_WaveTex("WaveTex", 2D) = "white" {}
		_PlayerPos("PlayerPos", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf StandardSpecular alpha:fade keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float4 screenPosition1;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform float3 _PlayerPos;
		uniform sampler2D _WaveTex;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;


float2 voronoihash115( float2 p )
{
	
	p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
	return frac( sin( p ) *43758.5453);
}


float voronoi115( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
{
	float2 n = floor( v );
	float2 f = frac( v );
	float F1 = 8.0;
	float F2 = 8.0; float2 mg = 0;
	for ( int j = -1; j <= 1; j++ )
	{
		for ( int i = -1; i <= 1; i++ )
	 	{
	 		float2 g = float2( i, j );
	 		float2 o = voronoihash115( n + g );
			o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = max(abs(r.x), abs(r.y));
	 		if( d<F1 ) {
	 			F2 = F1;
	 			F1 = d; mg = g; mr = r; id = o;
	 		} else if( d<F2 ) {
	 			F2 = d;
	
	 		}
	 	}
	}
	return (F2 + F1) * 0.5;
}


float2 voronoihash61( float2 p )
{
	
	p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
	return frac( sin( p ) *43758.5453);
}


float voronoi61( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
{
	float2 n = floor( v );
	float2 f = frac( v );
	float F1 = 8.0;
	float F2 = 8.0; float2 mg = 0;
	for ( int j = -1; j <= 1; j++ )
	{
		for ( int i = -1; i <= 1; i++ )
	 	{
	 		float2 g = float2( i, j );
	 		float2 o = voronoihash61( n + g );
			o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = 0.5 * dot( r, r );
	 		if( d<F1 ) {
	 			F2 = F1;
	 			F1 = d; mg = g; mr = r; id = o;
	 		} else if( d<F2 ) {
	 			F2 = d;
	
	 		}
	 	}
	}
	return F1;
}


float2 voronoihash129( float2 p )
{
	
	p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
	return frac( sin( p ) *43758.5453);
}


float voronoi129( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
{
	float2 n = floor( v );
	float2 f = frac( v );
	float F1 = 8.0;
	float F2 = 8.0; float2 mg = 0;
	for ( int j = -1; j <= 1; j++ )
	{
		for ( int i = -1; i <= 1; i++ )
	 	{
	 		float2 g = float2( i, j );
	 		float2 o = voronoihash129( n + g );
			o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = 0.5 * dot( r, r );
	 		if( d<F1 ) {
	 			F2 = F1;
	 			F1 = d; mg = g; mr = r; id = o;
	 		} else if( d<F2 ) {
	 			F2 = d;
	
	 		}
	 	}
	}
	return F1;
}


float2 voronoihash95( float2 p )
{
	
	p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
	return frac( sin( p ) *43758.5453);
}


float voronoi95( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
{
	float2 n = floor( v );
	float2 f = frac( v );
	float F1 = 8.0;
	float F2 = 8.0; float2 mg = 0;
	for ( int j = -1; j <= 1; j++ )
	{
		for ( int i = -1; i <= 1; i++ )
	 	{
	 		float2 g = float2( i, j );
	 		float2 o = voronoihash95( n + g );
			o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = 0.5 * dot( r, r );
	 		if( d<F1 ) {
	 			F2 = F1;
	 			F1 = d; mg = g; mr = r; id = o;
	 		} else if( d<F2 ) {
	 			F2 = d;
	
	 		}
	 	}
	}
	return (F2 + F1) * 0.5;
}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float time115 = 17.3;
			float2 voronoiSmoothId115 = 0;
			float2 coords115 = v.texcoord.xy * ( ( _SinTime.y * 2.0 ) + 66.0 );
			float2 id115 = 0;
			float2 uv115 = 0;
			float voroi115 = voronoi115( coords115, time115, id115, uv115, 0, voronoiSmoothId115 );
			float3 appendResult111 = (float3(0.0 , ( ( voroi115 * 0.2 ) + -0.1 ) , 0.0));
			v.vertex.xyz += appendResult111;
			v.vertex.w = 1;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 vertexPos1 = ase_vertex3Pos;
			float4 ase_screenPos1 = ComputeScreenPos( UnityObjectToClipPos( vertexPos1 ) );
			o.screenPosition1 = ase_screenPos1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 color177 = IsGammaSpace() ? float4(0.726415,0.824124,1,1) : float4(0.4865308,0.645527,1,1);
			float4 color178 = IsGammaSpace() ? float4(0.4559748,0.6568932,1,1) : float4(0.1755306,0.389036,1,1);
			float4 lerpResult179 = lerp( color177 , color178 , _PlayerPos.z);
			float mulTime66 = _Time.y * 0.1;
			float time61 = 0.0;
			float2 voronoiSmoothId61 = 0;
			float2 coords61 = i.uv_texcoord * ( 600.0 + ( sin( mulTime66 ) * 5.0 ) );
			float2 id61 = 0;
			float2 uv61 = 0;
			float voroi61 = voronoi61( coords61, time61, id61, uv61, 0, voronoiSmoothId61 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 appendResult141 = (float3(_PlayerPos.x , ase_vertex3Pos.y , _PlayerPos.y));
			float saferPower165 = abs( ( distance( appendResult141 , ase_vertex3Pos ) * 20.2 ) );
			float clampResult143 = clamp( pow( saferPower165 , 0.3 ) , 0.0 , 2.0 );
			float lerpResult175 = lerp( voroi61 , 1.0 , clampResult143);
			float4 lerpResult176 = lerp( lerpResult179 , float4( 1,1,1,1 ) , step( 0.7 , lerpResult175 ));
			float time129 = ( _SinTime.x * 2.0 );
			float2 voronoiSmoothId129 = 0;
			float2 coords129 = i.uv_texcoord * 33.0;
			float2 id129 = 0;
			float2 uv129 = 0;
			float voroi129 = voronoi129( coords129, time129, id129, uv129, 0, voronoiSmoothId129 );
			float2 uv_TexCoord123 = i.uv_texcoord * ( float2( 44,44 ) + ( _SinTime.x * float2( 0.2,0.5 ) ) ) + ( voroi129 * float2( 0.3,0.3 ) );
			float4 color50 = IsGammaSpace() ? float4(0,0.5395944,0.7987421,1) : float4(0,0.2525364,0.6016975,1);
			float3 ase_worldPos = i.worldPos;
			float clampResult55 = clamp( ( distance( _WorldSpaceCameraPos , ase_worldPos ) * 0.06 ) , 0.0 , 1.0 );
			float clampResult74 = clamp( pow( clampResult55 , 2.0 ) , 0.0 , 1.0 );
			float temp_output_56_0 = ( 1.0 - clampResult74 );
			float saferPower121 = abs( ( temp_output_56_0 * 1.1 ) );
			float clampResult134 = clamp( pow( saferPower121 , 1.5 ) , 0.0 , 1.0 );
			float4 lerpResult122 = lerp( tex2D( _WaveTex, uv_TexCoord123 ) , color50 , clampResult134);
			float4 color49 = IsGammaSpace() ? float4(0,1,0.8499384,1) : float4(0,1,0.6919581,1);
			float4 ase_screenPos1 = i.screenPosition1;
			float4 ase_screenPosNorm1 = ase_screenPos1 / ase_screenPos1.w;
			ase_screenPosNorm1.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm1.z : ase_screenPosNorm1.z * 0.5 + 0.5;
			float screenDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm1.xy ));
			float distanceDepth1 = saturate( abs( ( screenDepth1 - LinearEyeDepth( ase_screenPosNorm1.z ) ) / ( 0.5 ) ) );
			float4 lerpResult48 = lerp( lerpResult122 , color49 , ( distanceDepth1 * temp_output_56_0 ));
			float saferPower58 = abs( ( 1.0 - distanceDepth1 ) );
			float clampResult169 = clamp( ( ( 1.0 - clampResult143 ) * _PlayerPos.z ) , 0.0 , 1.0 );
			float clampResult63 = clamp( ( ( pow( saferPower58 , 2.5 ) + clampResult169 ) * 2.0 * voroi61 ) , 0.0 , 1.0 );
			float4 color98 = IsGammaSpace() ? float4(0.09016647,0.5706224,0.6100628,1) : float4(0.008563933,0.2853275,0.3304187,1);
			float time95 = 0.0;
			float2 voronoiSmoothId95 = 0;
			float2 coords95 = i.uv_texcoord * 40.0;
			float2 id95 = 0;
			float2 uv95 = 0;
			float voroi95 = voronoi95( coords95, time95, id95, uv95, 0, voronoiSmoothId95 );
			float lerpResult96 = lerp( 0.8 , 1.0 , voroi95);
			float temp_output_91_0 = ( lerpResult96 * 0.001 );
			float time115 = 17.3;
			float2 voronoiSmoothId115 = 0;
			float2 coords115 = i.uv_texcoord * ( ( _SinTime.y * 2.0 ) + 66.0 );
			float2 id115 = 0;
			float2 uv115 = 0;
			float voroi115 = voronoi115( coords115, time115, id115, uv115, 0, voronoiSmoothId115 );
			float clampResult166 = clamp( ceil( ( 0.0 - ase_vertex3Pos.z ) ) , -1.0 , 1.0 );
			float temp_output_149_0 = ( clampResult169 * 1.0 * voroi115 * clampResult166 );
			float2 appendResult88 = (float2(( ( _SinTime.w * 0.08 ) + 50.0 + temp_output_91_0 + temp_output_149_0 ) , ( ( _SinTime.z * 0.05 ) + 30.0 + temp_output_91_0 + temp_output_149_0 )));
			float2 break22_g4 = ( i.uv_texcoord * appendResult88 );
			float temp_output_9_0_g4 = ( ( break22_g4.y / 0.1 ) - (sin( ( ( break22_g4.x / 1.0 ) * 6.28318548202515 ) )*0.5 + 0.5) );
			float temp_output_5_0_g4 = ( abs( ( temp_output_9_0_g4 - round( temp_output_9_0_g4 ) ) ) * 2.0 );
			float smoothstepResult1_g4 = smoothstep( 0.5 , 0.55 , temp_output_5_0_g4);
			float2 appendResult103 = (float2(( ( _SinTime.w * 0.04 ) + 49.9 + temp_output_149_0 ) , ( ( _SinTime.z * 0.2 ) + 29.9 + temp_output_149_0 )));
			float2 break22_g7 = ( i.uv_texcoord * appendResult103 );
			float temp_output_9_0_g7 = ( ( break22_g7.y / 0.1 ) - (sin( ( ( break22_g7.x / 1.0 ) * 6.28318548202515 ) )*0.5 + 0.5) );
			float temp_output_5_0_g7 = ( abs( ( temp_output_9_0_g7 - round( temp_output_9_0_g7 ) ) ) * 2.0 );
			float smoothstepResult1_g7 = smoothstep( 0.5 , 0.55 , temp_output_5_0_g7);
			float temp_output_100_0 = ( smoothstepResult1_g4 * smoothstepResult1_g7 * temp_output_56_0 );
			float4 lerpResult97 = lerp( float4( 0,0,0,0 ) , color98 , temp_output_100_0);
			float4 clampResult71 = clamp( ( lerpResult48 + ( ( 1.0 - step( clampResult63 , 0.05 ) ) * 1.0 ) + lerpResult97 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float2 uv_TexCoord200 = i.uv_texcoord * float2( 11,11 );
			float simplePerlin2D198 = snoise( uv_TexCoord200*22.0 );
			simplePerlin2D198 = simplePerlin2D198*0.5 + 0.5;
			float saferPower202 = abs( ( 1.0 - simplePerlin2D198 ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult4_g14 = normalize( ( i.viewDir + ase_worldlightDir ) );
			float4 appendResult212 = (float4(0.0 , 1.0 , ( ( 1.0 - temp_output_100_0 ) + -0.5 ) , 0.0));
			float4 normalizeResult218 = normalize( appendResult212 );
			float fresnelNdotV214 = dot( normalizeResult218.xyz, normalizeResult4_g14 );
			float fresnelNode214 = ( 0.0 + 1.6 * pow( 1.0 - fresnelNdotV214, 0.29 ) );
			float saferPower197 = abs( ( pow( saferPower202 , 4.0 ) * 2.5 * ( 1.0 - fresnelNode214 ) ) );
			o.Albedo = ( ( lerpResult176 * clampResult71 ) + pow( saferPower197 , 10.0 ) ).rgb;
			float4 color80 = IsGammaSpace() ? float4(0.3738973,0.5049655,0.5974842,1) : float4(0.1153034,0.2186659,0.3156182,1);
			o.Specular = color80.rgb;
			o.Smoothness = 0.0;
			float lerpResult119 = lerp( 1.0 , 0.9 , temp_output_56_0);
			o.Alpha = lerpResult119;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19501
Node;AmplifyShaderEditor.PosVertexDataNode;137;-1584,1152;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;167;-1600,896;Inherit;False;Property;_PlayerPos;PlayerPos;1;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;141;-1248,1008;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;142;-1088,1088;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-928,1120;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;20.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;165;-800,1216;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;85;-1520,-144;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;143;-640,1120;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;52;-2848,-1120;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;53;-2784,-960;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-1104,528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;151;-2336,464;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;144;-480,1136;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;51;-2544,-1040;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-903.8131,608.3149;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;66;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;152;-2160,464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-192,1008;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;95;-1696,0;Inherit;True;0;0;1;3;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;40;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2272,-976;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;115;-720,448;Inherit;True;0;3;1;3;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;17.3;False;2;FLOAT;64.77;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.ClampOpNode;166;-2006.826,431.3763;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;169;-2224,-32;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;96;-1488,48;Inherit;False;3;0;FLOAT;0.8;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;55;-1952,-848;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-1840,368;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1312,48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.001;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1312,-80;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1424,352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1296,-192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.08;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-1392,192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.04;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;66;-2512,-448;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;3;-2448,-1232;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;73;-1728,-880;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-1120,-144;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;50;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-1104,0;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;30;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-1216,336;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;29.9;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-1216,192;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;49.9;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;67;-2304,-400;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;1;-2208,-1200;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;74;-1520,-832;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;-992,-64;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;103;-1056,256;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-2176,-352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;59;-2128,-640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;125;-3472,-1808;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;56;-1536,-960;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;84;-800,-32;Inherit;True;Smooth Wave;-1;;4;45d5b33902fbc0848a1166b32106db74;1,3,1;3;17;FLOAT2;55,33;False;16;FLOAT;1;False;18;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;99;-800,208;Inherit;True;Smooth Wave;-1;;7;45d5b33902fbc0848a1166b32106db74;1,3,1;3;17;FLOAT2;55,33;False;16;FLOAT;1;False;18;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-2144,-480;Inherit;False;2;2;0;FLOAT;600;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;128;-3456,-1616;Inherit;False;Constant;_Vector1;Vector 1;2;0;Create;True;0;0;0;False;0;False;0.2,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PowerNode;58;-1936,-672;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-3168,-1552;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-464.9054,87.14676;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-3232,-1728;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;61;-1968,-512;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;380.1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleAddOpNode;162;-1744,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;124;-3200,-1952;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;0;False;0;False;44,44;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;132;-2800,-1520;Inherit;False;Constant;_Vector2;Vector 2;2;0;Create;True;0;0;0;False;0;False;0.3,0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.VoronoiNode;129;-2992,-1632;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;33;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.OneMinusNode;219;-917.563,-551.2825;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-1824,-1328;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-3040,-1792;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;-2608,-1632;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1616,-592;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-704,-544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;121;-1696,-1456;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;-2272,-1744;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;63;-1440,-656;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;200;-1040,-1008;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;11,11;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;212;-1152,-784;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;50;-1984,-1536;Inherit;False;Constant;_Color1;Color 0;5;0;Create;True;0;0;0;False;0;False;0,0.5395944,0.7987421,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ClampOpNode;134;-1499.182,-1350.071;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;120;-2000,-1776;Inherit;True;Property;_WaveTex;WaveTex;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;4e43987007949c64d8f9b921c028c734;4e43987007949c64d8f9b921c028c734;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;62;-1520,-464;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;198;-816,-992;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;22;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;218;-1008,-736;Inherit;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;211;-1200,-864;Inherit;False;Blinn-Phong Half Vector;-1;;14;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;65;-1264,-448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-1376,-1040;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;98;-832,-336;Inherit;False;Constant;_Color3;Color 3;1;0;Create;True;0;0;0;False;0;False;0.09016647,0.5706224,0.6100628,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;122;-1411.889,-1508.221;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;49;-1680,-1216;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;0,1,0.8499384,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;201;-560,-944;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;214;-816,-816;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.6;False;3;FLOAT;0.29;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-1216,-1152;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;97;-384,-192;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;175;-224,736;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;177;-896,736;Inherit;False;Constant;_Color4;Color 4;2;0;Create;True;0;0;0;False;0;False;0.726415,0.824124,1,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;178;-896,944;Inherit;False;Constant;_Color5;Color 4;2;0;Create;True;0;0;0;False;0;False;0.4559748,0.6568932,1,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1072,-448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;202;-416,-1040;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;215;-512,-832;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-512,624;Inherit;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-288,-448;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;179;-576,848;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;174;16,-32;Inherit;False;2;0;FLOAT;0.7;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-320,544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;71;-96,-368;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;176;176,-224;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-288,-848;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;2.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-160,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;320,-304;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;197;-32,-704;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;111;0,448;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;80;-272,-80;Inherit;False;Constant;_Color2;Color 2;1;0;Create;True;0;0;0;False;0;False;0.3738973,0.5049655,0.5974842,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;81;208,80;Inherit;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;119;-144,192;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.9;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;636.9742,-264.323;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;976,48;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;141;0;167;1
WireConnection;141;1;137;2
WireConnection;141;2;167;2
WireConnection;142;0;141;0
WireConnection;142;1;137;0
WireConnection;146;0;142;0
WireConnection;165;0;146;0
WireConnection;143;0;165;0
WireConnection;116;0;85;2
WireConnection;151;1;137;3
WireConnection;144;0;143;0
WireConnection;51;0;52;0
WireConnection;51;1;53;0
WireConnection;117;0;116;0
WireConnection;152;0;151;0
WireConnection;168;0;144;0
WireConnection;168;1;167;3
WireConnection;54;0;51;0
WireConnection;115;2;117;0
WireConnection;166;0;152;0
WireConnection;169;0;168;0
WireConnection;96;2;95;0
WireConnection;55;0;54;0
WireConnection;149;0;169;0
WireConnection;149;2;115;0
WireConnection;149;3;166;0
WireConnection;91;0;96;0
WireConnection;90;0;85;3
WireConnection;170;0;85;3
WireConnection;86;0;85;4
WireConnection;171;0;85;4
WireConnection;73;0;55;0
WireConnection;87;0;86;0
WireConnection;87;2;91;0
WireConnection;87;3;149;0
WireConnection;89;0;90;0
WireConnection;89;2;91;0
WireConnection;89;3;149;0
WireConnection;102;0;170;0
WireConnection;102;2;149;0
WireConnection;101;0;171;0
WireConnection;101;2;149;0
WireConnection;67;0;66;0
WireConnection;1;1;3;0
WireConnection;74;0;73;0
WireConnection;88;0;87;0
WireConnection;88;1;89;0
WireConnection;103;0;101;0
WireConnection;103;1;102;0
WireConnection;68;0;67;0
WireConnection;59;0;1;0
WireConnection;56;0;74;0
WireConnection;84;17;88;0
WireConnection;99;17;103;0
WireConnection;69;1;68;0
WireConnection;58;0;59;0
WireConnection;130;0;125;1
WireConnection;100;0;84;0
WireConnection;100;1;99;0
WireConnection;100;2;56;0
WireConnection;127;0;125;1
WireConnection;127;1;128;0
WireConnection;61;2;69;0
WireConnection;162;0;58;0
WireConnection;162;1;169;0
WireConnection;129;1;130;0
WireConnection;219;0;100;0
WireConnection;133;0;56;0
WireConnection;126;0;124;0
WireConnection;126;1;127;0
WireConnection;131;0;129;0
WireConnection;131;1;132;0
WireConnection;60;0;162;0
WireConnection;60;2;61;0
WireConnection;213;0;219;0
WireConnection;121;0;133;0
WireConnection;123;0;126;0
WireConnection;123;1;131;0
WireConnection;63;0;60;0
WireConnection;212;2;213;0
WireConnection;134;0;121;0
WireConnection;120;1;123;0
WireConnection;62;0;63;0
WireConnection;198;0;200;0
WireConnection;218;0;212;0
WireConnection;65;0;62;0
WireConnection;57;0;1;0
WireConnection;57;1;56;0
WireConnection;122;0;120;0
WireConnection;122;1;50;0
WireConnection;122;2;134;0
WireConnection;201;0;198;0
WireConnection;214;0;218;0
WireConnection;214;4;211;0
WireConnection;48;0;122;0
WireConnection;48;1;49;0
WireConnection;48;2;57;0
WireConnection;97;1;98;0
WireConnection;97;2;100;0
WireConnection;175;0;61;0
WireConnection;175;2;143;0
WireConnection;72;0;65;0
WireConnection;202;0;201;0
WireConnection;215;0;214;0
WireConnection;70;0;48;0
WireConnection;70;1;72;0
WireConnection;70;2;97;0
WireConnection;179;0;177;0
WireConnection;179;1;178;0
WireConnection;179;2;167;3
WireConnection;174;1;175;0
WireConnection;107;0;115;0
WireConnection;107;1;108;0
WireConnection;71;0;70;0
WireConnection;176;0;179;0
WireConnection;176;2;174;0
WireConnection;191;0;202;0
WireConnection;191;2;215;0
WireConnection;109;0;107;0
WireConnection;173;0;176;0
WireConnection;173;1;71;0
WireConnection;197;0;191;0
WireConnection;111;1;109;0
WireConnection;119;2;56;0
WireConnection;185;0;173;0
WireConnection;185;1;197;0
WireConnection;0;0;185;0
WireConnection;0;3;80;0
WireConnection;0;4;81;0
WireConnection;0;9;119;0
WireConnection;0;11;111;0
ASEEND*/
//CHKSM=292EA6B2B2CB711193BE66131B168AD739EB4F97