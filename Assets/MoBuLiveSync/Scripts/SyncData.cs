using System;
using MoBuLiveSync.DataFormat;
using System.Collections.Generic;

namespace MoBuLiveSync
{
    public class SyncData
    {
        public Dictionary<string, SyncCharacter> Characters { get; }
        public Dictionary<string, SyncProp> Props { get; }
        public DateTime CreatedAt { get; }

        public SyncData(Dictionary<string, SyncCharacter> characters, Dictionary<string, SyncProp> props,
            DateTime createdAt)
        {
            Characters = characters;
            Props = props;
            CreatedAt = createdAt;
        }
    }
}
