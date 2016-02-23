using System;

namespace TestDataGenerator
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Console.WriteLine(new TimeSpan().ToString());

            if (args.Length > 0)
            {
                ConsoleProcess.Run(args);
            }

            // TODO 以后添加可视化窗体支持
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormMain());
        }
    }
}