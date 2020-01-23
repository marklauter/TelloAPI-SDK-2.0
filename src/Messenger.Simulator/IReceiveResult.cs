// <copyright file="IReceiveResult.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Messenger.Simulator
{
    public interface IReceiveResult
    {
        byte[] Buffer { get; }
    }
}
