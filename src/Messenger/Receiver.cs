// <copyright file="Receiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/dotnet/api/system.iobservable-1?view=netstandard-2.0
namespace Messenger
{
    public abstract class Receiver : IReceiver, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = null;

        public async void Start()
        {
            if (this.cancellationTokenSource == null)
            {
                this.cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(async () => await this.Listen(this.cancellationTokenSource.Token));
            }
        }

        public void Stop()
        {
            if (this.cancellationTokenSource != null
                && !this.cancellationTokenSource.IsCancellationRequested)
            {
                this.cancellationTokenSource.Cancel(false);
                this.cancellationTokenSource.Dispose();
                this.cancellationTokenSource = null;
            }
        }

        protected abstract Task Listen(CancellationToken cancellationToken);

        protected void MessageReceived(IEnvelope message)
        {
            foreach (var observer in this.observers.Keys)
            {
                observer.OnNext(message);
            }
        }

        protected void ExceptionThrown(Exception exception)
        {
            foreach (var observer in this.observers.Keys)
            {
                observer.OnError(exception);
            }
        }

        private readonly ConcurrentDictionary<IObserver<IEnvelope>, object> observers = new ConcurrentDictionary<IObserver<IEnvelope>, object>();

        public IDisposable Subscribe(IObserver<IEnvelope> observer)
        {
            if (!this.observers.ContainsKey(observer))
            {
                this.observers[observer] = null;
            }

            return new Unsubscriber(this.observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<IObserver<IEnvelope>, object> observers;
            private readonly IObserver<IEnvelope> observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<IEnvelope>, object> observers, IObserver<IEnvelope> observer)
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

        public void Dispose()
        {
            this.Stop();

            foreach (var observer in this.observers.Keys)
            {
                observer.OnCompleted();
            }

            this.observers.Clear();
        }
    }
}
