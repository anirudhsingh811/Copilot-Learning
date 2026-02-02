using System;
using System.Runtime.InteropServices;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("=================================");
Console.WriteLine("Hello from .NET!");
Console.WriteLine("=================================");
Console.WriteLine();

// Display runtime information
Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
Console.WriteLine($"Architecture: {RuntimeInformation.OSArchitecture}");
Console.WriteLine($".NET Version: {Environment.Version}");
Console.WriteLine($"Machine Name: {Environment.MachineName}");
Console.WriteLine($"User Name: {Environment.UserName}");
Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
Console.WriteLine();

// Check if running in WSL
bool isWSL = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && 
             File.Exists("/proc/version") && 
             File.ReadAllText("/proc/version").Contains("microsoft", StringComparison.OrdinalIgnoreCase);

Console.WriteLine($"Running in WSL: {(isWSL ? "YES ✓" : "NO")}");
Console.WriteLine();

Console.WriteLine("Program executed successfully!");
Console.WriteLine("=================================");
