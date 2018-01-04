namespace OpenCl.NOPenCL
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            var manager = new Manager();
            manager.Execute();

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
