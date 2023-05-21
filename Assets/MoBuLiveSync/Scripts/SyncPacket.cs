using MoBuLiveSync.DataFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MoBuLiveSync
{
    public class SyncPacket
    {
        // Packet Header Index const
        private const int HEADER_SIZE = 14;
        private const string IDENTIFICATION_TEXT = "MBLive";
        private const int VERSION_INDEX = 8;
        private const int VERSION = 0x01;
        private const int ENCODE_TYPE_INDEX = 7;

        private enum EncodeType : byte
        {
            Original = 0x00
        }

        private readonly EncodeType _encodeType;
        private readonly byte[] _data;

        internal SyncPacket(byte[] buffer, int length)
        {
            _encodeType = (EncodeType)buffer[ENCODE_TYPE_INDEX];

            var dataSize = length - HEADER_SIZE;
            _data = new byte[dataSize];
            Buffer.BlockCopy(buffer, HEADER_SIZE, _data, 0, dataSize);
        }

        private static readonly byte[] IdentificationIds = Encoding.ASCII.GetBytes(IDENTIFICATION_TEXT);

        internal static bool IsValid(byte[] buffer, int length)
        {
            if (length < HEADER_SIZE)
                return false;
            if (IdentificationIds.Where((t, i) => buffer[i] != t).Any())
                return false;
            return buffer[VERSION_INDEX] == VERSION;
        }


        internal SyncData ToSyncData()
        {
            return _encodeType switch
            {
                EncodeType.Original => PacketDeserializer.DeserializeSyncData(_data),
                _ => null
            };
        }
    }
}
