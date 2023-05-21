using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MoBuLiveSync.DataFormat
{
    internal static class PacketDeserializer
    {
        private static int SyncObjectSize => sizeof(float) * 3 * 2;

        private static IEnumerable<HumanBodyBones> StreamHumanBones { get; } = new List<HumanBodyBones>()
        {
            HumanBodyBones.Hips,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightFoot,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.UpperChest,
            HumanBodyBones.Neck,
            HumanBodyBones.Head,
            HumanBodyBones.LeftShoulder,
            HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightHand,
            HumanBodyBones.LeftToes,
            HumanBodyBones.RightToes,
            HumanBodyBones.LeftThumbProximal,
            HumanBodyBones.LeftThumbIntermediate,
            HumanBodyBones.LeftThumbDistal,
            HumanBodyBones.LeftIndexProximal,
            HumanBodyBones.LeftIndexIntermediate,
            HumanBodyBones.LeftIndexDistal,
            HumanBodyBones.LeftMiddleProximal,
            HumanBodyBones.LeftMiddleIntermediate,
            HumanBodyBones.LeftMiddleDistal,
            HumanBodyBones.LeftRingProximal,
            HumanBodyBones.LeftRingIntermediate,
            HumanBodyBones.LeftRingDistal,
            HumanBodyBones.LeftLittleProximal,
            HumanBodyBones.LeftLittleIntermediate,
            HumanBodyBones.LeftLittleDistal,
            HumanBodyBones.RightThumbProximal,
            HumanBodyBones.RightThumbIntermediate,
            HumanBodyBones.RightThumbDistal,
            HumanBodyBones.RightIndexProximal,
            HumanBodyBones.RightIndexIntermediate,
            HumanBodyBones.RightIndexDistal,
            HumanBodyBones.RightMiddleProximal,
            HumanBodyBones.RightMiddleIntermediate,
            HumanBodyBones.RightMiddleDistal,
            HumanBodyBones.RightRingProximal,
            HumanBodyBones.RightRingIntermediate,
            HumanBodyBones.RightRingDistal,
            HumanBodyBones.RightLittleProximal,
            HumanBodyBones.RightLittleIntermediate,
            HumanBodyBones.RightLittleDistal
        };

        internal static SyncData DeserializeSyncData(ReadOnlySpan<byte> buffer)
        {
            var unixTime = DeserializeDateTime(ref buffer);
            var props = DeserializeProps(ref buffer, unixTime);
            var characters = DeserializeCharacters(ref buffer, unixTime);

            return new SyncData(characters, props, unixTime);
        }

        private static DateTime DeserializeDateTime(ref ReadOnlySpan<byte> buffer)
        {
            var value = BitConverter.ToInt64(buffer);
            buffer = buffer[sizeof(long)..];
            return DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;
        }

        private static Dictionary<string, SyncProp> DeserializeProps(ref ReadOnlySpan<byte> buffer, DateTime unixTime)
        {
            var propCount = buffer[0];
            buffer = buffer[1..];

            var data = new (string, SyncProp)[propCount];
            for (var i = 0; i < propCount; i++)
            {
                var name = DeserializeName(ref buffer);
                var syncProp = DeserializeProp(ref buffer, unixTime);
                data[i] = (name, syncProp);
            }

            return data.ToDictionary(x => x.Item1, x => x.Item2);
        }

        private static Dictionary<string, SyncCharacter> DeserializeCharacters(ref ReadOnlySpan<byte> buffer,
            DateTime unixTime)
        {
            var characterCount = buffer[0];
            buffer = buffer[1..];

            var data = new (string, SyncCharacter)[characterCount];
            for (var i = 0; i < characterCount; i++)
            {
                var name = DeserializeName(ref buffer);
                var syncCharacter = DeserializeSyncCharacter(ref buffer, unixTime);
                data[i] = (name, syncCharacter);
            }

            return data.ToDictionary(x => x.Item1, x => x.Item2);
        }

        private static string DeserializeName(ref ReadOnlySpan<byte> buffer)
        {
            var size = buffer[0];
            var nameBuffer = buffer.Slice(1, size);
            var name = Encoding.UTF8.GetString(nameBuffer);

            buffer = buffer[(1 + size)..];
            return name;
        }

        private static SyncProp DeserializeProp(ref ReadOnlySpan<byte> buffer, DateTime unixTime)
        {
            var propBuffer = buffer[..SyncObjectSize];
            var data = new Vector3[2];
            MemoryMarshal.Cast<byte, Vector3>(propBuffer).CopyTo(data);

            buffer = buffer[SyncObjectSize..];
            return new SyncProp(data[0], data[1], unixTime);
        }

        private static SyncCharacter DeserializeSyncCharacter(ref ReadOnlySpan<byte> buffer, DateTime unixTime)
        {
            var boneCount = buffer[0];
            var streamBoneIndex = new byte[boneCount];
            buffer.Slice(1, boneCount).CopyTo(streamBoneIndex);
            buffer = buffer[(1 + boneCount)..];

            var boneData = DeserializeSyncObjects(ref buffer, boneCount);

            var character = streamBoneIndex
                .Select(index => StreamHumanBones.ElementAt(index))
                .Zip(boneData, (id, obj) => (id, obj))
                .ToDictionary(x => x.id, x => x.obj);
            return new SyncCharacter(character, unixTime);
        }

        private static IEnumerable<SyncObject> DeserializeSyncObjects(ref ReadOnlySpan<byte> buffer, byte count)
        {
            var data = new SyncObject[count];
            var dataLength = SyncObjectSize * count;
            var dataBuffer = buffer[..dataLength];
            var vectors = new Vector3[count * 2];
            MemoryMarshal.Cast<byte, Vector3>(dataBuffer).CopyTo(vectors);

            for (var i = 0; i < count; i++)
            {
                var syncObject = new SyncObject(vectors[i * 2], vectors[i * 2 + 1]);
                data[i] = syncObject;
            }

            buffer = buffer[dataLength..];
            return data;
        }
    }
}
