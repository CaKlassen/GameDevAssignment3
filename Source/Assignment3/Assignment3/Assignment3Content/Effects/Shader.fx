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
float SpotlightConeAngle;
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

// PIXEL SHADER HELPERS

float CalculateSpotCone(float3 spotDir, float3 lightDir)//lightDir is the light from surface to source
{
	float minCos = cos(SpotlightConeAngle);//inner cone
	float maxCos = (minCos + 1.0f) / 2.0f;//outer cone
	float CosAngle = dot(spotDir.xyz, -lightDir);//Angle between the spotlight direction and -L (L is vector of light from surface to source)
	return smoothstep(minCos, maxCos, CosAngle);//interpolation of intensity between minCos and maxCos
}

float calculateSpecular(float3 pos, float3 texCoordinate, float3 normal, float3 L)
{
	// Calculate Specular
	float3 V = normalize(pos - texCoordinate);
	float3 R = normalize(reflect(-L, normal));
	float RdotV = max(0.0000000000000001f, dot(R, V));

	// Create Phong
	return pow(RdotV, Shininess);
}

float3 calculateDiffuse(float3 normal, float3 L)
{
	float diffuseLight = max(dot(normal, L), 0);
	return DiffuseIntensity * lightColor * diffuseLight;
}


// PIXEL SHADER

float4 PSspotlight(VertexShaderOutput input) : COLOR0
{
	// Calculate the transformed player position
	float3 pos = spotlightPosition;

	pos = mul(spotlightPosition, World);
	pos = mul(pos, View);

	// Calculate distance and length
	float3 L = normalize(pos - input.TextureCoordinate);
	float distance = length(L);
	L = L / distance;

	// Calculate attenuation based on distance
	float attenuation = 1.0f / (ConstAttenuation + LinearAttenuation * distance + QuadraticAttenuation * distance * distance);

	
	//////////////////////
	// GENERAL LIGHTING //
	//////////////////////

	// Calculate Ambient
	float3 ambient = AmbientColor * AmbientIntensity;

	// Calculate Diffuse
	float diffuse = calculateDiffuse(input.Normal, L);

	// Calculate Colour from the texture
	float4 color = tex2D(textureSampler, input.TextureCoordinate);

	// Calculate Specular
	float specularLight = calculateSpecular(pos, input.TextureCoordinate, input.Normal, L);

	// Calculate the specular effect
	/*if (diffuseLight <= 0)
	specularLight = 0;*/
	float3 specular = lightColor * specularLight;


	///////////////
	// SPOTLIGHT //
	///////////////

	// Spotlight intensity
	float spotlightIntensity = CalculateSpotCone(spotlightDirection, L);
	

	//////////////////
	// FINALIZATION //
	//////////////////

	// Create the final diffuse and specular effects
	diffuse = diffuse * attenuation * spotlightIntensity;
	specular = specular * attenuation * spotlightIntensity;

	// Calculate the final light level and colour
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
