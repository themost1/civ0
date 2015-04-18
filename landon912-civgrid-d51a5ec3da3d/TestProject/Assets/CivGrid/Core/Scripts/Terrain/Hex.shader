Shader "Hexagon"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_GridTex("Grid Texture", 2D) = "white" {}
        _BorderTex ("Border Texture", 2D) = "white" {}
        _UseGrid("Show Grid", Range (0, 1)) = 0
        _FOWTex("Fog of War Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"IgnoreProjector"="True" "RenderType"="Opaque"}
        LOD 200
 
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
 
        CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
		#pragma exclude_renderers gles
        #pragma surface surf Lambert vertex:vert
 
		 
		float4 BlendMethod( float4 a, float4 b ){
		
			return a * b; 					// Default
//		    return (a+b-1); 				// Severe.
//			return (1-(1-a)*b);				// Nice effect (keeps white border) but inverts & lightens colors.
			
		}
		
        sampler2D _MainTex;
		sampler2D _GridTex;
        sampler2D _BlendTex;
        float _UseGrid;
        sampler2D _FOWTex;
       	

        struct Input
        {
            float2 uv_MainTex;
    		float2 uv2_BlendTex;
			float4 tangent;
    		float4 color: Color; // Vertex color
    		fixed underFOW;
        };

		void vert(inout appdata_full v, out Input o)
		{
			o.tangent = v.tangent;
			
			
			//determine FOW setting for this vertex
			o.underFOW = (fixed)v.tangent.z;
		}
 
       void surf (Input IN, inout SurfaceOutput o)
	   {
	     fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
	 	 fixed4 texTwoCol = BlendMethod( tex2D(_BlendTex, IN.uv2_BlendTex), IN.color.rgba );

	     fixed4 mainOutput = mainCol.rgba * (1.0 - texTwoCol.a);
	     fixed4 blendOutput = texTwoCol.rgba * texTwoCol.a;
	     
	     if(_UseGrid == 1)
	     {
	     	fixed4 gridCol = tex2D(_GridTex, IN.tangent.xy);
		 	fixed4 gridOutput = gridCol.rgba * gridCol.a; 		 	
		 	
		 	if(IN.underFOW == 1)
		 	{
		 		fixed4 FOWCol = tex2D(_FOWTex, IN.tangent.xy);
		 		fixed4 FOWOutput = FOWCol.rgba;
		 	
		 		o.Albedo =  mainOutput.rgb + blendOutput.rgb + gridOutput.rgb;// + FOWOutput.rgb;
		 		o.Alpha = mainOutput.a + blendOutput.a + gridOutput.a;// + FOWOutput.a;
		 	}
		 	else
			{
				o.Albedo = ( mainOutput.rgb + blendOutput.rgb + gridOutput.rgb);
		 		o.Alpha = ( mainOutput.a + blendOutput.a + gridOutput.a);
		 	}
		 }
		 else
		 {
		 	if(IN.underFOW == 1)
		 	{
		 		fixed4 FOWCol = tex2D(_FOWTex, IN.tangent.xy);
		 		fixed4 FOWOutput = FOWCol.rgba;
		 
		 		o.Albedo =  mainOutput.rgb + blendOutput.rgb;// + FOWOutput.rgb;
		 		o.Alpha = mainOutput.a + blendOutput.a;// + FOWOutput.a;
		 	}
		 	else
		 	{
		 		o.Albedo = ( mainOutput.rgb + blendOutput.rgb);
		 		o.Alpha = mainOutput.a + blendOutput.a;
		 	}
		 }
	   }
        ENDCG
    } 
    FallBack "Diffuse"
}