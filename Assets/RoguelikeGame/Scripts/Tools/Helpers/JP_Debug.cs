using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public class JP_Debug : MonoBehaviour
    {
        /// <summary>
        /// Draws a cube at the specified position, offset, and of the specified size
        /// </summary>
        public static void DrawGizmoCube(Transform transform, Vector3 offset, Vector3 cubeSize, bool wireOnly)
        {
            Matrix4x4 rotationMatrix = transform.localToWorldMatrix;
            Gizmos.matrix = rotationMatrix;
            if (wireOnly)
            {
                Gizmos.DrawWireCube(offset, cubeSize);
            }
            else
            {
                Gizmos.DrawCube(offset, cubeSize);
            }
        }
    }
}
