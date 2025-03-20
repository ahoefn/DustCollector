#version 450 core

// Inputs:
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 colorIn;

// Uniforms:
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float POINTSIZE;

// Outputs:
out vec3 vertexColor; // Sets color of particle
out float distmult;   // Scales alpha value depending on distance


void main()
{
    // Apply matrices in order to get real position and screen position:
    vec4 MVvec=vec4(aPosition, 1.0) * model * view ;
    gl_Position = MVvec * projection;

    // Scale points based on distance:
    float cameraDistance = length(MVvec.xyz); 
    gl_PointSize = POINTSIZE / cameraDistance;

    // Output color and alpha scaling: 
    vertexColor = colorIn;
    distmult = 1 / sqrt(length(MVvec));
}

