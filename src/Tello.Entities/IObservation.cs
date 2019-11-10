// <copyright file="IObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Repository;

namespace Tello.Entities
{
    public interface IObservation : IEntity
    {
        int GroupId { get; set; }

        DateTime Timestamp { get; set; }
    }
}
