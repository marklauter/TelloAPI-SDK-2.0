// <copyright file="TestObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestObservation : IState
    {
        public DateTime Timestamp { get; set; }
    }
}
