using System;
using System.Diagnostics;

namespace test_c_sharp
{
    class Program
    {
        static void Main(string[] args)
        {

            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\ProgramData\Anaconda3\python.exe";

            var script = @"C:\dev\FS-BMK\Optimization\module1.py";
            var a = "rtgf";
            string args1 = string.Format("{0} {1} {2}", script, a, a);
            psi.Arguments = args1;

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;

            //var results = OptimizationSuspension.SuspensionFeatureLimits[0];

            using (var process = Process.Start(psi))
            {
                //results = process.StandardOutput.ReadToEnd();
            }

            Console.WriteLine($"args2 \n{args1}");
        }
    }
}
