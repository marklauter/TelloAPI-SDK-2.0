// <copyright file="CommandTests.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tello.Test
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CardinalDirections_enum_cast_tostring()
        {
            Assert.AreEqual('b', (char)(CardinalDirection)CardinalDirections.Back);
            Assert.AreEqual('f', (char)(CardinalDirection)CardinalDirections.Front);
            Assert.AreEqual('l', (char)(CardinalDirection)CardinalDirections.Left);
            Assert.AreEqual('r', (char)(CardinalDirection)CardinalDirections.Right);
        }

        [TestMethod]
        public void Commands_enum_cast_to_command()
        {
            var command = new Command(Commands.EnterSdkMode);
            Assert.AreEqual(Commands.EnterSdkMode, command.Rule.Command);
            Assert.IsTrue(command.Immediate);
            Assert.IsNull(command.Arguments);

            command = new Command(Commands.Forward, 20);
            Assert.AreEqual(Commands.Forward, command.Rule.Command);
            Assert.IsFalse(command.Immediate);
            Assert.IsNotNull(command.Arguments);
            Assert.AreEqual(1, command.Arguments.Length);

            command = new Command(Commands.Flip, (char)(CardinalDirection)CardinalDirections.Back);
            Assert.AreEqual(Commands.Flip, command.Rule.Command);
            Assert.IsFalse(command.Immediate);
            Assert.IsNotNull(command.Arguments);
            Assert.AreEqual(1, command.Arguments.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new Command(Commands.Forward));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Command(Commands.Forward, 5));

            Assert.ThrowsException<ArgumentException>(() => new Command(Commands.Forward, 20, 20));
        }
    }
}
