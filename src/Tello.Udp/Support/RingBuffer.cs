using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal class RingBuffer<T>
    {
        internal RingBuffer(int size)
        {
            _buffer = new T[size];
        }

        #region private
        private readonly Gate _gate = new Gate();
        private readonly T[] _buffer;
        private int _head = 0;
        private int _tail = 0;
        #endregion

        #region internal
        internal int Count => _gate.WithReadLock(() => { return UnsafeGetSize(UnsafeGetInverted()); });

        internal bool IsEmpty => _gate.WithReadLock(() => { return UnsafeGetIsEmpty(); });

        internal void Push(T item)
        {
            _gate.WithWriteLock(() =>
            {
                _buffer[_head] = item;
                UnsafeAdvanceHead();
            });
        }

        internal bool TryPop(out T result)
        {
            result = _gate.WithUpgradeableReadLock(() =>
            {
                var value = default(T);
                if (!UnsafeGetIsEmpty())
                {
                    value = _buffer[_tail];
                    _gate.WithWriteLock(() =>
                    {
                        _buffer[_tail] = default(T);
                        UnsafeAdvanceTail();
                    });
                }
                return value;
            });
            return result != null;
        }

        internal bool TryPeek(out T result)
        {
            result = _gate.WithReadLock(() =>
            {
                return !UnsafeGetIsEmpty()
                    ? _buffer[_tail]
                    : default(T);
            });
            return result != null;
        }

        internal T[] Flush()
        {
            return _gate.WithUpgradeableReadLock(() =>
            {
                var result = UnsafeToArray();
                Clear();
                return result;
            });
        }

        internal void Clear()
        {
            _gate.WithWriteLock(() =>
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                _head = 0;
                _tail = 0;
            });
        }

        internal T[] ToArray()
        {
            return _gate.WithReadLock(() =>
            {
                return UnsafeToArray();
            });
        }
        #endregion

        #region unsafe
        private void UnsafeAdvanceHead()
        {
            _head = (_head + 1) % _buffer.Length;
            if (_head == _tail)
            {
                UnsafeAdvanceTail();
            }
        }

        private void UnsafeAdvanceTail()
        {
            _tail = (_tail + 1) % _buffer.Length;
        }

        private bool UnsafeGetIsEmpty()
        {
            return _tail == _head;
        }

        private bool UnsafeGetInverted()
        {
            return _tail > _head;
        }

        private int UnsafeGetSize(bool inverted)
        {
            return inverted
                ? _buffer.Length - _tail + _head
                : _head - _tail;
        }

        private T[] UnsafeToArray()
        {
            if (UnsafeGetIsEmpty())
            {
                return new T[0];
            }

            var inverted = UnsafeGetInverted();
            var size = UnsafeGetSize(inverted);

            var result = new T[size];
            if (inverted)
            {
                var tailToEnd = _buffer.Length - _tail;
                Array.Copy(_buffer, _tail, result, 0, tailToEnd);
                Array.Copy(_buffer, 0, result, tailToEnd, _head);
            }
            else
            {
                Array.Copy(_buffer, _tail, result, 0, size);
            }

            return result;
        }
        #endregion
    }
}
