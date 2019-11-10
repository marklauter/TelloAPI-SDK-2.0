// <copyright file="Messenger.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;

namespace Messenger
{
    public class Messenger<T> : IMessenger<T>, IDisposable
    {
        protected void ReponseReceived(IResponse<T> response)
        {
            foreach (var observer in this.observers.Keys)
            {
                try
                {
                    observer.OnNext(response);
                }
                catch (Exception ex)
                {
                    this.ExceptionThrown(ex);
                }
            }
        }

        protected void ExceptionThrown(Exception exception)
        {
            foreach (var observer in this.observers.Keys)
            {
                try
                {
                    observer.OnError(exception);
                }
                catch
                {
                }
            }
        }

        private readonly ConcurrentDictionary<IObserver<IResponse<T>>, object> observers = new ConcurrentDictionary<IObserver<IResponse<T>>, object>();

        public IDisposable Subscribe(IObserver<IResponse<T>> observer)
        {
            if (!this.observers.ContainsKey(observer))
            {
                this.observers[observer] = null;
            }

            return new Unsubscriber(this.observers, observer);
        }

        public void Dispose()
        {
            foreach (var observer in this.observers.Keys)
            {
                observer.OnCompleted();
            }

            this.observers.Clear();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<IObserver<IResponse<T>>, object> observers;
            private readonly IObserver<IResponse<T>> observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<IResponse<T>>, object> observers, IObserver<IResponse<T>> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (this.observer != null && this.observers.ContainsKey(this.observer))
                {
                    while (!this.observers.TryRemove(this.observer, out _))
                    {
                    }
                }
            }
        }
    }
}
