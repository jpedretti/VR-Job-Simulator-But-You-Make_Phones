using UnityEngine;

namespace com.NW84P
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public static class RotationAxisExtensions
    {
        public static Vector3 ToVector3Axis(this RotationAxis rotationAxis) => rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up,
        };

        public static float GetAxisValue(this RotationAxis rotationAxis, Vector3 vector3) => rotationAxis switch
        {
            RotationAxis.X => vector3.x,
            RotationAxis.Y => vector3.y,
            RotationAxis.Z => vector3.z,
            _ => vector3.y,
        };
    }
}
