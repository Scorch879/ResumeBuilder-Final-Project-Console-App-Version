using System;

namespace ResumeBuilderApp
{ 
    public class PersonalInfo : Section
    {
        private string? _name, _email, _phoneNumber;

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

        public override void CollectData()
        {
            Console.Write("Enter Full Name:");
            Name = Console.ReadLine();
            Console.Write("Enter Email:");
            Email = Console.ReadLine();
            Console.Write("Enter Phone Number:");
            PhoneNumber = Console.ReadLine();
        }

        public override string ToString() =>
            $"Name: {Name}\nEmail: {Email}\nPhone: {PhoneNumber}\n";
    }
}
