// <copyright file="Observer.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public abstract class Observer<T> : IObserver<T>
    {
        private IDisposable unsubscriber;

        public Observer()
        {
        }

        public Observer(IObservable<T> observable)
        {
            this.Subscribe(observable);
        }

        public void Subscribe(IObservable<T> observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (this.unsubscriber == null)
            {
                this.unsubscriber = observable.Subscribe(this);
            }
        }

        public void Unsubscribe()
        {
            if (this.unsubscriber != null)
            {
                this.unsubscriber.Dispose();
                this.unsubscriber = null;
            }
        }

        public void OnCompleted()
        {
            this.Unsubscribe();
        }

        public abstract void OnError(Exception error);

        public abstract void OnNext(T message);
    }
}
