using Amazon.Lambda.Core;
using TripFlow.Shared;
using TripFlow.Shared.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TripFlow.FlightService;

public class Function
{
    public TripState ReserveFlight(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Flight] Processing reservation for Trip: {state.TripId}");

        // 1. CHAOS ENGINEERING: Fake chaos (If request)
        if(state.ChaosTarget != null && state.ChaosTarget.Equals("Flight", StringComparison.OrdinalIgnoreCase))
        {
            context.Logger.LogError("[Flight] 🔥 CHAOS INJECTED! Failing flight reservation.");
            throw new Exception("Simulated Failure in Flight Service");
        }

        // 2. Normal Reservetion Logic!
        state.Flight.ReservationId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        state.Flight.Status = BookingStatus.Confirmed;
        state.Flight.Price = 450.00m;

        context.Logger.LogInformation($"[Flight] Reserved successfully. ID: {state.Flight.ReservationId}");

        return state; // send to the next stage
    }

    public TripState CancelFlight(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Flight] ↩️ Compensating transaction (Rollback) for Trip: {state.TripId}");

        if (state.Flight.Status == BookingStatus.Confirmed)
        {
            state.Flight.Status = BookingStatus.Cancelled;
            state.Flight.ErrorMessage = "Booking cancelled due to failure in subsequent steps.";
            context.Logger.LogInformation($"[Flight] Reservation {state.Flight.ReservationId} cancelled.");
        }
        else
        {
            context.Logger.LogInformation("[Flight] Nothing to cancel (was not confirmed).");
        }

        return state;
    }


}
