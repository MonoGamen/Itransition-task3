using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void CheckArgs(string[] args)
        {
            static void Exit(string information)
            { 
                Console.WriteLine(information);
                Console.WriteLine("Example: rock paper scissors");
                Environment.Exit(0);
            }

            if (args.Length < 3)
                Exit("The number of arguments must be more than 2");
            else if (args.Length != args.Distinct().Count())
                Exit("Arguments must be unique");
            else if (args.Length % 2 == 0)
                Exit("The number of arguments must be odd");
        }

        static int ChooseMove(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nAvailable moves:");
                for (int i = 0; i < args.Length; i++)
                    Console.WriteLine($"{i+1} - {args[i]}");
                Console.WriteLine("0 - exit");

                Console.WriteLine("Enter your move:");
                int userMove;
                if (int.TryParse(Console.ReadLine(), out userMove))
                {
                    if (userMove == 0)
                        Environment.Exit(0);

                    userMove--;
                    if (userMove < 0 || userMove >= args.Length)
                        continue;

                    return userMove;
                }
                else
                    continue;
            }
        }
        enum Result
        {
            DeadHeat, Win, Lose
        }

        static Result CheckResult(int user, int comp, int length)
        {
            if (user == comp)
                return Result.DeadHeat;

            bool isMaxWin = Math.Abs(user - comp) <= Math.Floor((double)(length / 2));
            if (user > comp)
                return isMaxWin ? Result.Win : Result.Lose;
            else 
                return isMaxWin ? Result.Lose : Result.Win;
        }

        static void Main(string[] args)
        {
            CheckArgs(args);

            var keyBytes = new byte[16];
            RandomNumberGenerator.Fill(new Span<byte>(keyBytes));
            string hmackey = BitConverter.ToString(keyBytes).Replace("-", null);

            int compMove = RandomNumberGenerator.GetInt32(0, args.Length);

            var hmac = SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(args[compMove] + hmackey));
            string hmacString = BitConverter.ToString(hmac).Replace("-", null);
            Console.WriteLine($"HMAC: {hmacString}");

            int userMove = ChooseMove(args);
            Console.WriteLine($"Your move: {args[userMove]}");
            Console.WriteLine($"Computer move: {args[compMove]}");

            switch (CheckResult(userMove, compMove, args.Length))
            {
                case Result.DeadHeat:
                    Console.WriteLine("Dead heat!");
                    break;
                case Result.Win:
                    Console.WriteLine("You win!");
                    break;
                case Result.Lose:
                    Console.WriteLine("You lose!");
                    break;
            }

            Console.WriteLine($"HMAC key: {hmackey}");
        }
    }
}
