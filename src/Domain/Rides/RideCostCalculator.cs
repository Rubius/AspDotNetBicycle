using Domain.Entities;
using Domain.Enums;

namespace Domain.Rides
{
    internal class RideCostCalculator
    {
        private const double MaxDistanceByHour = 10;
        private const double PricePerHour = 7;
        private const double ExtraMileFine = 8;

        private readonly IDictionary<BicycleBrandClass, double> CoefficientsByBicycleClass = new Dictionary<BicycleBrandClass, double>()
        {
           { BicycleBrandClass.A, 1 },
           { BicycleBrandClass.B, 1.01 },
           { BicycleBrandClass.C, 1.05 },
           { BicycleBrandClass.D, 1.1 }
        };

        private const long UsersDistancesSumForDiscount = 200_000;
        private const double Discount = 0.05;

        public double Calculate(long usersDistancesSum, Ride ride)
        {
            var sum = 0D;

            sum += AddForTime(ride);
            sum += AddForDistance(ride);
            sum *= AddForBicycleBrandClass(ride);
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

        private double AddForBicycleBrandClass(Ride ride)
        {
            var brand = ride.Bicycle?.Brand;
            if (brand is null)
            {
                throw new Exception("Can`t calculate ride sum");
            }

            return CoefficientsByBicycleClass.ContainsKey(brand.Class) 
                ? CoefficientsByBicycleClass[brand.Class] 
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
