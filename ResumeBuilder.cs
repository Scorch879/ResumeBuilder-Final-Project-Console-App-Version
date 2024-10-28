using System;

namespace ResumeBuilderApp
{
	public class ResumeBuilder
	{
        private readonly Resume _resume = new Resume();

        public void Start()
        {
            Console.WriteLine("\nWelcome to the Resume Builder\n");

            // Collect data for each section
            _resume.PersonalInfo.CollectData();
            _resume.WorkExperience.CollectData();
            _resume.Education.CollectData();
            _resume.Skills.CollectData();

            // Display the collected information
            DisplayResume();
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