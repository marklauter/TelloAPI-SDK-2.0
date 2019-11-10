// <copyright file="IResponse.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public interface IResponse : IEnvelope
    {
        /// <summary>
        /// request that initiated the response.
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// Time taken from the moment the request crosses the transit boundary until the response is received.
        /// TimeTaken != Respose.Timestamp - Request.Timestamp because request might have been queued for a while before sending.
        /// </summary>
        TimeSpan TimeTaken { get; }

        /// <summary>
        /// if false check the status message and exception property.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// message regarding server status in case of failure.
        /// </summary>
        string StatusMessage { get; }

        /// <summary>
        /// populated on failure.
        /// </summary>
        Exception Exception { get; }
    }

    public interface IResponse<T> : IResponse, IEnvelope<T>
    {
    }
}
