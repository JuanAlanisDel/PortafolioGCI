#version 330 core
out vec4 FragColor;


uniform samplerCube skybox;
float shininess;

struct DirLight {
    vec3 direction;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
  
    float constant;
    float linear;
    float quadratic;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;       
};


//NÚMERO DE LUCES POR TIPO DE LUZ
#define NR_POINT_LIGHTS 4
uniform int maxRenderLights;

in vec3 FragPos;
in vec3 Normal;
in vec3 TexCoords;
uniform vec3 viewPos;

//LUCES EJERCICIO
uniform PointLight plred;
uniform PointLight plgreen;
uniform PointLight plblue;
uniform SpotLight sl;
uniform DirLight dl;

//ARREGLOS DE LUCES
uniform DirLight dirLights[NR_POINT_LIGHTS];
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform SpotLight spotLights[NR_POINT_LIGHTS];


//CAMBIO DE LUCES
uniform int lightType;

uniform bool ejercicioIluminacion;

// function prototypes
vec4 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec4 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec4 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec4 CalcFlatColor(DirLight light);
vec4 CalcAllLights();
vec4 PortafolioIluminacion();


void main()
{    
vec4 result;
if(ejercicioIluminacion ==true){
    result = PortafolioIluminacion();
}
else{
    result = CalcAllLights();
}
    FragColor = result;
}

vec4 PortafolioIluminacion()
{
vec3 norm = normalize(Normal);
     vec3 viewDir = normalize(viewPos - FragPos);  
     vec4 result;
     if(lightType == 2)
            result += CalcDirLight(dl , norm, viewDir);
     
     else if(lightType == 3)
            result += CalcSpotLight(sl, norm, FragPos, viewDir);   
     
     else if(lightType == 4)
            {
            result += CalcPointLight(plred, norm, FragPos, viewDir);   
            result += CalcPointLight(plgreen, norm, FragPos, viewDir);   
            result += CalcPointLight(plblue, norm, FragPos, viewDir);   
            }
            else
     result = CalcFlatColor(dirLights[0]);
   return result;
}

vec4 CalcAllLights()
{
     vec3 norm = normalize(Normal);
     vec3 viewDir = normalize(viewPos - FragPos);  
     vec4 result;
     if(lightType == 1)
          for(int i = 0; i < maxRenderLights; i++)
          {
            result += CalcDirLight(dirLights[i] , norm, viewDir);
            result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);    
            result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);
          }     
     else if(lightType == 2)
        for(int i = 0; i < maxRenderLights; i++)
            result += CalcDirLight(dirLights[i] , norm, viewDir);
     
     else if(lightType == 3)
        for(int i = 0; i < maxRenderLights; i++)
            result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);   
     
     else if(lightType == 4)
        for(int i = 0; i < maxRenderLights; i++)
            result += CalcPointLight(pointLights[i], norm, FragPos, viewDir); 
          else
     result = CalcFlatColor(dirLights[0]);
     
   return result;
}
// calculates the color when using a directional light.
vec4 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // combine results
    vec4 ambient, diffuse, specular;
    
    ambient = vec4(light.ambient,1.0) * texture(skybox, TexCoords);
    diffuse = vec4(light.diffuse,1.0) * diff * texture(skybox, TexCoords);
    specular = vec4(light.specular,1.0) * spec * texture(skybox, TexCoords);  
   
    return (ambient + diffuse + specular);
}

// calculates the color when using a point light.
vec4 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // combine results
    vec4 ambient, diffuse, specular;
    ambient = vec4(light.ambient,1.0) * texture(skybox, TexCoords);
    diffuse = vec4(light.diffuse,1.0) * diff * texture(skybox, TexCoords);
    specular =vec4( light.specular,1.0) * spec * texture(skybox, TexCoords);

    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;


    return (ambient + diffuse + specular);
}

// calculates the color when using a spot light.
vec4 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
   vec3 lightDir = normalize(light.position - fragPos);
   // diffuse shading
   float diff = max(dot(normal, lightDir), 0.0);
   // specular shading
   vec3 reflectDir = reflect(-lightDir, normal);
   float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
   // attenuation
   float distance = length(light.position - fragPos);
   float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
   // spotlight intensity
   float theta = dot(lightDir, normalize(-light.direction)); 
   float epsilon = light.cutOff - light.outerCutOff;
   float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
   // combine results
    vec4 ambient, diffuse, specular;
    ambient = vec4(light.ambient,1.0) * texture(skybox, TexCoords);
    diffuse = vec4(light.diffuse,1.0) * diff * texture(skybox, TexCoords);
    specular = vec4(light.specular,1.0) * spec * texture(skybox, TexCoords);
  
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular) + texture(skybox, TexCoords) *0.1;
}

vec4 CalcFlatColor(DirLight light)
{
    vec4 ambient, diffuse, specular;
    ambient = vec4(light.ambient,1.0) * texture(skybox, TexCoords);
    diffuse = vec4(light.diffuse,1.0)  * texture(skybox, TexCoords);
    specular = vec4(light.specular,1.0)  * texture(skybox, TexCoords);

    return  texture(skybox, TexCoords);
}