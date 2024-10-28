using System;

namespace ResumeBuilderApp
{
	public class WorkExperience : Section
	{
        private string?  _company, _jobTitle, _duration;

        public string? Company 
        { 
            get => this._company; 
            set => this._company = value; 
        }

        public string? JobTitle 
        { 
            get => this._jobTitle;     
            set => _jobTitle = value; 
        }

        public string? Duration 
        { 
            get => this._duration;  
            set => this._duration = value; 
        }

        public override void CollectData()
        {
            Console.WriteLine("Enter Company:");
            Company = Console.ReadLine();
            Console.WriteLine("Enter Job Title:");
            JobTitle = Console.ReadLine();
            Console.WriteLine("Enter Duration:");
            Duration = Console.ReadLine();
        }

        public override string ToString() =>
            $"Company: {Company}\nJob Title: {JobTitle}\nDuration: {Duration}\n";
    }
}
