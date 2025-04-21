#version 150

in vec2 sTexcoord;
flat in int sIndex;

out vec4 fragColor;

uniform float collected[58];
uniform sampler2D uTexture[2];
uniform bool grid;

void main()
{
	if (sIndex == 100)
	{
		fragColor = texture(uTexture[1], sTexcoord);
		return;
	}

    if (collected[sIndex] == 0.0f)
    {
		if (!grid)
		{
			fragColor = vec4(0.0f);
			return;
		}
        vec4 textureColor = texture(uTexture[0], sTexcoord);
        float greyscale = textureColor.r * 0.1 + textureColor.g * 0.3 + textureColor.b * 0.1;
        fragColor = vec4(greyscale, greyscale, greyscale, textureColor.a);
        return;
    }
    else if ( collected[sIndex] < 1.73322f) {
        float val = abs(sin(collected[sIndex] * 7))+0.6;
        vec4 textureColor = texture(uTexture[0], sTexcoord);
        vec4 adjusted = vec4(textureColor.r * val, textureColor.g * val, textureColor.b * val, textureColor.a);
        fragColor = adjusted;
        return;
    }
    
    fragColor = texture(uTexture[0], sTexcoord);
}