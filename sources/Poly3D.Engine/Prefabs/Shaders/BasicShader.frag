#version 330

uniform mat4 model;

out vec4 fragColor;

in vec3 fragNormal;
in vec3 fragVert;

void main()
{
	mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 normal = normalize(normalMatrix * fragNormal);
	//vec3 fragPosition = vec3(model * vec4(fragVert, 1));

    fragColor = vec4(1.0, 0.0, 0.0, 1.0);
}