using System;
using UnityEngine;

namespace Treevel.Common.Managers
{
    public static class ScheduleManager
    {
        public static void AddEvent(MonoBehaviour actor, string methodName, DateTime dateTime)
        {
            var seconds = (float)(dateTime - DateTime.Now).TotalSeconds;

            if (seconds <= 0) return;

            actor.Invoke(methodName, seconds);
        }
    }
}
