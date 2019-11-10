// <copyright file="StateObserver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using Messenger;
using Tello.Events;
using Tello.State;

namespace Tello.Controller
{
    public sealed class StateObserver : Observer<IEnvelope>, IStateObserver
    {
        public StateObserver(IReceiver receiver)
            : base(receiver)
        {
        }

        public override void OnError(Exception error)
        {
            try
            {
                this.ExceptionThrown?.Invoke(this, error);
            }
            catch
            {
            }
        }

        public override void OnNext(IEnvelope message)
        {
            try
            {
                this.State = new TelloState(Encoding.UTF8.GetString(message.Data), message.Timestamp, this.position);
                this.StateChanged?.Invoke(this, new StateChangedArgs(this.State));
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        public void UpdatePosition(object sender, PositionChangedArgs e)
        {
            this.position = e.Position;
        }

        private Vector position = new Vector();

        public ITelloState State { get; private set; }

        public event EventHandler<StateChangedArgs> StateChanged;

        public event EventHandler<Exception> ExceptionThrown;
    }
}
