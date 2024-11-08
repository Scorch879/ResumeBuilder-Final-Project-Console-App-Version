using System;

namespace ResumeBuilderApp
{
	public class ResumeBuilder
	{
        private Resume _resume = new Resume(); //instantiate Resume class privately

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("\nWelcome to the Resume Builder\n");

                try
                {
                    // Collect data for each section
                    _resume.PersonalInfo.CollectData();
                    _resume.WorkExperience.CollectData();
                    _resume.Education.CollectData();
                    _resume.Skills.CollectData();
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("\nError caught: " + ex.Message); continue;
                }

                Console.Clear();

                choices:
                _DisplayResume(); // Display the collected information
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Edit a section");
                Console.WriteLine("2. Export Current Generated Resume");
                Console.WriteLine("3. Exit");
                Console.Write("Choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        try
                        {
                            _EditSection();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nError caught: " + ex.Message); continue;
                        }
                        goto choices;
                    case "2":
                        FileHandler.SaveToFile(_resume);
                        break;
                    case "3":
                        Console.WriteLine("\n Exiting Resume Builder.....");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        goto choices;

                }

                response:
                Console.Write("Would you like to build another resume? (y/n): ");
                choice = Console.ReadLine();
                if (choice == "y")
                {
                    Console.Clear();
                    Console.WriteLine("Restarting Program.....");
                    continue;
                }
                else if (choice == "n")
                {
                    Console.WriteLine("Thanks for using my program!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Try again!");
                    goto response;
                }
             }
        }
       

        private void _EditSection()
        {
            Console.Clear();

            editChoices:
            Console.WriteLine("\nWhich section would you like to edit?");
            Console.WriteLine("1. Personal Information");
            Console.WriteLine("2. Work Experience");
            Console.WriteLine("3. Education");
            Console.WriteLine("4. Skills");
            Console.Write("Choice: ");
            string? editChoice = Console.ReadLine();

            switch (editChoice)
            {
                case "1":
                    _resume.PersonalInfo.CollectData();
                    break;
                case "2":
                    _resume.WorkExperience.CollectData();
                    break;
                case "3":
                    _resume.Education.CollectData();
                    break;
                case "4":
                    _resume.Skills.CollectData();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    goto editChoices;
            }
        }

        private void _DisplayResume()
        {
            Console.WriteLine("\nGenerated Resume:");

            Console.WriteLine(_resume.PersonalInfo);
            Console.WriteLine(_resume.WorkExperience);
            Console.WriteLine(_resume.Education);
            Console.WriteLine(_resume.Skills);
        }
	}
}