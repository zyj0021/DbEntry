﻿using System;
using Leafing.Core.Logging;
using Leafing.Data;

namespace Leafing.Extra.Logging
{
    public class DatabaseLogRecorder : ILogRecorder
    {
        public void ProcessLog(SysLogType type, string name, string message, Exception exception)
        {
            var li = new LeafingLog(type, name, message, exception);
            try
            {
                DbEntry.Save(li);
            }
            catch (Exception ex)
            {
                string msg = (exception == null) ? message : message + "\n" + exception;
                if(Logger.System.LogRecorders != null)
                {
                    foreach(var recorder in Logger.System.LogRecorders)
                    {
                        recorder.ProcessLog(type, name, msg, ex);
                    }
                }
            }
        }
    }
}
