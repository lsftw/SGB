// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "BlockOutlineShader" {
	Properties {
		_LineColor ("Line Color", Color) = (1,1,1,1)
		_GridColor ("Grid Color", Color) = (94, 70, 35, 255)
		_LineWidth ("Line Width", float) = 0.05
		_SideLength ("Side Length", float) = 1.0
	}
	SubShader {
        Pass {
 
CGPROGRAM
 
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
uniform float4 _LineColor;
uniform float4 _GridColor;
uniform float _LineWidth;
uniform float _SideLength;
 
// vertex input: position, uv1, uv2
struct appdata {
    float4 vertex : POSITION;
    float4 texcoord1 : TEXCOORD1;
    float4 color : COLOR;
};
 
struct v2f {
    float4 pos : POSITION;
    float4 texcoord1 : TEXCOORD1;
    float4 color : COLOR;
};
 
v2f vert (appdata v) {
    v2f o;
    o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
    o.texcoord1 = v.texcoord1;
    o.color = v.color;
    return o;
}
 
float4 frag(v2f i ) : COLOR
{
//	if (i.texcoord1.x < _LineWidth ||
//		i.texcoord1.y < _LineWidth)
//	{
//		return _LineColor;
//	}
 
//	if ((i.texcoord1.x - i.texcoord1.y) < _LineWidth &&
//		(i.texcoord1.y - i.texcoord1.x) < _LineWidth)
//	{
//		return _LineColor;
//	}
//	else
// i.texcoord1.x > _SideLength-_LineWidth  || i.texcoord1.y > _SideLength-_LineWidth
//	if (i.texcoord1.x < _LineWidth || i.texcoord1.y < _LineWidth ||
//		i.texcoord1.x > _SideLength-_LineWidth || 
//		i.texcoord1.y > _SideLength-_LineWidth) {
	if (i.texcoord1.x < _LineWidth+.7) {
		return _LineColor;
	} else	{
		return _GridColor;
	}
}
 
ENDCG
 
        }
	} 
	Fallback "Vertex Colored", 1
}