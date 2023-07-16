using System;
using System.Collections.Generic;

namespace Gateway
{
    public static class BlackListHelper
    {
        private static Dictionary<string, DateTime> _BlackList = new Dictionary<string, DateTime>();

        public static void AddBlackList(string userAccount)
        {
            _BlackList.Add(userAccount, DateTime.UtcNow);
        }

        public static bool CheckInBlackList(string userAccount, DateTime issueDate)
        {
            if (_BlackList.TryGetValue(userAccount, out DateTime dateTime))
            {
                return issueDate > dateTime;
            }
            return true;
        }
    }
}
