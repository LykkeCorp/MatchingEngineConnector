using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Lykke.MatchingEngine.Connector.Abstractions.Services;

namespace Lykke.MatchingEngine.Connector.Tools
{
    internal class Connections
    {
        private readonly Dictionary<int, TcpConnection> _sockets = new Dictionary<int, TcpConnection>();

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly ISocketLog _log;


        public Connections(ISocketLog log)
        {
            _log = log;
        }

        private void RemoveSocket(TcpConnection connection)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                if (_sockets.ContainsKey(connection.Id))
                    _sockets.Remove(connection.Id);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }

        }

        public void AddSocket(TcpConnection connection)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                connection.DisconnectAction = RemoveSocket;
                _sockets.Add(connection.Id, connection);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
            _log.Add("Socket Accepted. Id=" + connection.Id.ToString(CultureInfo.InvariantCulture));
        }

        public IEnumerable<TcpConnection> AllConnections
        {
            get
            {
                _lockSlim.EnterReadLock();
                try
                {
                    return _sockets.Values.ToArray();
                }
                finally
                {
                    _lockSlim.ExitReadLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _lockSlim.EnterReadLock();
                try
                {
                    return _sockets.Count;
                }
                finally
                {
                    _lockSlim.ExitReadLock();
                }

            }
        }
    }
}