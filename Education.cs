using System;


namespace ResumeBuilderApp
{
	public class Education : Section
	{
        private string? _degree, _school, _yearOfGraduation;

        public string? Degree 
        { 
            get => this._degree; 
            set => this._degree = value; 
        }

        public string? School 
        { 
            get => this._school; 
            set => this._school = value; 
        }

        public string? YearOfGraduation 
        { 
            get => this._yearOfGraduation ; 
            set => this._yearOfGraduation = value; 
        }

        public override void CollectData()
        {
            Console.Write("Enter Degree:");
            Degree = GetInput("Degree");

            Console.Write("Enter School:");
            School = GetInput("School");

            Console.Write("Enter Year of Graduation:");
            YearOfGraduation = GetInput("YearOfGraduation");
        }

        public override string ToString() =>
            $"Degree: {Degree}\nSchool: {School}\nYear of Graduation: {YearOfGraduation}\n";
    }
}
