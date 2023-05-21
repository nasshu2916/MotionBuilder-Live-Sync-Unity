using System;

namespace MoBuLiveSync.DataFormat
{
    public abstract class SyncInfoBase
    {
        protected SyncInfoBase(DateTime createdAt)
        {
            CreatedAt = createdAt;
        }

        public DateTime CreatedAt { get; }


        public bool IsUpdateRequired(DateTime lastSyncTime)
        {
            return CreatedAt > lastSyncTime;
        }
    }
}
