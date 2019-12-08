float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose; 
float3 Camera;
float3 LightPosition;
float4 LightColor;
texture ModelTexture;

float LightRadius = 100;
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.5;
float4 DiffuseColor = float4(1, .5, .5, 1);
float DiffuseIntensity = 0.7;
float SpecularIntensity = 0.5;
float Shininess = 20;

sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

static const float PI = 3.13159265f;


struct VertexShaderInput {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput {
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 WorldPosition : POSITIONT;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input) {
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	float3 normal = normalize(mul(input.Normal, (float3x3)WorldInverseTranspose));
	output.Normal = normal;

	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0 {
	float3 lightDirection = LightPosition -(float3)input.WorldPosition;
	float3 normal = normalize(input.Normal);
	float intensity = pow(1 - saturate(length(lightDirection) / LightRadius), 2);
	lightDirection = normalize(lightDirection);
	float3 view = normalize(Camera - (float3)input.WorldPosition);
	float diffuseColor = dot(normal, lightDirection) * intensity;
	float3 reflect = normalize(2 * diffuseColor * normal - lightDirection);
	diffuseColor = saturate(diffuseColor);
	float dotProduct = dot(reflect, view);
	float4 specular = (8 + Shininess) / (8 * PI) * SpecularIntensity *
		pow(saturate(dotProduct), Shininess) * intensity;

	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;
	
	
	return saturate((diffuseColor * textureColor) + (textureColor * AmbientIntensity) + (specular * LightColor));
}

technique PointLight
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}