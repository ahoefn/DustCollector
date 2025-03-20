#version 450 core

in vec3 vertexColor;
in float distmult;
out vec4 FragColor;

float alphaAntiaialisedMultiSample()
{
    // Multisamples 9 times
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
    // Output particle color with alpha value anti-aliased using multisampling
    FragColor=vec4(vertexColor,alphaAntiaialisedMultiSample()*distmult);
}