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
            Rotation = Quaternion.Euler(rot);
        }
    }
}
