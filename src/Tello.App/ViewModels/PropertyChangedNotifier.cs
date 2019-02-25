﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tello.App.ViewModels
{
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        protected IUIDispatcher Dispatcher { get; }

        public PropertyChangedNotifier(IUIDispatcher dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T storage, T value, [CallerMemberName]string callerMemberName = null)
        {
            //todo: the property name needs to be extracted from the callerMemberName 
            if (!EqualityComparer<T>.Default.Equals(storage, value))
            {
                storage = value;
                Dispatcher.Invoke((args) => { PropertyChanged?.Invoke(this, args as PropertyChangedEventArgs); }, new PropertyChangedEventArgs(callerMemberName));
            }
        }
    }
}