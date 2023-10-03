using System;
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
                RotationAxis.Z => Vector3.forward,
                _ => Vector3.up, // Including RotationAxis.Y
            };
            return result;
        }

        public static Vector3 ToVector3Axis(this RotationAxis rotationAxis, Transform transform)
        {
            Vector3 result = rotationAxis switch
            {
                RotationAxis.X => transform.right,
                RotationAxis.Z => transform.forward,
                _ => transform.up, // Including RotationAxis.Y
            };
            return result;
        }

        public static float GetAxisValue(this RotationAxis rotationAxis, Vector3 vector3)
        {
            float result = rotationAxis switch
            {
                RotationAxis.X => vector3.x,
                RotationAxis.Z => vector3.z,
                _ => vector3.y, // Including RotationAxis.Y
            };
            return result;
        }

        public static Quaternion EulerAxisValue(this RotationAxis rotationAxis, Vector3 fixedPart, Vector3 rotationPart)
        {
            Quaternion result = rotationAxis switch
            {
                RotationAxis.X => Quaternion.Euler(rotationPart.x, fixedPart.y, fixedPart.z),
                RotationAxis.Z => Quaternion.Euler(fixedPart.x, fixedPart.y, rotationPart.z),
                _ => Quaternion.Euler(fixedPart.x, rotationPart.y, fixedPart.z), // Including RotationAxis.Y
            };
            return result;
        }

        public static Tuple<Vector3, Vector3> OthersVector3Axis(this RotationAxis rotationAxis, Transform transform)
        {
            Tuple<Vector3, Vector3> result = rotationAxis switch
            {
                RotationAxis.X => new(transform.up, transform.forward),
                RotationAxis.Z => new(transform.right, transform.up),
                _ => new(transform.right, transform.forward), // Including RotationAxis.Y
            };
            return result;
        }
    }
}
