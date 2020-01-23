// <copyright file="IDroneMessageHandler.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public interface IDroneMessageHandler
    {
        Task<byte[]> Invoke(byte[] buffer);
    }
}
