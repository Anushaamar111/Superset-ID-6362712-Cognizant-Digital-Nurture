using System;

public class FinancialForecaster
{
    
    public static double PredictFutureValueRecursive(double initialValue, double growthRate, int years)
    {
        if (years == 0)
            return initialValue;

        return PredictFutureValueRecursive(initialValue, growthRate, years - 1) * (1 + growthRate);
    }

   
    public static double PredictFutureValueIterative(double initialValue, double growthRate, int years)
    {
        double result = initialValue;
        for (int i = 0; i < years; i++)
        {
            result *= (1 + growthRate);
        }
        return result;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
      
        double initialAmount = 10000;   
        double annualGrowth = 0.08;      
        int years = 5;

        Console.WriteLine("📊 Financial Forecasting Tool");

        double recursiveValue = FinancialForecaster.PredictFutureValueRecursive(initialAmount, annualGrowth, years);
        Console.WriteLine($"\n🔁 Recursive Forecast:");
        Console.WriteLine($"Predicted future value after {years} years: ₹{recursiveValue:F2}");

      
        double iterativeValue = FinancialForecaster.PredictFutureValueIterative(initialAmount, annualGrowth, years);
        Console.WriteLine($"\n🔂 Iterative Forecast:");
        Console.WriteLine($"Predicted future value after {years} years: ₹{iterativeValue:F2}");

      
        Console.WriteLine($"\n✅ Values Match: {Math.Abs(recursiveValue - iterativeValue) < 0.01}");
    }
}
