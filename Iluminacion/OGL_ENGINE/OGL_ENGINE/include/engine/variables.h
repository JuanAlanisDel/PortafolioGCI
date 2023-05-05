
#include <GLFW/glfw3.h>
#include <engine/Billboard.h>
#include <engine/CollisionBox.h>
#include <engine/Objectives.h>
#include <engine/Particles.h>
#include <engine/Plane.h>
#include <engine/QuadTexture.h>
#include <engine/RigidModel.h>
#include <engine/Terrain.h>
#include <engine/shader_m.h>
#include <engine/skybox.h>
#include <engine/textrenderer.h>
#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <iostream>
#include <string>
#include <vector>

enum LightType
{
    FlatColor,
    AllLights,
    DirectionalLight,
    SpotLight,
    PointLight
};

enum Axis
{
    X,
    Y,
    Z
};
enum TypeActionKeyBoard
{
    GAME,
    OBJECTS,
    COLLISION,
    LIGHTING,
};

enum TransformObject
{
    MODEL,
    COLLBOX,
};

//:::: ENUMS :::://
LightType lightType = FlatColor;
TypeActionKeyBoard typemenu = GAME;
TransformObject transformObject = COLLBOX;

//:::: VARIABLES GLOBALES :::://

//:::: BANDERAS :::://
// 
//VENTANA
const unsigned int SCR_WIDTH = 800;
const unsigned int SCR_HEIGHT = 600;

//CONTROL
double xJoy = 0.0;
double yJoy = 0.0;
double yLeftJoy = 0.0;
bool isJoyStick = false;
float lastX = SCR_WIDTH / 2.0f;
float lastY = SCR_HEIGHT / 2.0f;
bool firstMouse = true;

//TIMING
float deltaTime = 0.0f;
float lastFrame = 0.0f;
//TEXTURAS DEL TERRENO
const char **texturePaths;

//PARTICULAS
float respawnCount = 0.0f;

//:::: OBJETOS :::://
Camera camera(glm::vec3(2.0f, 1.5f, 4.0f));


//:::: VECTORES :::://
glm::vec3 force(0);
glm::vec3 posModel(0);
glm::vec3 rotationModel(0);
glm::vec3 scaleCollBox(0);
glm::vec3 skyPos(0);
glm::vec3 lightPos(1.2f, 1.0f, 2.0f);
glm::vec3 lightDir(0.0f, 1.0f, 0.0f);
vec3 originPoint(0.0f, 1.0f, 0.0f);
vec3 rotationPoint(0.0, 1.0, 0.0);
vec3 mainLight(0.5);

//::::::::::::::MOVER OBJETOS:::::::::::::://
float initScale = 0.2f;
int indexObject = 0;
int indexCollBox = 0;
int indexLight = 0;

bool isCollBoxModel = false;
bool renderCollBox = false;
bool renderLightingCubes = false;
float angleObjects = 0.0f;
float movement = 0.01;

//::::::::::::::ARREGLOS DE OBJETOS:::::::::::::://
vector<glm::vec3> pointLightPositions;
vector<glm::vec3> physicsObjectsPositions;
vector<glm::vec3> ejercicioIlumPositions;
vector<RigidModel> rigidModels;
vector<RigidModel> rbmodels = rigidModels;

vector<Model> models;
vector<Model> modelsIlumination;
map<int, pair<string, CollisionBox>> collboxes;
map<int, pair<string, CollisionBox>> lightcubes;
CollisionBox* cb = new CollisionBox();
//::::::::::::::FISICAS:::::::::::::://
rbEnvironment physicsEnviroment;
rbRigidBody piso, pared;
int indexRigidModel = 0;
float colDetection = 0;
bool ejercicioIluminacion = false;

