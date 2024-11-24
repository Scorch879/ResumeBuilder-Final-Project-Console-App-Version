using System;

namespace ResumeBuilderApp
{ 
    public class PersonalInfo : Section
    {
        private string? _name, _address, _email, _phoneNumber, _description;

        public string?Name 
        { 
            get => this._name; 
            set => this._name = value; 
        }

        public string? Email 
        { 
            get => this._email; 
            set => this._email = value; 
        }

        public string? PhoneNumber 
        { 
            get => this._phoneNumber; 
            set => this._phoneNumber = value; 
        }

        public string? Description
        {
            get => _description;
            set => this._description = value;
        }

        public string? Address
        {
            get => _address;
            set => this._address = value;
        }

        public override void CollectData()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==============================");
            Console.WriteLine("     PERSONAL INFORMATION");
            Console.WriteLine("==============================\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Full Name: ");
            Name = GetInput("Name");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Address: ");
            Address = GetInput("Address");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Email: ");
            Email = GetInput("Email");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter Phone Number: ");
            PhoneNumber = GetInput("Email");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Write a Small description about yourself: ");
            Description = GetInput("Description");
        }

        public override string ToString() =>
            $"Name: {Name}\nEmail: {Email}\nPhone: {PhoneNumber}\nDescription: {Description}\n";
    }
}