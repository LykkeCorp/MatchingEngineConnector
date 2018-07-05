using System;
using System.Collections.Generic;
using System.Threading;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Me;

namespace Lykke.MatchingEngine.Connector.Services
{
    [Obsolete("Do not use it. Use overloads without ISocketLog")]
    public class SocketLogInMemory : ISocketLog
    {
        private readonly int _maxItems;
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        public SocketLogInMemory(int maxItems = 100)
        {
            _maxItems = maxItems;
        }

        private readonly List<SocketLogItem> _items = new List<SocketLogItem>();

        public void Add(string message)
        {


            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                _items.Add(new SocketLogItem { DateTime = DateTime.UtcNow, Message = message });

                while (_items.Count > _maxItems)
                    _items.RemoveAt(0);

                Count++;

            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }

        }

        public IEnumerable<SocketLogItem> GetItems()
        {
            _readerWriterLockSlim.EnterReadLock();
            try
            {
                return _items.ToArray();
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }

        }

        public int Count { get; private set; }

    }
}