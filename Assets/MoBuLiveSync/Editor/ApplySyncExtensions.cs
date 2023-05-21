using MoBuLiveSync.ApplySync;
using UnityEditor;
using UnityEngine;

namespace MoBuLiveSync
{
    public static class ApplySyncExtensions
    {
        [MenuItem("GameObject/MBSync/Add Sync Character", true, 10)]
        private static bool ValidateAddSyncCharacter()
        {
            return Selection.activeTransform;
        }

        [MenuItem("GameObject/MBSync/Add Sync Character", false, 10)]
        private static void AddSyncCharacter()
        {
            var selectedTransform = Selection.activeTransform;
            LiveSyncCharacter.AddAndInitialize<LiveSyncCharacter>(selectedTransform.gameObject);
        }

        [MenuItem("GameObject/MBSync/Add Sync Prop", true, 11)]
        private static bool ValidateAddSyncProp()
        {
            return Selection.activeTransform;
        }

        [MenuItem("GameObject/MBSync/Add Sync Prop", false, 11)]
        private static void AddSyncProp()
        {
            var selectedTransform = Selection.activeTransform;
            LiveSyncProp.AddAndInitialize<LiveSyncProp>(selectedTransform.gameObject);
        }

        [MenuItem("CONTEXT/Animator/MBSync/Add Sync Character", false, 10)]
        private static void AddSyncCharacter(MenuCommand menuCommand)
        {
            var animator = menuCommand.context as Animator;
            if (animator != null)
                LiveSyncCharacter.AddAndInitialize<LiveSyncCharacter>(animator.gameObject);
        }


        [MenuItem("CONTEXT/Transform/MBSync/Add Sync Prop", false, 11)]
        private static void AddSyncProp(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            if (transform != null)
                LiveSyncProp.AddAndInitialize<LiveSyncProp>(transform.gameObject);
        }
    }
}
