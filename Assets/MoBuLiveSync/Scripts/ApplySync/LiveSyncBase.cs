using MoBuLiveSync.DataFormat;
using System;
using UnityEngine;

namespace MoBuLiveSync.ApplySync
{
    public abstract class LiveSyncBase<TSyncData> : MonoBehaviour where TSyncData : SyncInfoBase
    {
        [SerializeField] protected MoBuLiveSyncManager _liveSyncManager;
        [SerializeField] protected string _subjectName;
        [SerializeField] protected bool _isEnableSync = true;

        protected DateTime LastSyncTime;

        public virtual void Initialize()
        {
            if (_liveSyncManager == null)
            {
                _liveSyncManager = FindObjectOfType<MoBuLiveSyncManager>();
                if (_liveSyncManager == null)
                {
                    Debug.LogWarning("[LiveSyncBase] MoBuLiveSyncManager is not found.");
                }
            }

            if (string.IsNullOrWhiteSpace(_subjectName))
            {
                _subjectName = name;
            }
        }

        protected abstract void UpdateFromSyncInfo(TSyncData syncData);

        public static void AddAndInitialize<T>(GameObject gameObject) where T : LiveSyncBase<TSyncData>
        {
            if (gameObject == null)
                return;

            if (gameObject.GetComponent<T>() != null)
            {
                Debug.LogWarning($"[{typeof(T).Name}] {typeof(T).Name} is already added.");
                return;
            }

            var liveSyncComponent = gameObject.AddComponent<T>();
            liveSyncComponent.Initialize();
        }
    }
}
