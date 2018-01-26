using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;
using Lykke.MatchingEngine.Connector.Models;
using ProtoBuf;

namespace Lykke.MatchingEngine.Connector.Tools
{
    public class MatchingEngineSerializer : ITcpSerializer
    {

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

        private readonly Dictionary<MeDataType, object> _instancesCache = new Dictionary<MeDataType, object>();

        static MatchingEngineSerializer()
        {
            foreach (var tp in Types)
                TypesReverse.Add(tp.Value, tp.Key);
        }

        public async Task<Tuple<object, int>> Deserialize(Stream stream)
        {
            const int headerSize = 5;

            var dataType = (MeDataType)await stream.ReadByteFromSocket();
            if (dataType == MeDataType.Ping)
                return new Tuple<object, int>(MePingModel.Instance, 1);

            var datalen = await stream.ReadIntFromSocket();
            if (datalen == 0)
            {
                CheckIfTypeIsSupportedOrThrow(dataType);
                lock (_instancesCache)
                {
                    if (!_instancesCache.ContainsKey(dataType))
                        _instancesCache.Add(dataType, Types[dataType].CreateUsingDefaultConstructor());

                    return new Tuple<object, int>(_instancesCache[dataType], headerSize);
                }

            }

            var data = await stream.ReadFromSocket(datalen);
            var memStream = new MemoryStream(data) { Position = 0 };

            CheckIfTypeIsSupportedOrThrow(dataType);
            var result = Serializer.NonGeneric.Deserialize(Types[dataType], memStream);
            return new Tuple<object, int>(result, headerSize + datalen);

        }

        private void CheckIfTypeIsSupportedOrThrow(MeDataType dataType)
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


        private static readonly byte[] PingPacket = { (byte)MeDataType.Ping };

        public byte[] Serialize(object data)
        {
            if (data is MePingModel)
                return PingPacket;


            var type = TypesReverse[data.GetType()];

            var memStream = new MemoryStream();
            Serializer.Serialize(memStream, data);
            var outData = memStream.ToArray();

            var outStream = new MemoryStream();
            outStream.WriteByte((byte)type);
            outStream.WriteInt(outData.Length);
            outStream.Write(outData, 0, outData.Length);

            return outStream.ToArray();
        }
    }
}
