// <copyright file="IStateObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tello.Entities
{
    public interface IStateObservation : IObservation
    {
        string Data { get; set; }
    }
}
