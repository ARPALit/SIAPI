// See https://aka.ms/new-console-template for more information
using Arpal.SiApi.ConsoleApp;

Console.WriteLine("ARPAL Test!");

var test = new OracleTest();
await test.ExecReadAsync();
