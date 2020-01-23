// <copyright file="ITransceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace Messenger
{
    public interface ITransceiver : IDisposable
    {
        /// <summary>
        /// send request, receive response.
        /// </summary>
        Task<IResponse> SendAsync(IRequest request);
    }
}
