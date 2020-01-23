// <copyright file="IAttitudeObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities
{
    public interface IAttitudeObservation : IObservation, IAttitude
    {
    }
}
