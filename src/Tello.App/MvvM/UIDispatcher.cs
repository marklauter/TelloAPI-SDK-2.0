// <copyright file="UIDispatcher.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;

namespace Tello.App.MvvM
{
    // todo: i have no idea if this will work or how to test without an actual UWP app running
    public class UIDispatcher : IUIDispatcher
    {
        private readonly SynchronizationContext context;

        private UIDispatcher()
        {
        }

        /// <summary>
        /// pass main thread context.
        /// </summary>
        /// <param name="context"></param>
        public UIDispatcher(SynchronizationContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Invoke(Action<object> action, object state)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.context.Post(new SendOrPostCallback(action), state);
        }

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.context.Post(new SendOrPostCallback(state => { action(); }), null);
        }
    }
}
