// <copyright file="IResponseObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.Entities
{
    public interface IResponseObservation : IObservation
    {
        /// <summary>
        /// IRequest.Timestamp.
        /// </summary>
        DateTime TimeInitiated { get; set; }

        /// <summary>
        /// IResponse.TimeTaken.
        /// </summary>
        TimeSpan TimeTaken { get; set; }

        /// <summary>
        /// IRequest.Data (after parsing).
        /// </summary>
        Command Command { get; set; }

        /// <summary>
        /// IResponse.Data (after parsing).
        /// </summary>
        string Response { get; set; }

        /// <summary>
        /// IResponse.Success.
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// IResponse.StatusMessage.
        /// </summary>
        string StatusMessage { get; set; }

        /// <summary>
        /// IResponse.Exception.
        /// </summary>
        string ExceptionType { get; set; }

        /// <summary>
        /// IResponse.Exception.
        /// </summary>
        string ExceptionMessage { get; set; }

        /// <summary>
        /// IResponse.Exception.
        /// </summary>
        string ExceptionStackTrace { get; set; }
    }
}
