// <copyright file="ISession.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Repository;

namespace Tello.Entities
{
    public interface ISession : IEntity
    {
        DateTime StartTime { get; set; }

        TimeSpan Duration { get; set; }
    }
}
