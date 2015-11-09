// XNA 4.0 Shader Programming #2 - Diffuse light

// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

// Light related
float4 AmbientColor;
float AmbientIntensity;

float3 LightDirection;
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1;

float4 SpecularColor = float4(1, 1, 1, 0.05);
float3 EyePosition;

float fogNear = 10.0;
float fogFar = 20.0;
float4 fogColor = float4(0.3, 0.3, 0.3, 1);

//Attenuation Variables
float Kc = 1; //Constant Attenuation
float Kl = 0.2; //Linear Attenuation
float Kq = 0.1; //quadratic Attenuation

//Texture shading
texture ModelTexture;

sampler2D textureSampler = sampler_state 
{
	Texture = (ModelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

// The input for the VertexShader
struct VertexShaderInput
{
	float4 Position : POSITION0;
	//Texture shading
	float3 TextureCoordinate : TEXCOORD0;
};

// The output from the vertex shader, used for later processing
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 PositionOut : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 View : TEXCOORD2;
	//Texture shading
	float3 TextureCoordinate : TEXCOORD3;
};

// The VertexShader.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float3 Normal : NORMAL)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.PositionOut = worldPosition;

	float3 normal = normalize(mul(Normal, World));
	output.Normal = normal;
	output.View = normalize(float4(EyePosition, 1.0) - worldPosition);

	//Texture shading
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

// The Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 L = EyePosition - input.PositionOut;
	float distance = length(EyePosition - input.PositionOut);

	//spot cone
	float minCos = cos(0.785398f);
	float maxCos = (minCos + 1.0f) / 2.0f;
	float cosAngle = dot(LightDirection, -(L/distance));
	float spotIntensity = smoothstep(minCos, maxCos, cosAngle);

	//calculate Attenuation
	float4 Attenuation = 1 / (Kc + Kl * distance + Kq*distance*distance);

	float4 normal = float4(input.Normal, 1.0);
	float4 diffuse = saturate(dot(-LightDirection,normal)) * Attenuation * spotIntensity;
	float4 reflect = normalize(2 * diffuse*normal - float4(LightDirection, 1.0));
	float4 specular = pow(saturate(dot(reflect, input.View)), 2) * Attenuation *spotIntensity;
	
	// Calculate fog
	float fog = clamp((distance - fogNear) / (fogFar - fogNear), 0, 1);



	// Calculate final colouration
	float4 color = tex2D(textureSampler, input.TextureCoordinate);
	color.rgb *= AmbientColor * AmbientIntensity + DiffuseIntensity * DiffuseColor * diffuse + SpecularColor * specular;
	color.rgb = lerp(color.rgb, fogColor, fog);

	return color;
}

// Our Techinique
technique ShaderTech
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}