using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;


namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // ==========================================
            // 1. LAMBAS DEFINITION
            // ==========================================
            // Note.: In the real system, we point to the compiled .zip file
            // ------ here, we are poiting to the source code folders to local deployment.

            var flightCode = Code.FromAsset("../src/TripFlow.FlightService/bin/Release/net10.0");
            var hotelCode = Code.FromAsset("../src/TripFlow.HotelService/bin/Release/net10.0");
            var carCode = Code.FromAsset("../src/TripFlow.CarService/bin/Release/net10.0");

            var reserveFlightLambda = CreateLambda(this, "ReserveFlight", flightCode, "TripFlow.FlightService::TripFlow.FlightService.Function::ReserveFlight");
            var cancelFlightLambda = CreateLambda(this, "CancelFlight", flightCode, "TripFlow.FlightService::TripFlow.FlightService.Function::CancelFlight");

            var reserveHotelLambda = CreateLambda(this, "ReserveHotel", hotelCode, "TripFlow.HotelService::TripFlow.HotelService.Function::ReserveHotel");
            var cancelHotelLambda = CreateLambda(this, "CancelHotel", hotelCode, "TripFlow.HotelService::TripFlow.HotelService.Function::CancelHotel");

            var reserveCarLambda = CreateLambda(this, "ReserveCar", carCode, "TripFlow.CarService::TripFlow.CarService.Function::ReserveCar");
            var cancelCarLambda = CreateLambda(this, "CancelCar", carCode, "TripFlow.CarService::TripFlow.CarService.Function::CancelCar");


            // ==========================================
            // 2. STEP FUNCTIONS - TAKS DEFINITIONS
            // ==========================================
            // PayloadResponseOnly = true garante que o JSON limpo do nosso TripState continue fluindo sem metadados extras da AWS

            var reserveFlightTask = new LambdaInvoke(this, "1. Reserve Flight", new LambdaInvokeProps { LambdaFunction = reserveFlightLambda, PayloadResponseOnly = true });
            var cancelFlightTask = new LambdaInvoke(this, "Undo Flight", new LambdaInvokeProps { LambdaFunction = cancelFlightLambda, PayloadResponseOnly = true });

            var reserveHotelTask = new LambdaInvoke(this, "2. Reserve Hotel", new LambdaInvokeProps { LambdaFunction = reserveHotelLambda, PayloadResponseOnly = true });
            var cancelHotelTask = new LambdaInvoke(this, "Undo Hotel", new LambdaInvokeProps { LambdaFunction = cancelHotelLambda, PayloadResponseOnly = true });

            var reserveCarTask = new LambdaInvoke(this, "3. Reserve Car", new LambdaInvokeProps { LambdaFunction = reserveCarLambda, PayloadResponseOnly = true });
            var cancelCarTask = new LambdaInvoke(this, "Undo Car", new LambdaInvokeProps { LambdaFunction = cancelCarLambda, PayloadResponseOnly = true });

            // Estados finais
            var tripSucceed = new Succeed(this, "Trip Confirmed! 🎉");
            var tripFailed = new Fail(this, "Trip Cancelled ❌", new FailProps { Error = "SagaFailed", Cause = "Transaction rolled back due to a failure in one of the steps." });

            // ==========================================
            // 3. SAGA CORE: ROLLBACK AND ORQUESTRATION
            // ==========================================

            // If car fails -> Cancel the hotel -> Cancel fligth and them Fail SAGA
            cancelHotelTask.Next(cancelFlightTask);
            cancelFlightTask.Next(tripFailed);

            reserveCarTask.AddCatch(cancelHotelTask, new CatchProps
            {
                Errors = new[] { "States.ALL" },
                ResultPath = "$.ErrorDetails" // Guarda o erro sem sobrescrever nosso TripState
            });
            reserveCarTask.Next(tripSucceed); // Se não falhar, sucesso!

            // --- If Hotel Fails -> Cancel fligth and them, Fail SAGA!
            reserveHotelTask.AddCatch(cancelFlightTask, new CatchProps
            {
                Errors = new[] { "States.ALL" },
                ResultPath = "$.ErrorDetails"
            });
            reserveHotelTask.Next(reserveCarTask);

            // --- If Fligth fails -> FAILS SAGA
            reserveFlightTask.AddCatch(tripFailed, new CatchProps
            {
                Errors = new[] { "States.ALL" },
                ResultPath = "$.ErrorDetails"
            });

            reserveFlightTask.Next(reserveHotelTask);

            // Monta a Máquina de Estados começando pela Reserva do Voo
            var sagaStateMachine = new StateMachine(this, "TripFlowSagaOrchestrator", new StateMachineProps
            {
                DefinitionBody = DefinitionBody.FromChainable(reserveFlightTask),
                Timeout = Duration.Minutes(5)
            });
        }


        private Function CreateLambda(Construct scope, string id, AssetCode code, string handler)
        {
            return new Function(scope, id, new FunctionProps
            {
                Runtime = new Runtime("dotnet10", RuntimeFamily.DOTNET_CORE),
                Code = code,
                Handler = handler,
                MemorySize = 256,
                Timeout = Duration.Seconds(10)
            });
        }
    }
}
