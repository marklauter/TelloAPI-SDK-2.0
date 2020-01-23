// <copyright file="ResponseObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;

namespace Tello.Entities.Sqlite
{
    public sealed class ResponseObservation : Observation, IResponseObservation
    {
        public ResponseObservation()
            : base()
        {
        }

        public ResponseObservation(IResponse<string> response)
            : this(
                  0,
                  response ?? throw new ArgumentNullException(nameof(response)))
        {
        }

        public ResponseObservation(IObservationGroup group, IResponse<string> response)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  response ?? throw new ArgumentNullException(nameof(response)))
        {
        }

        public ResponseObservation(int groupId, IResponse<string> response)
            : base(
                  groupId,
                  (response ?? throw new ArgumentNullException(nameof(response))).Timestamp)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            this.TimeInitiated = response.Request.Timestamp;
            this.TimeTaken = response.TimeTaken;
            this.Command = (Command)response.Request.Data;
            this.Response = response.Message;
            this.Success = response.Success;
            this.StatusMessage = response.StatusMessage;
            this.ExceptionType = response.Exception?.GetType().Name;
            this.ExceptionType = response.Exception?.GetType().Name;
        }

        #region TimeInitiated
        [SQLite.Indexed]
        public DateTime TimeInitiated { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string TimeInitiatedString
        {
            get => this.TimeInitiated.ToString("o");
            set { }
        }
        #endregion

        #region TimeTaken
        [SQLite.Ignore]
        public TimeSpan TimeTaken { get; set; }

        [SQLite.Indexed]
        public int TimeTakenMS
        {
            get => (int)this.TimeTaken.TotalMilliseconds;
            set => this.TimeTaken = TimeSpan.FromMilliseconds(value);
        }
        #endregion

        #region Command

        [SQLite.Ignore]
        public Command Command { get; set; }

        [SQLite.Indexed]
        [SQLite.Column("Command")]
        public string CommandValue
        {
            get => (string)this.Command;
            set => this.Command = (Command)value;
        }

        #endregion

        #region Response
        [SQLite.Indexed]
        public string Response { get; set; }
        #endregion

        #region Success
        [SQLite.Indexed]
        public bool Success { get; set; }
        #endregion

        public string StatusMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }
    }
}
