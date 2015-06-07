
Shader "cursorShador" 
{
	Properties 
	{
		_Color ("Main Color", COLOR) = (1,1,1,1)
	}

 
    SubShader {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Opaque" }
		ZTest Always Lighting Off Cull Off Fog { Mode Off } 
		LOD 110
        
        Pass 
        {
            SetTexture[_] {
                constantColor [_Color]
                Combine constant
            }
        }
    }
    
}







