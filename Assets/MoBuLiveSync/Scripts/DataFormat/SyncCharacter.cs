using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoBuLiveSync.DataFormat
{
    public class SyncCharacter : SyncInfoBase
    {
        public Dictionary<HumanBodyBones, SyncObject> BoneDict { get; }


        internal SyncCharacter(Dictionary<HumanBodyBones, SyncObject> boneDict, DateTime createdAt)
            : base(createdAt)
        {
            BoneDict = boneDict;
        }
    }
}
