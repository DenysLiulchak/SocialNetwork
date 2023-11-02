using System.Text;

namespace SocialNetwork
    {
    class Program
    {
        public static void Main(string[] args)
    {
            Encoding UTF8 = Encoding.UTF8;
            SocialNetwork socialNetwork = new SocialNetwork();

            Console.OutputEncoding = UTF8;
            Console.InputEncoding = UTF8;

            socialNetwork.Run();
        }
    }
}