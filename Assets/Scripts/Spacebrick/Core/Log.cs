//
// Log.cs
//
// Author:
//       Evan Reidland <er@evanreidland.com>
//
// Copyright (c) 2014 Evan Reidland
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Spacebrick
{
    public class Log
    {
        public enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
            Exception,
        }

        private static Log _default = new Log();
        public static Log Default { get { return _default; } }

        public virtual void Text(LogType logType, string format, params object[] args)
        {
            string logText = string.Format("[{0}]: {1}", logType, string.Format(format, args));
            #if UNITY_EDITOR || UNITY_STANDALONE
            switch(logType)
            {
            case LogType.Warning:
                UnityEngine.Debug.LogWarning(logText);
                break;
            case LogType.Exception:
            case LogType.Error:
                UnityEngine.Debug.LogError(logText);
                break;
            case LogType.Info:
            case LogType.Debug:
            default:
                UnityEngine.Debug.Log(logText);
                break;
            }
            #else
            System.Console.WriteLine(logText);
            #endif
        }

        public void Debug(string format, params object[] args) { Text(LogType.Debug, format, args); }
        public void Info(string format, params object[] args) { Text(LogType.Debug, format, args); }
        public void Warning(string format, params object[] args) { Text(LogType.Warning, format, args); }
        public void Error(string format, params object[] args) { Text(LogType.Error, format, args); }
        public void Exception(System.Exception e) { Text(LogType.Exception, "{0}: {1} at {2}", e.GetType(), e.Message, e.StackTrace); }
    }

    public class EmptyLogger : Log
    {
        public override void Text (LogType logType, string format, params object[] args) {}
    }
}

