using System;
using System.Collections.Generic;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class CircularBuffer<T>
    {
        private T[] _buffer { get; set; }
        public int Size => _buffer.Length;

        public int Count;
        public IEnumerable<T> Buffer => _buffer;

        public CircularBuffer(int size)
        {
            _buffer = new T[size];
            Count = 0;
        }

        public void Insert(T[] array)
        {
            var sizeOfArray = array.Length;
            var overflow = Math.Max(0, Count + sizeOfArray - Size);

            ShiftLeftAndPadWithZeroes(overflow);
            Count -= overflow;
            InsertAtEnd(array);
        }
        private void InsertAtEnd(T[] smallArray)
        {
            int arrayLength = smallArray.Length;

            if (arrayLength > Size)
                throw new ArgumentException("The smaller array cannot be larger than the larger array.");

            if((Count + arrayLength) > Size)
                throw new ArgumentException("The smaller array cannot be larger than the larger array.");

            for(var i = 0; i < arrayLength; i++)
                _buffer[Count++] = smallArray[i];
        }

        private void ShiftLeftAndPadWithZeroes(int shiftAmount)
        {
            for (var i = shiftAmount; i < Size; i++)
            {
                _buffer[i - shiftAmount] = _buffer[i];
            }

        }
    }
}
