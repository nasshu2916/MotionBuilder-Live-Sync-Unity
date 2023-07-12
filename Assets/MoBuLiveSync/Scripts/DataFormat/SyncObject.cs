using UnityEngine;

namespace MoBuLiveSync.DataFormat
{
    public class SyncObject
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        internal SyncObject(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = ToRotation(rot);
        }

        private static Quaternion ToRotation(Vector3 eulerRotation)
        {
            var xQuaternion = Quaternion.AngleAxis(eulerRotation.x, new Vector3(1f, 0.0f, 0.0f));
            var yQuaternion = Quaternion.AngleAxis(eulerRotation.y, new Vector3(0.0f, 1f, 0.0f));
            var zQuaternion = Quaternion.AngleAxis(eulerRotation.z, new Vector3(0.0f, 0.0f, 1f));
            return zQuaternion * yQuaternion * xQuaternion;
        }
    }
}
