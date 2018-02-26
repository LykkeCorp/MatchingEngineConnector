using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Lykke.MatchingEngine.Connector.Helpers
{
    internal static class TelemetryHelper
    {
        private static readonly TelemetryClient _telemetry = new TelemetryClient();

        private const string _telemetryType = "Me Connector";

        internal static IOperationHolder<DependencyTelemetry> InitTelemetryOperation(
            string target,
            string name,
            string data)
        {
            var operation = _telemetry.StartOperation<DependencyTelemetry>(name);
            operation.Telemetry.Type = _telemetryType;
            operation.Telemetry.Target = target;
            operation.Telemetry.Name = name;
            operation.Telemetry.Data = data;

            return operation;
        }

        internal static void SubmitException(Exception e)
        {
            _telemetry.TrackException(e);
        }

        internal static void SubmitOperationFail(IOperationHolder<DependencyTelemetry> telemtryOperation)
        {
            telemtryOperation.Telemetry.Success = false;
        }

        internal static void SubmitOperationResult(IOperationHolder<DependencyTelemetry> telemtryOperation)
        {
            _telemetry.StopOperation(telemtryOperation);
        }
    }
}
