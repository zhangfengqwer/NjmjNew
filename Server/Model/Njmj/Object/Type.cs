using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 房间
    /// </summary>
    public enum RoomName : byte
    {
        None,
        ChuJi,
        JingYing,
        Friend
    }

    public enum DismissState : byte
    {
        None,
        Agree,
        Cancel
    }
}
