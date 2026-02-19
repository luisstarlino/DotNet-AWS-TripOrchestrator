using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;
using System.Collections.Generic;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Amazon.CDK.Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // ==========================================
            // 1. LAMBAS DEFINITION
            // ==========================================
            // Note.: In the real system, we point to the compiled .zip file
            // ------ here, we are poiting to the source code folders to local deployment.

            var flightCode = Code.FromAsset("../src/TripFlow.FlightService/bin/Release/net8.0");
            var hotelCode = Code.FromAsset("../src/TripFlow.HotelService/bin/Release/net8.0");
            var carCode = Code.FromAsset("../src/TripFlow.CarService/bin/Release/net8.0");

            var reserveFlightLambda = CreateLambda(this, "ReserveFlight", flightCode, "TripFlow.FlightService::TripFlow.FlightService.Function::ReserveFlight");
            var cancelFlightLambda = CreateLambda(this, "CancelFlight", flightCode, "TripFlow.FlightService::TripFlow.FlightService.Function::CancelFlight");

            var reserveHotelLambda = CreateLambda(this, "ReserveHotel", hotelCode, "TripFlow.HotelService::TripFlow.HotelService.Function::ReserveHotel");
            var cancelHotelLambda = CreateLambda(this, "CancelHotel", hotelCode, "TripFlow.HotelService::TripFlow.HotelService.Function::CancelHotel");

            var reserveCarLambda = CreateLambda(this, "ReserveCar", carCode, "TripFlow.CarService::TripFlow.CarService.Function::ReserveCar");
            var cancelCarLambda = CreateLambda(this, "CancelCar", carCode, "TripFlow.CarService::TripFlow.CarService.Function::CancelCar");


            // ==========================================
            // 2. STEP FUNCTIONS - TAKS DEFINITIONS
            // ==========================================

        }


        private Function CreateLambda(Amazon.CDK.Construct scope, string id, AssetCode code, string handler)
        {
            return new Function(scope, id, new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = code,
                Handler = handler,
                MemorySize = 256,
                Timeout = Duration.Seconds(10)
            });
        }
    }
}
