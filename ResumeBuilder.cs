using System;

namespace ResumeBuilderApp
{
	public class ResumeBuilder
	{
        private readonly Resume _resume = new Resume();

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

                // Display the collected information
                DisplayResume();

                Console.Clear();

                choices:
                Console.WriteLine("What would you like to do?");
                Console.Write("1. Edit a section");
                Console.Write("2. Export Current Generated Resume");
                Console.Write("3. Exit");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        try
                        {
                            EditSection();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nError caught: " + ex.Message); continue;
                        }
                        break;
                    case "2":
                        FileHandler.SaveToFile(_resume);
                        break;
                    case "3":
                        Console.WriteLine("\n Exiting Resume Builder.....");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        goto choices;
                }
            }
        }

        private void EditSection()
        {
            Console.Clear();

            Console.WriteLine("\nWhich section would you like to edit?");
            Console.WriteLine("1. Personal Information");
            Console.WriteLine("2. Work Experience");
            Console.WriteLine("3. Education");
            Console.WriteLine("4. Skills");
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
                    break;
            }
        }

        private void DisplayResume()
        {
            Console.WriteLine("\nGenerated Resume:");

            Console.WriteLine(_resume.PersonalInfo);
            Console.WriteLine(_resume.WorkExperience);
            Console.WriteLine(_resume.Education);
            Console.WriteLine(_resume.Skills);
        }
	}
}