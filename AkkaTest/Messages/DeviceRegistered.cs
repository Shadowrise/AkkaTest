using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaTest.Messages
{
    public sealed class DeviceRegistered
    {
        public string GroupId { get; private set; }
        public string DeviceId { get; private set; }

        public DeviceRegistered(string groupId, string deviceId)
        {
            GroupId = groupId;
            DeviceId = deviceId;
        }
    }
}
