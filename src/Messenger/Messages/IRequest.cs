// <copyright file="IRequest.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public interface IRequest : IEnvelope
    {
        /// <summary>
        /// some requests take longer than others, so this makes it possible to set per-request timeout.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// don't wait for response.
        /// </summary>
        bool NoWait { get; }
    }

    public interface IRequest<T> : IRequest, IEnvelope<T>
    {
    }
}
