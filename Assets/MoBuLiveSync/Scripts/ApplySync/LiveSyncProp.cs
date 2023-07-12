using MoBuLiveSync.DataFormat;
using UnityEngine;

namespace MoBuLiveSync.ApplySync
{
    [AddComponentMenu("MBSync/LiveSyncProp", 11)]
    public class LiveSyncProp : LiveSyncBase<SyncProp>
    {
        private Transform _transform;

        [ContextMenu("Initialize LiveSyncProp")]
        public override void Initialize() => base.Initialize();

        private void OnEnable()
        {
            Initialize();
            _transform = transform;
        }

        private void Update()
        {
            if (_isEnableSync == false || _liveSyncManager == null)
                return;

            var syncProp = _liveSyncManager.GetSyncProp(_subjectName);
            if (syncProp == null)
                return;

            if (!syncProp.IsUpdateRequired(LastSyncTime))
                return;

            UpdateFromSyncInfo(syncProp);
            LastSyncTime = syncProp.CreatedAt;
        }

        protected override void UpdateFromSyncInfo(SyncProp syncProp)
        {
            var syncObject = syncProp.Object;
            _transform.localPosition = syncObject.Position;
            _transform.rotation = syncObject.Rotation;
        }
    }
}
