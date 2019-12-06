float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 Color;

struct VertexShaderInput {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 WorldPosition : POSITIONT;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input) {
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	float3 normal = normalize(mul(input.Normal, (float3x3)WorldInverseTranspose));
	output.Normal = normal;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0{
	return Color;
}

technique Simple
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}