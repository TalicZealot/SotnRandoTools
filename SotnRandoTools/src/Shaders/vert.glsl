//opengl 3.2
#version 150

in vec2 vPosition;
in vec2 vTexcoord;
in float vIndex;

out vec2 sTexcoord;
flat out int sIndex;

uniform vec2 viewportSize;

void main()
{
    float nx = vPosition.x / viewportSize.x + vPosition.x / viewportSize.x - 1.0f;
    float ny = vPosition.y / viewportSize.y + vPosition.y / viewportSize.y - 1.0f;

    sTexcoord = vTexcoord;
	sIndex = int(vIndex);
    gl_Position = vec4(nx, ny, 0.0f, 1.0);
}