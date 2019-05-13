using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tello.State;

namespace Messenger.Test
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void Vector_move()
        {
            var position = new Vector();
            Assert.AreEqual(0, position.Heading);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);

            position = position.Move(Tello.CardinalDirections.Front, 100);
            Assert.AreEqual(0, position.Heading);
            Assert.AreEqual(100, position.X);
            Assert.AreEqual(0, position.Y);

            position = position.Turn(Tello.ClockDirections.Clockwise, 90);
            Assert.AreEqual(90, position.Heading);
            Assert.AreEqual(100, position.X);
            Assert.AreEqual(0, position.Y);

            position = position.Move(Tello.CardinalDirections.Front, 100);
            Assert.AreEqual(90, position.Heading);
            Assert.AreEqual(100, position.X);
            Assert.AreEqual(100, position.Y);

            position = position.Turn(Tello.ClockDirections.Clockwise, 90);
            Assert.AreEqual(180, position.Heading);
            Assert.AreEqual(100, position.X);
            Assert.AreEqual(100, position.Y);

            position = position.Move(Tello.CardinalDirections.Front, 100);
            Assert.AreEqual(180, position.Heading);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(100, position.Y);

            position = position.Turn(Tello.ClockDirections.Clockwise, 90);
            Assert.AreEqual(270, position.Heading);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(100, position.Y);

            position = position.Move(Tello.CardinalDirections.Front, 100);
            Assert.AreEqual(270, position.Heading);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
        }
    }
}
