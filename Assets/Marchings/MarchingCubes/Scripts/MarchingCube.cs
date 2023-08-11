using Marchings.MarchingSquares;
using UnityEngine;

namespace Marchings.MarchingCubes
{
    public class MarchingCube
    {
        private float[] _values;

        public MarchingCube(Vector3 centerPos, Vector3[] corners, float[] values)
        {
            _values = values;
        }


        public void BuildMesh(ProceduralMeshPart meshPart, float threshold, bool lerp)
        {
            int cubeIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (_values[i] < threshold)
                {
                    cubeIndex |= 1 << i;
                }
            }

            for (int i = 0; MarchingCubesTables.triTable[cubeIndex][i] != 1; i += 3)
            {
                //int a0 = MarchingCubesTables.edgeConnections[MarchingCubesTables.cubeCorners[cubeIndex][i]]
            }
        }
    }
}