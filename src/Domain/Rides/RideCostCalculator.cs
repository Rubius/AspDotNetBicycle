using Domain.Entities;
using Domain.Enums;

namespace Domain.Rides
{
    internal class RideCostCalculator
    {
        private const double MaxDistanceByHour = 10;
        private const double PricePerHour = 7;
        private const double ExtraMileFine = 8;

        private readonly IDictionary<BicycleModelClass, double> CoefficientsByBicycleClass = new Dictionary<BicycleModelClass, double>()
        {
           { BicycleModelClass.A, 1 },
           { BicycleModelClass.B, 1.01 },
           { BicycleModelClass.C, 1.05 },
           { BicycleModelClass.D, 1.1 }
        };

        private const long UsersDistancesSumForDiscount = 200_000;
        private const double Discount = 0.05;

        public double Calculate(long usersDistancesSum, Ride ride)
        {
            var sum = 0D;

            sum += AddForTime(ride);
            sum += AddForDistance(ride);
            sum *= AddForBicycleModelClass(ride);
            sum *= DecreaseForUsersDistancesSum(usersDistancesSum);

            return sum;
        }

        private static double AddForTime(Ride ride)
        {
            var hours = ride.RideHours();

            return hours * PricePerHour;
        }

        private static double AddForDistance(Ride ride)
        {
            var hours = ride.RideHours();

            var extraMiles = ride.Distance!.Value - hours * MaxDistanceByHour;
            if (extraMiles > 0)
            {
                return extraMiles * ExtraMileFine;
            }

            return 0;
        }

        private double AddForBicycleModelClass(Ride ride)
        {
            var model = ride.Bicycle?.Model;
            if (model is null)
            {
                throw new Exception("Can`t calculate ride sum");
            }

            return CoefficientsByBicycleClass.ContainsKey(model.Class) 
                ? CoefficientsByBicycleClass[model.Class] 
                : 1;
         }

        private static double DecreaseForUsersDistancesSum(long usersDistancesSum)
        {
            if (usersDistancesSum > UsersDistancesSumForDiscount)
            {
                return 1 - Discount;
            }

            return 1;
        }
    }
}
