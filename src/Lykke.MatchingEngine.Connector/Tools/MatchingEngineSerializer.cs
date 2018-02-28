using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Tools
{
    internal sealed class MatchingEngineSerializer : ITcpSerializer
    {
        private const int HeaderSize = 5;

        private static readonly Dictionary<MeDataType, Type> Types = new Dictionary<MeDataType, Type>
        {
            [MeDataType.Ping] = typeof(MePingModel),
            [MeDataType.TheResponse] = typeof(TheResponseModel),
            [MeDataType.UpdateBalance] = typeof(MeCashInOutModel),
            [MeDataType.LimitOrder] = typeof(MeLimitOrderModel),
            [MeDataType.MarketOrderObsolete] = typeof(MeMarketOrderObsoleteModel),
            [MeDataType.LimitOrderCancel] = typeof(MeLimitOrderCancelModel),
            [MeDataType.BalanceUpdate] = typeof(MeUpdateBalanceModel),
            [MeDataType.Transfer] = typeof(MeNewTransferModel),
            [MeDataType.CashInOut] = typeof(MeNewCashInOutModel),
            [MeDataType.Swap] = typeof(MeNewSwapModel),
            [MeDataType.WalletCredentialsReload] = typeof(MeUpdateWalletCredsModel),
            [MeDataType.NewLimitOrder] = typeof(MeNewLimitOrderModel),
            [MeDataType.NewLimitOrderCancel] = typeof(MeNewLimitOrderCancelModel),
            [MeDataType.TheNewResponse] = typeof(TheNewResponseModel),
            [MeDataType.NewMarketOrder] = typeof(MeNewMarketOrderModel),
            [MeDataType.MarketOrderResponse] = typeof(MarketOrderResponseModel),
            [MeDataType.MarketOrder] = typeof(MeMarketOrderModel),
            [MeDataType.UpdateBalanceNew] = typeof(MeNewUpdateBalanceModel),
            [MeDataType.NewMultiLimitOrder] = typeof(MeMultiLimitOrderModel),
            [MeDataType.MultiLimitOrderResponse] = typeof(MeMultiLimitOrderResponseModel),
            [MeDataType.MultiLimitOrderCancel] = typeof(MeMultiLimitOrderCancelModel)
        };

        private static readonly Dictionary<Type, MeDataType> TypesReverse = new Dictionary<Type, MeDataType>();
        private static readonly byte[] PingPacket = { (byte)MeDataType.Ping };

        private readonly Dictionary<MeDataType, object> _instancesCache = new Dictionary<MeDataType, object>();

        public MatchingEngineSerializer()
        {
            foreach (var tp in Types)
            {
                _instancesCache.Add(tp.Key, tp.Value.CreateUsingDefaultConstructor());
            }
        }

        static MatchingEngineSerializer()
        {
            foreach (var tp in Types)
            {
                TypesReverse.Add(tp.Value, tp.Key);
            }
        }

        public async Task<Tuple<object, int>> Deserialize(Stream stream)
        {
            var dataType = (MeDataType)await stream.ReadByteFromSocket();
            if (dataType == MeDataType.Ping)
                return new Tuple<object, int>(MePingModel.Instance, 1);

            CheckIfTypeIsSupportedOrThrow(dataType);

            var datalen = await stream.ReadIntFromSocket();
            if (datalen == 0)
            {
                return new Tuple<object, int>(_instancesCache[dataType], HeaderSize);
            }


            var data = await stream.ReadFromSocket(datalen);
            using (var memStream = new MemoryStream(data) { Position = 0 })
            {
                var result = Serializer.NonGeneric.Deserialize(Types[dataType], memStream);
                return new Tuple<object, int>(result, HeaderSize + datalen);
            }
        }

        private static void CheckIfTypeIsSupportedOrThrow(MeDataType dataType)
        {
            if (!Enum.IsDefined(typeof(MeDataType), dataType))
            {
                throw new InvalidOperationException($"Not defined MeDataType {dataType}");
            }
            if (!Types.ContainsKey(dataType))
            {
                throw new InvalidOperationException($"Not supported MeDataType {dataType}");
            }
        }

        public byte[] Serialize(object data)
        {
            if (data is MePingModel)
                return PingPacket;

            var type = TypesReverse[data.GetType()];
            using (var memStream = new MemoryStream())
            {
                memStream.Position = HeaderSize;

                Serializer.Serialize(memStream, data);

                var size = memStream.Length - HeaderSize;

                memStream.Position = 0;

                memStream.WriteByte((byte)type);
                memStream.WriteInt((int)size);

                return memStream.ToArray();
            }

        }
    }
}
