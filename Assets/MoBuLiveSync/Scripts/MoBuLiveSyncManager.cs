using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using MoBuLiveSync.DataFormat;
using UnityEngine;

namespace MoBuLiveSync
{
    public class MoBuLiveSyncManager : MonoBehaviour
    {
        private const int DEFAULT_PORT = 49000;

        [SerializeField] private bool _isEnableReceive = true;
        [SerializeField] public int _port = DEFAULT_PORT;

        [SerializeField] [HideInInspector] private int _receiveRate;
        [SerializeField] [HideInInspector] private int _lastReceiveSize;

        public static string Version => "v0.0.1";
        public bool IsRunning => _udpReceiver?.IsRunning == true;
        public int ReceivingPort => _udpReceiver?.Port ?? 0;
        public DateTime LastReceivedAt { get; private set; }


        private UdpReceiver _udpReceiver;
        private int _receiveCount;
        private readonly Dictionary<string, SyncCharacter> _characterDict = new();
        private readonly Dictionary<string, SyncProp> _propDict = new();

        private void Update()
        {
            switch (IsRunning)
            {
                case true when _isEnableReceive == false:
                    StopUdpReceive();
                    break;
                case false when _isEnableReceive:
                    StartUdpReceive();
                    break;
            }
        }

        private void OnEnable()
        {
            if (_isEnableReceive)
                StartUdpReceive();

            StartCoroutine(SpeedCounter());
        }

        private void OnDisable() => StopUdpReceive();
        private void OnDestroy() => StopUdpReceive();

        private void StartUdpReceive()
        {
            _udpReceiver ??= new UdpReceiver(_port);
            _udpReceiver.OnReceivedPacket -= OnReceivedPacket;
            _udpReceiver.StartReceive();
            _udpReceiver.OnReceivedPacket += OnReceivedPacket;
        }

        private void StopUdpReceive()
        {
            if (_udpReceiver == null)
                return;

            _udpReceiver.StopReceive();
            _udpReceiver.OnReceivedPacket -= OnReceivedPacket;
            _udpReceiver = null;
        }

        /// <summary>
        /// callback when received packet.
        /// </summary>
        /// <param name="receiveBuffer"></param>
        /// <param name="length"></param>
        /// <param name="remoteEp"></param>
        private void OnReceivedPacket(byte[] receiveBuffer, int length, EndPoint remoteEp)
        {
            if (receiveBuffer == null || length == 0)
                return;
            if (!SyncPacket.IsValid(receiveBuffer, length)) return;

            var packet = new SyncPacket(receiveBuffer, length);
            var syncData = packet.ToSyncData();
            if (syncData == null) return;

            UpdateSyncData(syncData);
            LastReceivedAt = DateTime.Now;
            _receiveCount++;
            _lastReceiveSize = length;
        }


        /// <summary>
        /// update sync data to local cache.
        /// </summary>
        /// <param name="syncData"></param>
        private void UpdateSyncData(SyncData syncData)
        {
            var characters = syncData.Characters;
            foreach (var (subjectName, character) in characters)
            {
                _characterDict[subjectName] = character;
            }

            var props = syncData.Props;
            foreach (var (subjectName, prop) in props)
            {
                _propDict[subjectName] = prop;
            }
        }

        /// <summary>
        /// Get SyncCharacter by subjectName. If not found, return null.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        [CanBeNull]
        public SyncCharacter GetSyncCharacter(string subjectName)
        {
            _characterDict.TryGetValue(subjectName, out var character);
            return character;
        }

        /// <summary>
        /// Get character subject names.
        /// </summary>
        public List<string> CharacterSubjectNames => _characterDict.Keys.ToList();


        /// <summary>
        /// Get Prop SyncObject by subjectName. If not found, return null.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        [CanBeNull]
        public SyncProp GetSyncProp(string subjectName)
        {
            _propDict.TryGetValue(subjectName, out var prop);
            return prop;
        }

        /// <summary>
        /// Get prop subject names.
        /// </summary>
        public List<string> PropSubjectNames => _propDict.Keys.ToList();


        private IEnumerator SpeedCounter()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                _receiveRate = _receiveCount;
                _receiveCount = 0;
            }
        }
    }
}
