#version 450 core

in vec3 vertexColor;
out vec4 FragColor;

void main()
{
    float distFromCenter=distance(gl_PointCoord,vec2(0.5f,0.5f)); 
    FragColor=vec4(vertexColor,0.8f-1.6*distFromCenter);
}