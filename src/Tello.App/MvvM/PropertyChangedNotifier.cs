// <copyright file="PropertyChangedNotifier.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tello.App.MvvM
{
    // todo: here are two ideas on INotifyPropertyChanged proxies
    // http://jonas.follesoe.no/oldblog/2009-12-23-automatic-inotifypropertychanged-using-dynamic-proxy/
    // https://ayende.com/blog/4106/nhibernate-inotifypropertychanged
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        protected IUIDispatcher Dispatcher { get; }

        public PropertyChangedNotifier(IUIDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T storage, T value, [CallerMemberName]string callerMemberName = null)
        {
            // todo: the property name might need to be extracted from the callerMemberName - test this to see what the value of callerMemberName before publishing to nuget
            if (!EqualityComparer<T>.Default.Equals(storage, value))
            {
                storage = value;
                this.Dispatcher.Invoke(args => this.PropertyChanged?.Invoke(this, args as PropertyChangedEventArgs), new PropertyChangedEventArgs(callerMemberName));
            }
        }
    }
}
