using AcousticWavePropagationSimulation.DataStructures;

namespace AcousitcWavePropagationTests
{
    [TestClass]
    public class CircularBufferTests
    {
        [TestMethod]
        public void TestInsert()
        {
            var buffer = new CircularBuffer<int>(10);

            buffer.Insert(new int[]{ 1, 2, 3,});
            Assert.IsTrue(buffer.Buffer.SequenceEqual(new int[] { 1, 2, 3, 0, 0, 0, 0, 0, 0, 0 }));
            buffer.Insert(new int[] { 4, 5 });
            Assert.IsTrue(buffer.Buffer.SequenceEqual(new int[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0 }));
            buffer.Insert(new int[] { 6, 7 });
            Assert.IsTrue(buffer.Buffer.SequenceEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 0, 0, 0 }));
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            Assert.IsTrue(buffer.Buffer.SequenceEqual(new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            Assert.IsTrue(buffer.Buffer.SequenceEqual(new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 }));

            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
            buffer.Insert(new int[] { 12, 13, 14, 15, 16, 17, 18 });
            buffer.Insert(new int[] { 8, 9, 10, 11 });
        }
    }
}