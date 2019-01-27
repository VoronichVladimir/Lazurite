﻿using System;
using System.Threading.Tasks;

namespace Lazurite.Utils
{
    public static class TaskUtils
    {
        public static void Start(Action action, Action<Exception> onException = null, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            if (onException == null)
                Task.Factory.StartNew(action, creationOptions);
            else
                Task.Factory.StartNew(action, creationOptions).ContinueWith((t) => onException(t.Exception.InnerException ?? t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void StartLongRunning(Action action, Action<Exception> onException = null)
        {
            Start(action, onException, TaskCreationOptions.LongRunning);
        }

        public static T Wait<T>(Task<T> task)
        {
            try
            {
                task.Wait();
                return task.Result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
