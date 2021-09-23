Shader "CrossSection/FemurResectionFivePlane" {
	Properties{
		_Color("Color", Color) = (0.9803922,0.9176471,0.8313726,1)
		_CrossColor("Cross Section Color", Color) = (0.8,0.4,0.467,1)
		
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		
		_PlanePosition1("Cut Plane 1 Position",Vector) = (0.5,0,0,0)
		_PlaneNormal1("Cut Plane 1 Normal",Vector) = (-1,0,0,0)

		_PlanePosition2("Cut Plane 2 Position",Vector) = (0.5,0.5,0,0)
		_PlaneNormal2("Cut Plane 2 Normal",Vector) = (-1,1,0,0)

		_PlanePosition3("Cut Plane 3 Position",Vector) = (0,0.5,0,0)
		_PlaneNormal3("Cut Plane 3 Normal",Vector) = (0,1,0,0)

		_PlanePosition4("Cut Plane 4 Position",Vector) = (0.5,0.5,0,0)
		_PlaneNormal4("Cut Plane 4 Normal",Vector) = (-1,1,0,0)

		_PlanePosition5("Cut Plane 5 Position",Vector) = (0.5,0,0,0)
		_PlaneNormal5("Cut Plane 5 Normal",Vector) = (-1,0,0,0)
        
	}

	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry" "LightMode" = "ForwardBase" }
		Pass {
			Cull Off

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 vertex: SV_POSITION;
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
				fixed4 diff : COLOR0;
            };

			fixed4 _Color;
			fixed4 _CrossColor;

			fixed3 _PlanePosition1;
			fixed3 _PlanePosition2;
			fixed3 _PlanePosition3;
			fixed3 _PlanePosition4;
			fixed3 _PlanePosition5;

			fixed3 _PlaneNormal1;
			fixed3 _PlaneNormal2;
			fixed3 _PlaneNormal3;
			fixed3 _PlaneNormal4;
			fixed3 _PlaneNormal5;

			bool checkVisibility(fixed3 worldPos)
			{
				float dotProd = dot(worldPos - _PlanePosition1, _PlaneNormal1);
				if (dotProd >= 0) return true;
				dotProd = dot(worldPos - _PlanePosition2, _PlaneNormal2);
				if (dotProd >= 0) return true;
				dotProd = dot(worldPos - _PlanePosition3, _PlaneNormal3);
				if (dotProd >= 0) return true;
				dotProd = dot(worldPos - _PlanePosition4, _PlaneNormal4);
				if (dotProd >= 0) return true;
				dotProd = dot(worldPos - _PlanePosition5, _PlaneNormal5);
				if (dotProd >= 0) return true;
				return false;
			}

            //Intersection of two planes
            bool PlanePlaneIntersection(fixed3 Plane1Normal,fixed3 Plane1Position,fixed3 Plane2Normal,fixed3 Plane2Position,out fixed3 linePoint,out fixed3 lineVector)
            {

                linePoint=0;
                lineVector=0;

                //CrossProduct         
                lineVector=cross(Plane1Normal,Plane2Normal);

                //Line direction
                fixed3 lineDir=cross(Plane2Normal,lineVector);

                float denominator=dot(Plane1Normal,lineDir);
                if(abs(denominator)>0.006)
                {
                    fixed3 plane1ToPlan2=Plane1Position-Plane2Position;
                    float t=dot(Plane1Normal,plane1ToPlan2)/denominator;
                    linePoint=Plane2Position+(t*lineDir);
                    return true;
                }
                else{
                    return false;
                }
                
            }

            //This function returns a point which is a projection from a point to a line.
	        //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.

            fixed3 ProjectPointOnLine(fixed3 linePoint, fixed3 lineVec, fixed3 posPoint)
            {		
                //get vector from point on line to point in space
                fixed3 linePointToPoint = posPoint - linePoint;

                float t = dot(linePointToPoint, lineVec);
                return linePoint + lineVec * t;

            }

			//Get the shortest distance between a point and a plane. The output is signed so it holds information
			//as to which side of the plane normal the point is.
			float signedDistancePlanePoint(fixed3 planeNormal, fixed3 planePoint, fixed3 pt) {
				return dot(planeNormal, (pt - planePoint));
			}

			//create a vector of direction "vector" with length "size"
			fixed3 setVectorLength(fixed3 vec, float size) {
				//normalize the vector
				fixed3 vectorNormalized = normalize(vec);
				//scale the vector
				return vectorNormalized *= size;
			}

			fixed3 projectPointOnPlane(fixed3 planeNormal, fixed3 planePoint, fixed3 pt) {
				float distance;
				fixed3 translationVector;
				//First calculate the distance from the point to the plane:
				distance = signedDistancePlanePoint(planeNormal, planePoint, pt);
				if(distance == 0) distance = 0.001;
				//Reverse the sign of the distance
				distance *= -1;
				//Get a translation vector
				translationVector = setVectorLength(planeNormal, distance);
				//Translate the point to form a projection
				return pt + translationVector;
			}

			fixed3 projectPointToPlane(fixed3 planePos, fixed3 planeNorm, fixed3 worldPos, out bool updated) {
				if (dot(worldPos - planePos, planeNorm) > 0) {
					updated = true;
					return projectPointOnPlane(planeNorm, planePos, worldPos);
				}
				updated = false;
				return worldPos;
			}

			v2f vert (appdata_full v)
			{
				fixed3 xyz = v.vertex;
				bool updated = false;
				fixed3 pos = projectPointToPlane(_PlanePosition1, _PlaneNormal1, xyz, updated);
				if (abs(distance(pos, xyz)) > 0.001 && updated) { v.normal = _PlaneNormal1; }
				pos = projectPointToPlane(_PlanePosition2, _PlaneNormal2, pos, updated);
				if (abs(distance(pos, xyz)) > 0.001 && updated) { v.normal = _PlaneNormal2; }
				pos = projectPointToPlane(_PlanePosition3, _PlaneNormal3, pos, updated);
				if (abs(distance(pos, xyz)) > 0.001 && updated) { v.normal = _PlaneNormal3; }
				pos = projectPointToPlane(_PlanePosition4, _PlaneNormal4, pos, updated);
				if (abs(distance(pos, xyz)) > 0.001 && updated) { v.normal = _PlaneNormal4; }
				pos = projectPointToPlane(_PlanePosition5, _PlaneNormal5, pos, updated);
				if (abs(distance(pos, xyz)) > 0.001 && updated) { v.normal = _PlaneNormal5; }
				v2f o;
                
				if (checkVisibility(xyz)) 
                {
					v.vertex.xyz = pos;
                    fixed3 lineVector1;
                    fixed3 linePoint1;
                    if(PlanePlaneIntersection(_PlaneNormal1,_PlanePosition1,_PlaneNormal2,_PlanePosition2,linePoint1,lineVector1))
                    {
                        fixed3 posVertex=ProjectPointOnLine(linePoint1,lineVector1,v.vertex);
                        if(abs(distance(posVertex,v.vertex)) < 0.2)
                        {
                            v.vertex.xyz=posVertex;
                        }     
                              
                    }
                    o.uv = (1, 1);

				} 
                else 
                {
					o.uv = (0, 0);
				}

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				// the only difference from previous shader:
				// in addition to the diffuse lighting from the main light,
				// add illumination from ambient or light probes
				// ShadeSH9 function from UnityCG.cginc evaluates it,
				// using world space normal
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
				return o;
			}

            
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _Color;
				if (i.uv.x == 1) {
					col = _CrossColor;
				}
				col *= i.diff;
				return col;
			}
			ENDCG
		}
	}
	//FallBack "Diffuse"
}