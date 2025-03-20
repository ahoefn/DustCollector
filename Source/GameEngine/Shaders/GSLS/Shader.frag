#version 450 core

in vec3 vertexColor;
in float distmult;
out vec4 FragColor;

float alphaAntiaialisedMultiSample()
{
    //Antialiasing using multisampling
    float alphaScaling=distance(gl_PointCoord,vec2(0.25f,0.25f));
    alphaScaling+=distance(gl_PointCoord,vec2(0.25f,0.75f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.75f,0.25f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.75f,0.25f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.5f,0.75f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.5f,0.25f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.75f,0.5f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.25f,0.5f)); 
    alphaScaling+=distance(gl_PointCoord,vec2(0.5f,0.5f)); 

    return 1.0f-0.1111f*0.5f*alphaScaling*alphaScaling;
}

void main()
{
    FragColor=vec4(vertexColor,alphaAntiaialisedMultiSample()*distmult);
    // FragColor=vec4(vertexColor,1.0f);
}