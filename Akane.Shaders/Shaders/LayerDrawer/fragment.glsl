

// Interpolated values from the vertex shaders
in vec2 UV;

// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D ColorMap;
uniform sampler2D TileIDs;

uniform float maxGid;
uniform vec2 textureSize;
uniform vec2 mapSize;
uniform vec2 tileSize;
uniform vec2 viewportTileRes;
uniform vec2 viewportOffset;

void main(){
	
	vec2 tileCounts = vec2(textureSize/tileSize);
	
	vec2 uvStep = vec2(1)/viewportTileRes;	//The value at which a new tile should occur
	vec2 curPos = mod(UV, uvStep);
	vec2 tileBaseCoords = clamp(UV - curPos, vec2(0.0), vec2(1));	//the UV coordinates of the base of the current tile

	vec2 nextTilePos = tileBaseCoords + uvStep;

	vec2 mapInfo = vec2(1)/viewportTileRes;
	vec2 curPixelPos = mod(UV, mapInfo);
	vec2 viewportOff = viewportOffset/(mapSize * tileSize);
	curPixelPos -= viewportOff;

	//The id of the tile
	float tileId = round(float(texelFetch(TileIDs, ivec2( ceil((UV - curPixelPos) * viewportTileRes - vec2(0.3, 0.3) )), 0).r * maxGid)) - 1;
	
	//The appropriate tile position
	vec2 offset = vec2(0);
	offset.x = mod(tileId, tileCounts.x);
	offset.y = (tileId - offset.x)/tileCounts.x + 1; 
	offset.y = tileCounts.y - offset.y;

	//offset = round(offset);
	offset = (offset * tileSize)/textureSize;

	vec2 UVCoords =  offset + ((curPos/uvStep) * tileSize)/textureSize;

	// Output color = color of the texture at the specified UV
	color = texture2D(ColorMap, UVCoords );
	//color.a = 0.5;
}