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
        public static Vector3 ToVector3Axis(this RotationAxis rotationAxis)
        {
            Vector3 result = rotationAxis switch
            {
                RotationAxis.X => Vector3.right,
                RotationAxis.Y => Vector3.up,
                RotationAxis.Z => Vector3.forward,
                _ => Vector3.up,
            };
            return result;
        }

        public static float GetAxisValue(this RotationAxis rotationAxis, Vector3 vector3)
        {
            float result = rotationAxis switch
            {
                RotationAxis.X => vector3.x,
                RotationAxis.Y => vector3.y,
                RotationAxis.Z => vector3.z,
                _ => vector3.y,
            };
            return result;
        }

        public static Quaternion EulerAxisValue(this RotationAxis rotationAxis, Vector3 fixedPart, Vector3 rotationPart)
        {
            Quaternion result = rotationAxis switch
            {
                RotationAxis.X => Quaternion.Euler(rotationPart.x, fixedPart.y, fixedPart.z),
                RotationAxis.Y => Quaternion.Euler(fixedPart.x, rotationPart.y, fixedPart.z),
                RotationAxis.Z => Quaternion.Euler(fixedPart.x, fixedPart.y, rotationPart.z),
                _ => Quaternion.Euler(fixedPart.x, rotationPart.y, fixedPart.z),
            };
            return result;
        }

        public static RotationAxis[] Others(this RotationAxis rotationAxis)
        {
            RotationAxis[] result = rotationAxis switch
            {
                RotationAxis.X => new RotationAxis[] { RotationAxis.Y, RotationAxis.Z },
                RotationAxis.Y => new RotationAxis[] { RotationAxis.X, RotationAxis.Z },
                RotationAxis.Z => new RotationAxis[] { RotationAxis.X, RotationAxis.Y },
                _ => new RotationAxis[] { RotationAxis.Y, RotationAxis.Z },
            };
            return result;
        }
    }
}
