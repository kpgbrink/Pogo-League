// MIT License
//
// Copyright(c) 2018 Ryan Hipple
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoLevelMenu.Events
{
    public abstract class GameEvent<TResponse> : ScriptableObject
    {
        public const string eventAssetMenuStart = "Event";
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        readonly List<IGameEventListener<TResponse>> eventListeners
            = new List<IGameEventListener<TResponse>>();

        protected void RaiseAll(Action<TResponse> callResponse)
        {
            for (var i = eventListeners.Count - 1; i >= 0; i--)
            {
                callResponse(eventListeners[i].TheResponse);
            }
        }

        public void RegisterListener(IGameEventListener<TResponse> listener)
        {
            if (!eventListeners.Contains(listener))
            {
                eventListeners.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener<TResponse> listener)
        {
            if (eventListeners.Contains(listener))
            {
                eventListeners.Remove(listener);
            }
        }
    }

    [CreateAssetMenu(menuName = GameEvent.eventAssetMenuStart + "/" + nameof(GameEvent))]
    public class GameEvent : GameEvent<UnityEvent>
    {
        public void Raise()
        {
            RaiseAll(response => response.Invoke());
        }
    }
}
