using Moyboy;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            await Console.Out.WriteAsync("Ссылка на видео: ");
            await RuTubeRequest.GetURLs(Console.ReadLine());
        }
        else
        {
            await RuTubeRequest.GetURLs(args[0]);
        }

        Console.ReadLine();
    }
}