using System;
using UnityEngine;

namespace MoBuLiveSync.DataFormat
{
    public class SyncProp : SyncInfoBase
    {
        public SyncObject Object { get; }


        internal SyncProp(Vector3 pos, Vector3 rot, DateTime createdAt) : base(createdAt)
        {
            Object = new SyncObject(pos, rot);
        }
    }
}
