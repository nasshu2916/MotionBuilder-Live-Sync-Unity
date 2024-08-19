using MoBuLiveSync.DataFormat;
using UnityEngine;

namespace MoBuLiveSync.ApplySync
{
    [AddComponentMenu("MBSync/LiveSyncCharacter", 10)]
    public class LiveSyncCharacter : LiveSyncBase<SyncCharacter>
    {
        [SerializeField] private Animator _animator;

        [ContextMenu("Initialize LiveSyncCharacter")]
        public override void Initialize()
        {
            base.Initialize();
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                {
                    Debug.LogWarning("[LiveSyncCharacter] Animator is not found.");
                }
            }
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Update()
        {
            if (_isEnableSync == false || _liveSyncManager == null || _animator == null)
                return;

            var syncCharacter = _liveSyncManager.GetSyncCharacter(_subjectName);
            if (syncCharacter == null)
                return;

            if (!syncCharacter.IsUpdateRequired(LastSyncTime))
                return;

            UpdateFromSyncInfo(syncCharacter);
            LastSyncTime = syncCharacter.CreatedAt;
        }


        protected override void UpdateFromSyncInfo(SyncCharacter syncData)
        {
            foreach (var (bone, boneData) in syncData.BoneDict)
            {
                var boneTransform = _animator.GetBoneTransform(bone);
                if (boneTransform == null)
                    continue;
                boneTransform.rotation = boneData.Rotation;

                if (bone == HumanBodyBones.Hips)
                {
                    boneTransform.localPosition = boneData.Position;
                }
            }
        }
    }
}
