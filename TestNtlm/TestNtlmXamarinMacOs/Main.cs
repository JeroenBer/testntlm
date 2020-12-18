using System;
using AppKit;
using NUnitLite;

namespace TestNtlmXamarinMacOs
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            return new AutoRun(typeof(Program).Assembly).Execute(args);

            //Console.WriteLine("HELLO WORLD");

            //Xunit.Runners.
            //NSApplication.Main(args);
        }
    }
}
