// SET IN CODE

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor = float4(1, 1, 1, 1);
float4 AmbientIntensity = 0.1;


// NOT SET IN CODE

float3 DiffuseLightDirection = float3(1, -1, 1);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1;

float Shininess = 100;//lower for less reflective objects (100-150 for metal)
float4 SpecularColor = float4(1, 1, 1, 0.05);
float SpecularIntensity = 0;//lower intensity means dimmer highlights
float3 ViewVector = float3(1, 0, 0);//direction of camera eye

//spotlight
float spotlightPower = 10;
float3 spotlightPosition;//camera position
float3 spotlightDirection;//camera lookat-position (normalized)
float3 lightColor;
float SpotlightConeAngle = 23.5;
float ConstAttenuation = 1.0f;
float LinearAttenuation = 0.2f;
float QuadraticAttenuation = 0.1f;

//Texture shading
texture ModelTexture;
sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	//Texture shading
	float3 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	//Texture shading
	float3 TextureCoordinate : TEXCOORD1;
};

float CalculateSpotCone(float3 spotDir, float3 lightDir)//lightDir is the light from surface to source
{
	float minCos = cos(SpotlightConeAngle);//inner cone
	float maxCos = (minCos + 1.0f) / 2.0f;//outer cone
	float CosAngle = dot(spotDir, -lightDir);//Angle between the spotlight direction and -L (L is vector of light from surface to source)
	return smoothstep(minCos, maxCos, CosAngle);//interpolation of intensity between minCos and maxCos
}

// VERTEX SHADER

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);
	output.Normal = normal;


	//Texture shading
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}


// PIXEL SHADER

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	
	float3 light = normalize(DiffuseLightDirection);
	float3 normal = normalize(input.Normal);
	float3 r = normalize(2 * dot(light, normal) * normal - light);
	float3 v = normalize(mul(normalize(ViewVector), World));

	//ambient
	float4 ambient = AmbientColor * AmbientIntensity;//Ka = AmbientIntensity

	float dotProduct = dot(r, v);

	//specular
	float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);

	
	//Texture shading
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1;

	return saturate(textureColor * (input.Color) + ambient);
}

float4 PSspotlight(VertexShaderOutput input) : COLOR0
{

	float4 color = tex2D(textureSampler, input.TextureCoordinate);

	//ambient
	float3 ambient = AmbientColor * AmbientIntensity;//Ka = AmbientIntensity



	
	float3 L = normalize(spotlightPosition - input.TextureCoordinate);
	float distance = length(L);
	L = L / distance;

	//attenuation
	float attenuation = 1.0f / (ConstAttenuation + LinearAttenuation * distance + QuadraticAttenuation * distance * distance);

	//spotlight intensity
	float spotlightIntensity = CalculateSpotCone(spotlightDirection, L);

	//diffuse
	float diffuseLight = max(dot(input.Normal, L), 0);
	float3 diffuse = DiffuseIntensity * lightColor * diffuseLight;

	//specular; Phong
	float3 V = normalize(spotlightPosition - input.TextureCoordinate);
	float3 R = normalize(reflect(-L, input.Normal));
	float RdotV = max(0.0000000000000001f, dot(R, V));

	/* THIS WAS BLINN-PHONG
	float3 H = normalize(L + V);
	float specularLight = pow(dot(input.Normal, H), SpecularIntensity);*/
	float specularLight = pow(RdotV, Shininess);//THIS IS PHONG

	/*if (diffuseLight <= 0)
		specularLight = 0;*/
	float3 specular = lightColor * specularLight;//specularIntensity *


	//NOT USED ATM
	//float spotlightScale = pow(max(dot(L, -spotlightDirection), 0), spotlightPower);
	//float3 light = ambient + (diffuse + specular) * spotlightScale;

	//finalize Diffuse and Specular
	diffuse = diffuse * attenuation * spotlightIntensity;
	specular = specular * attenuation * spotlightIntensity;

	float3 light = ambient + diffuse + specular;

	color.rgb *= light;

	return color;
}

// TECHNIQUE

technique ShaderTech
{
	//pass Pass1
	//{
	//	VertexShader = compile vs_2_0 VertexShaderFunction();
	//	PixelShader = compile ps_2_0 PixelShaderFunction();
	//}

	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PSspotlight();
	}
}
