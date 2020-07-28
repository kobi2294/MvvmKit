using ConsoleDemo.Properties;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Diffplex
{
    public static class Main
    {
        public static void Run()
        {
            var txt1 = Resources.oldState;

            var txt2 = Resources.newState;

            var diff = InlineDiffBuilder.Diff(txt1, txt2);
            var diff2 = SideBySideDiffBuilder.Diff(txt1, txt2);
            var savedColor = Console.ForegroundColor;

            _printLines(diff2.OldText.Lines);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------------------------");
            _printLines(diff2.NewText.Lines);

            Console.ForegroundColor = savedColor;
        }

        private static void _printLines(IEnumerable<DiffPiece> lines)
        {
            foreach (var line in lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{line.Position} + ");
                        break;
                    case ChangeType.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{line.Position} - ");
                        break;
                    case ChangeType.Imaginary:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{line.Position} i ");
                        break;
                    case ChangeType.Modified:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{line.Position} * ");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray; // compromise for dark or light background
                        Console.Write($"{line.Position}   ");
                        break;
                }

                Console.WriteLine(line.Text);
            }
        }

    }
}
