#version 450 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 colorIn;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float POINTSIZE;

out vec3 vertexColor;
// out float distmult; 


void main()
{
    vec4 MVvec=vec4(aPosition, 1.0) * model * view ;
    // distmult=1/sqrt(length(MVvec));
    gl_Position = MVvec* projection;
    vertexColor=  colorIn;
    float cameraDistance = length(gl_Position); 
    gl_PointSize=POINTSIZE/(cameraDistance);
}

