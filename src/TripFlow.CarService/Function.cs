using Amazon.Lambda.Core;
using TripFlow.Shared;
using TripFlow.Shared.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TripFlow.CarService;

public class Function
{
    public TripState ReserveCar(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Car] Processing reservation for Trip: {state.TripId}");

        // 1. CHAOS
        if (state.ChaosTarget != null && state.ChaosTarget.Equals("Car", StringComparison.OrdinalIgnoreCase))
        {
            context.Logger.LogError("[Car] 🔥 CHAOS INJECTED! Failing car rental.");
            throw new Exception("Simulated Failure in Car Service");
        }

        // 2. Business Logic
        state.Car.ReservationId = "CAR-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        state.Car.Status = BookingStatus.Confirmed;
        state.Car.Price = 120.00m;

        context.Logger.LogInformation($"[Car] Reserved successfully. ID: {state.Car.ReservationId}");

        return state;
    }

    public TripState CancelCar(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Car] ↩️ Compensating transaction for Trip: {state.TripId}");

        if (state.Car.Status == BookingStatus.Confirmed)
        {
            state.Car.Status = BookingStatus.Cancelled;
            context.Logger.LogInformation($"[Car] Reservation {state.Car.ReservationId} cancelled.");
        }

        return state;
    }
}
