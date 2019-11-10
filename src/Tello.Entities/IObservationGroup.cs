// <copyright file="IObservationGroup.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Repository;

namespace Tello.Entities
{
    public interface IObservationGroup : IEntity
    {
        int SessionId { get; set; }
    }
}
