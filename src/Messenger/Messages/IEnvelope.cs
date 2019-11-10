// <copyright file="IEnvelope.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public interface IEnvelope
    {
        /// <summary>
        /// id for use in queuing systems or logs.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// time that the message was created/initiated.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// marshalled data.
        /// </summary>
        byte[] Data { get; }
    }

    public interface IEnvelope<T> : IEnvelope
    {
        /// <summary>
        /// typed message.
        /// </summary>
        T Message { get; }
    }
}
