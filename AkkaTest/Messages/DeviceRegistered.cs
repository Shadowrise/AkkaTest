using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaTest.Messages
{
    public sealed class DeviceRegistered
    {
        public static DeviceRegistered Instance { get; } = new DeviceRegistered();
        private DeviceRegistered() { }
    }
}
