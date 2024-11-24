using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeBuilderApp
{
    public enum ResumeCategory
    {
        Engineering,
        BPO,
        Medical,
    }

    public class CategorySelector
    {
        public ResumeCategory SelectCategory()
        {
            
            start:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to the Resume Builder!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n==============================\n"); Console.ResetColor(); Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please choose a category for your resume:");
            Console.WriteLine("1. Engineering");
            Console.WriteLine("2. BPO");
            Console.WriteLine("3. Medical");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==============================\n"); Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter the number of your choice: ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou did not enter anything. Try again\n");
                goto start;
            }
            switch (input)
            {
                case "1":
                    return ResumeCategory.Engineering;
                case "2":
                    return ResumeCategory.BPO;
                case "3":
                    return ResumeCategory.Medical;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid selection. Please try again.");
                    goto start;
            }
        }
    }
}
