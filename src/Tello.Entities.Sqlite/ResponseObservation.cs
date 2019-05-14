using Messenger;
using System;

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

            TimeInitiated = response.Request.Timestamp;
            TimeTaken = response.TimeTaken;
            Command = (Command)response.Request.Data;
            Response = response.Message;
            Success = response.Success;
            StatusMessage = response.StatusMessage;
            ExceptionType = response.Exception?.GetType().Name;
            ExceptionType = response.Exception?.GetType().Name;
        }

        #region TimeInitiated
        [SQLite.Indexed]
        public DateTime TimeInitiated { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string TimeInitiatedString
        {
            get => TimeInitiated.ToString("o");
            set { }
        }
        #endregion

        #region TimeTaken
        [SQLite.Ignore]
        public TimeSpan TimeTaken { get; set; }

        [SQLite.Indexed]
        public int TimeTakenMS
        {
            get => (int)TimeTaken.TotalMilliseconds;
            set => TimeTaken = TimeSpan.FromMilliseconds(value);
        }
        #endregion

        #region Command
        [SQLite.Ignore]
        public Commands Command { get; set; }

        [SQLite.Indexed]
        [SQLite.Column("Command")]
        public string CommandValue
        {
            get => Command.ToString();
            set => Command = (Commands)Enum.Parse(typeof(Commands), value);
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
