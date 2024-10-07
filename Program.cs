using System;
using System.Collections.Generic;
using System.IO;

public class BankRunTotals
{
    public string BankID { get; set; }
    public string Date { get; set; }
    public string RunName { get; set; }
    public string Pattern { get; set; }
    public int ItemCount { get; set; }
    public int Pat101TotalItemCount { get; set; }
    public int Pat102TotalItemCount { get; set; }
    public int Pat103TotalItemCount { get; set; }
    public int Pat104TotalItemCount { get; set; }
    public int Pat105TotalItemCount { get; set; }
    public int Pat106TotalItemCount { get; set; }
    public int Pat107TotalItemCount { get; set; }
    public int Pat108TotalItemCount { get; set; }
    public int Pat109TotalItemCount { get; set; }
    public int Pat201TotalItemCount { get; set; }
    public int Pat202TotalItemCount { get; set; }
    public int Pat203TotalItemCount { get; set; }
    public int Pat204TotalItemCount { get; set; }
    public int Pat205TotalItemCount { get; set; }
    public int Pat206TotalItemCount { get; set; }
    public int Pat207TotalItemCount { get; set; }
    public int Pat208TotalItemCount { get; set; }
    public int Pat209TotalItemCount { get; set; }
    public string Time { get; set; }
    public int TotalINCRuns { get; set; }
    public int TotalPODRuns { get; set; }
    public int TotalINCItems { get; set; }
    public int TotalPODItems { get; set; }
}
public class BankData
{
    public string BankID { get; set; }
    public Dictionary<string, int> PatternTotals { get; set; }

    public BankData() => PatternTotals = new Dictionary<string, int>();
}

public class Program
{
    static BankRunTotals prevRun;
    // Dictionary to hold total items per day: Date -> Total Items
    static Dictionary<string, int> dailyTotalItems = new Dictionary<string, int>();

    // Nested dictionary to hold pattern-specific items per day: Date -> (Pattern -> Total Items)
    static Dictionary<string, Dictionary<string, int>> dailyPatternTotalItems = new Dictionary<string, Dictionary<string, int>>();

    public static void ProcessIpMetricsReport(string[] reportLines)
    {
        BankRunTotals currentDayTotals = null;
        BankRunTotals prevDayTotals = null;

        foreach (var line in reportLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the line into an array based on spaces and tabs
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6) continue;  // Skip if the line doesn't have enough parts

            var currentBank = parts[2];
            var currentDate = parts[1];
            var currentPattern = parts[4];
            var currentRun = parts[0];
            var currentTime = parts[5];
            var currentItemCount = 1; // Assuming each line represents one item

            // Skip lines where BankID doesn't start with 'C'
            if (!currentRun.StartsWith("C"))
            {
                Console.WriteLine($"Skipping line with BankID: {currentRun}");
                continue;
            }

            // Initialize the current day's totals if we are processing a new date
            if (prevDayTotals == null || prevDayTotals.Date != currentDate)
            {
                if (prevDayTotals != null)
                {
                    // Write previous day totals to CSV when switching to a new date
                    WriteRunTotalsToCSV(prevDayTotals);
                }

                // Create a new instance for the current day's totals
                currentDayTotals = new BankRunTotals
                {
                    BankID = currentBank,
                    Date = currentDate,
                    Pat101TotalItemCount = 0,
                    Pat102TotalItemCount = 0,
                    Pat103TotalItemCount = 0,
                    Pat104TotalItemCount = 0,
                    Pat105TotalItemCount = 0,
                    Pat106TotalItemCount = 0,
                    Pat107TotalItemCount = 0,
                    Pat108TotalItemCount = 0,
                    Pat109TotalItemCount = 0,
                    Pat201TotalItemCount = 0,
                    Pat202TotalItemCount = 0,
                    Pat203TotalItemCount = 0,
                    Pat204TotalItemCount = 0,
                    Pat205TotalItemCount = 0,
                    Pat206TotalItemCount = 0,
                    Pat207TotalItemCount = 0,
                    Pat208TotalItemCount = 0,
                    Pat209TotalItemCount = 0,
                    TotalPODItems = 0,
                    TotalPODRuns = 0,
                    TotalINCItems = 0,
                    TotalINCRuns = 0
                };

                prevDayTotals = currentDayTotals;
            }

            // Update the appropriate pattern totals
            if (currentPattern == "0101") currentDayTotals.Pat101TotalItemCount++;
            if (currentPattern == "0102") currentDayTotals.Pat102TotalItemCount++;
            if (currentPattern == "0103") currentDayTotals.Pat103TotalItemCount++;
            if (currentPattern == "0104") currentDayTotals.Pat104TotalItemCount++;
            if (currentPattern == "0105") currentDayTotals.Pat105TotalItemCount++;
            if (currentPattern == "0106") currentDayTotals.Pat106TotalItemCount++;
            if (currentPattern == "0107") currentDayTotals.Pat107TotalItemCount++;
            if (currentPattern == "0108") currentDayTotals.Pat108TotalItemCount++;
            if (currentPattern == "0109") currentDayTotals.Pat109TotalItemCount++;

            if (currentPattern == "0201") currentDayTotals.Pat201TotalItemCount++;
            if (currentPattern == "0202") currentDayTotals.Pat202TotalItemCount++;
            if (currentPattern == "0203") currentDayTotals.Pat203TotalItemCount++;
            if (currentPattern == "0204") currentDayTotals.Pat204TotalItemCount++;
            if (currentPattern == "0205") currentDayTotals.Pat205TotalItemCount++;
            if (currentPattern == "0206") currentDayTotals.Pat206TotalItemCount++;
            if (currentPattern == "0207") currentDayTotals.Pat207TotalItemCount++;
            if (currentPattern == "0208") currentDayTotals.Pat208TotalItemCount++;
            if (currentPattern == "0209") currentDayTotals.Pat209TotalItemCount++;

            // Count the POD and INC items
            if (currentPattern.StartsWith("02"))
            {
                currentDayTotals.TotalPODItems++;

                if (prevDayTotals.Pattern != currentPattern)
                    currentDayTotals.TotalPODRuns++;
            }
            else if (currentPattern.StartsWith("01"))
            {
                currentDayTotals.TotalINCItems++;

                if (prevDayTotals.Pattern != currentPattern)
                    currentDayTotals.TotalINCRuns++;
            }

            // Save current pattern for comparison
            prevDayTotals.Pattern = currentPattern;
        }

        // Ensure the last day's totals are written after processing
        if (prevDayTotals != null)
        {
            WriteRunTotalsToCSV(prevDayTotals);
        }
    }


    public static void WriteRunTotalsToCSV(BankRunTotals runTotals)
    {
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat101TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat102TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat103TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat104TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat105TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat106TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat107TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat108TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat109TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat201TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat202TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat203TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat204TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat205TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat206TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat207TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat208TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat209TotalItemCount} items");

        using (StreamWriter runWriter = new StreamWriter(@"C:\Users\Ehulen\Documents\RunsCSV.csv", true))
        {
            runWriter.WriteLine($"Bank ID, Date, Pattern, RunName, Time, TotalItems");
            runWriter.WriteLine($"{runTotals.BankID}, {runTotals.Date}, {runTotals.Pattern},{runTotals.RunName}, {runTotals.Time}, {runTotals.TotalPODItems + runTotals.TotalINCItems}");
        }
    }

    // New Method to Count Items by Day
    public static void CountItemsByDay(string[] reportLines)
    {
        BankRunTotals currentDayTotals = null;
        BankRunTotals prevDayTotals = null;

        foreach (var line in reportLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the line into an array based on spaces and tabs
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6) continue;  // Skip if the line doesn't have enough parts

            var currentBank = parts[2];
            var currentDate = parts[1];
            var currentPattern = parts[4];
            var currentRun = parts[0];
            var currentTime = parts[5];
            var currentItemCount = 1; // Assuming each line represents one item

            // Skip lines where BankID doesn't start with 'C'
            if (!currentRun.StartsWith("C"))
            {
                Console.WriteLine($"Skipping line with BankID: {currentRun}");
                continue;
            }

            // Initialize the current day's totals if we are processing a new date
            if (prevDayTotals == null || prevDayTotals.Date != currentDate)
            {
                if (prevDayTotals != null)
                {
                    // Write previous day totals to CSV when switching to a new date
                    WriteDailyPatternCountsToCSV(prevDayTotals);
                }

                // Create a new instance for the current day's totals
                currentDayTotals = new BankRunTotals
                {
                    BankID = currentBank,
                    Date = currentDate,
                    Pat101TotalItemCount = 0,
                    Pat102TotalItemCount = 0,
                    Pat103TotalItemCount = 0,
                    Pat104TotalItemCount = 0,
                    Pat105TotalItemCount = 0,
                    Pat106TotalItemCount = 0,
                    Pat107TotalItemCount = 0,
                    Pat108TotalItemCount = 0,
                    Pat109TotalItemCount = 0,
                    Pat201TotalItemCount = 0,
                    Pat202TotalItemCount = 0,
                    Pat203TotalItemCount = 0,
                    Pat204TotalItemCount = 0,
                    Pat205TotalItemCount = 0,
                    Pat206TotalItemCount = 0,
                    Pat207TotalItemCount = 0,
                    Pat208TotalItemCount = 0,
                    Pat209TotalItemCount = 0,
                    TotalPODItems = 0,
                    TotalPODRuns = 0,
                    TotalINCItems = 0,
                    TotalINCRuns = 0
                };

                prevDayTotals = currentDayTotals;
            }

            // Update the appropriate pattern totals
            if (currentPattern == "0101") currentDayTotals.Pat101TotalItemCount++;
            if (currentPattern == "0102") currentDayTotals.Pat102TotalItemCount++;
            if (currentPattern == "0103") currentDayTotals.Pat103TotalItemCount++;
            if (currentPattern == "0104") currentDayTotals.Pat104TotalItemCount++;
            if (currentPattern == "0105") currentDayTotals.Pat105TotalItemCount++;
            if (currentPattern == "0106") currentDayTotals.Pat106TotalItemCount++;
            if (currentPattern == "0107") currentDayTotals.Pat107TotalItemCount++;
            if (currentPattern == "0108") currentDayTotals.Pat108TotalItemCount++;
            if (currentPattern == "0109") currentDayTotals.Pat109TotalItemCount++;

            if (currentPattern == "0201") currentDayTotals.Pat201TotalItemCount++;
            if (currentPattern == "0202") currentDayTotals.Pat202TotalItemCount++;
            if (currentPattern == "0203") currentDayTotals.Pat203TotalItemCount++;
            if (currentPattern == "0204") currentDayTotals.Pat204TotalItemCount++;
            if (currentPattern == "0205") currentDayTotals.Pat205TotalItemCount++;
            if (currentPattern == "0206") currentDayTotals.Pat206TotalItemCount++;
            if (currentPattern == "0207") currentDayTotals.Pat207TotalItemCount++;
            if (currentPattern == "0208") currentDayTotals.Pat208TotalItemCount++;
            if (currentPattern == "0209") currentDayTotals.Pat209TotalItemCount++;

            // Count the POD and INC items
            if (currentPattern.StartsWith("02"))
            {
                currentDayTotals.TotalPODItems++;

                if (prevDayTotals.Pattern != currentPattern)
                    currentDayTotals.TotalPODRuns++;
            }
            else if (currentPattern.StartsWith("01"))
            {
                currentDayTotals.TotalINCItems++;

                if (prevDayTotals.Pattern != currentPattern)
                    currentDayTotals.TotalINCRuns++;
            }

            // Save current pattern for comparison
            prevDayTotals.Pattern = currentPattern;
        }

        // Ensure the last day's totals are written after processing
        if (prevDayTotals != null)
        {
            WriteDailyPatternCountsToCSV(prevDayTotals);
        }
    }

    // Optional: Method to Write Daily Pattern Counts
    public static void WriteDailyPatternCountsToCSV(BankRunTotals runTotals)
    {
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat101TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat102TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat103TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat104TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat105TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat106TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat107TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat108TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat109TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat201TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat202TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat203TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat204TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat205TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat206TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat207TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat208TotalItemCount} items");
        Console.WriteLine($"Writing Run to CSV: {runTotals.RunName} - {runTotals.Pat209TotalItemCount} items");

        try
        {
            // Writing daily totals to second file
            using (StreamWriter dailyWriter = new StreamWriter(@"C:\Users\Ehulen\Documents\DailyTotalsCSV.csv", true))
            {
                Console.WriteLine($"Writing daily totals to CSV for {runTotals.BankID} on {runTotals.Date}");

                dailyWriter.WriteLine($"Bank ID, Date, PodRunCount, INCRunCount, Pat101RunCount,Pat102RunCount,Pat103RunCount,Pat104RunCount,Pat105RunCount,Pat106RunCount,Pat107RunCount,Pat108RunCount,Pat109RunCount,Pat201RunCount,Pat202RunCount,Pat203RunCount,Pat204RunCount,Pat205RunCount,Pat206RunCount,Pat207RunCount,Pat208RunCount,Pat209RunCount");

                dailyWriter.WriteLine($"{runTotals.BankID}, {runTotals.Date}, {runTotals.TotalPODItems}, {runTotals.TotalINCItems},{runTotals.Pat101TotalItemCount}, " +
                    $"{runTotals.Pat102TotalItemCount}, {runTotals.Pat103TotalItemCount}, {runTotals.Pat104TotalItemCount}, " +
                    $"{runTotals.Pat105TotalItemCount}, {runTotals.Pat106TotalItemCount},{runTotals.Pat107TotalItemCount}, {runTotals.Pat108TotalItemCount}, " +
                    $"{runTotals.Pat109TotalItemCount}, {runTotals.Pat201TotalItemCount}, {runTotals.Pat202TotalItemCount}, {runTotals.Pat203TotalItemCount}, " +
                    $"{runTotals.Pat204TotalItemCount}, {runTotals.Pat205TotalItemCount}, {runTotals.Pat206TotalItemCount}, " +
                    $"{runTotals.Pat207TotalItemCount}, {runTotals.Pat208TotalItemCount}, {runTotals.Pat209TotalItemCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to DailyTotalsCSV: {ex.Message}");
        }
    }

    public static void Main(string[] args)
    {
        // Check if the file path was provided as a command-line argument
        // if (args.Length < 1)
        // {
        //    Console.WriteLine("Please provide the path to the IpMetricsReport file.");
        //    return;
        // }

        // string filePath = args[0];
        var filePath = @"C:\Users\Ehulen\Documents\Metrics.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }

        try
        {
            Console.WriteLine($"Reading file: {filePath}");
            var reportLines = File.ReadAllLines(filePath);

            ProcessIpMetricsReport(reportLines);

            // Count items by day and write to CSV
            CountItemsByDay(reportLines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing the file: {ex.Message}");
        }
    }
}