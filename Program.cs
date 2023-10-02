namespace Multi_threaded_Processing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new MultithreadedQueueProcessingService<int>();

            service.Worker = (value) => Console.WriteLine(value);

            service.RunThreads();

            for (int i = 0; i < 10000; i++)
            {
                service.AddToQueue(i);
            }

            Console.ReadKey();
        }
    }
}