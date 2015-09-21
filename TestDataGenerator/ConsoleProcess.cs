namespace TestDataGenerator
{
    public class ConsoleProcess
    {
        public static void Run(string[] args)
        {
            foreach (var configFile in args)
            {
                var config = Configuration.Parse(configFile);

                var generator = new Generator(config);
                generator.Generate();
            }
        }
    }
}