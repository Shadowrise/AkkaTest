﻿using System.Collections.Generic;

namespace AkkaTest.Messages
{
    public sealed class ReplyDeviceList
    {
        public ReplyDeviceList(long requestId, ISet<string> ids)
        {
            RequestId = requestId;
            Ids = ids;
        }

        public long RequestId { get; }
        public ISet<string> Ids { get; }
    }
}