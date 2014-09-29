//
// EventHub.cs
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
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Spacebrick
{
    public interface IEventCallback
    {
        Type EventType { get; }
        void Execute(object eventObject);
    }

    public class MethodEventCallback : IEventCallback
    {
        private object[] _parameters = new object[1];
        private MethodInfo _methodInfo;
        private object _methodTarget;

        public Type EventType { get; private set; }

        public void Execute(object eventObject)
        {
            _parameters[0] = eventObject;
            _methodInfo.Invoke(_methodTarget, _parameters);
        }

        public MethodEventCallback(MethodInfo method, object methodTarget)
        {
            _methodInfo = method;
            _methodTarget = methodTarget;
            EventType = method.GetParameters()[0].ParameterType;
        }

        public MethodEventCallback(Delegate delegateInstance) : this(delegateInstance.Method, delegateInstance.Target)
        {
        }
    }

    public class EventCallbackSubscription
    {
        private EventCallbackList _callbackList;
        private IEventCallback _callback;

        public bool Canceled { get; private set; }

        public void Cancel()
        {
            if (!Canceled)
            {
                Canceled = true;
                _callbackList.RemoveCallback(_callback);
            }
        }

        public EventCallbackSubscription(EventCallbackList callbackList, IEventCallback callback)
        {
            _callbackList = callbackList;
            _callback = callback;
        }
    }

    public class EventCallbackList
    {
        public int Key { get; private set; }
        public Type EventType { get; private set; }

        private List<IEventCallback> _eventCallbacks = new List<IEventCallback>();

        public EventCallbackSubscription AddReceiver(IEventCallback callback)
        {
            if (callback.EventType == EventType)
            {
                var subscription = new EventCallbackSubscription(this, callback);
                _eventCallbacks.Add(callback);
                return subscription;
            }
            return null;
        }

        public EventCallbackSubscription AddReceiver<T>(Action<T> callback)
        {
            if (typeof(T).Equals(EventType))
            {
                var icallback = new MethodEventCallback(callback);
                return AddReceiver(icallback);
            }
            return null;
        }

        public void RemoveCallback(IEventCallback callback)
        {
            _eventCallbacks.Remove(callback);
        }

        public void Execute(object instance)
        {
            for (int i = 0; i < _eventCallbacks.Count; i++)
            {
                try
                {
                    _eventCallbacks[i].Execute(instance);
                }
                catch (Exception e) { Log.Default.Exception(e); }
            }
        }

        public EventCallbackList(Type key)
        {
            EventType = key;
            Key = EventHub.GetKey(key);
        }
    }

    public class EventHub
    {
        public static int GetKey(Type type) { return type.GetHashCode(); }
        private Dictionary<int, EventCallbackList> _callbackListsByKey = new Dictionary<int, EventCallbackList>();

        public EventCallbackList GetList(Type key)
        {
            EventCallbackList list;
            int intKey = GetKey(key);
            if (!_callbackListsByKey.TryGetValue(intKey, out list))
            {
                list = new EventCallbackList(key);
                _callbackListsByKey.Add(intKey, list);
            }

            return list;
        }

        public EventCallbackSubscription AddReceiver(IEventCallback callback)
        {
            var list = GetList(callback.EventType);
            return list.AddReceiver(callback);
        }

        public EventCallbackSubscription AddReceiver<T>(Action<T> callback)
        {
            var list = GetList(typeof(T));
            return list.AddReceiver<T>(callback);
        }
    }
}

