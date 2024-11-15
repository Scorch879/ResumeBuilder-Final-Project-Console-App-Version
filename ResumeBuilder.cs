using iText.Bouncycastle.Crypto;
using System;

namespace ResumeBuilderApp
{
	public class ResumeBuilder
	{
        private Resume _resume = new Resume(); //instantiate Resume class privately
       
        protected string? title = "\nWelcome to the Resume Builder\n";
        protected string? choice;
        protected string currentUser = ""; //store logged in user temporarily
        protected bool authStatus = false;
        protected string? LoggedIn;

        public void Start()
        {
            
            while (true)
            {
            Start:
                Console.ForegroundColor = ConsoleColor.Blue; // Set text color to green
                Console.WriteLine(title);
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n1. Login");
                Console.WriteLine("\n2. Register");
                Console.WriteLine("\n3. Exit");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Choice: ");
                choice = Console.ReadLine(); Console.ResetColor();

                switch(choice)
                {
                    case "1":
                        authStatus = Login();
                        if (authStatus == true) //log in the user
                        {
                            if (LoggedIn != "admin")
                                MainMenu(); //Go to main menu
                        }
                        break;
                    case "2":
                        Console.Clear();
                        Register();
                        goto Start;
                    case "3":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red; 
                        Console.WriteLine("Exiting program..."); Console.ResetColor();
                        return;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInvalid option. Please choose again.\n"); Console.ResetColor();
                        goto Start;
                }
            }
        }

        protected bool Login()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Login Required\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Type ('forgot') if you have forgotten your password\n");
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write("Enter Username: ");
            FileHandler.Username = Console.ReadLine();

            Console.Write("Enter Password: ");
            FileHandler.Password = Console.ReadLine();

            Console.Clear();
            if (FileHandler.Password == "forgot")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\neAccount Recovery | Password Change Request to Admin\n");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("==============================================================================");

                FileHandler.RequestPassChange(FileHandler.Username, FileHandler.Password);
                Console.Write("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
                return false;
            }
             
            LoggedIn = FileHandler.Login(FileHandler.Username, FileHandler.Password);

            AdminMenu admin = new AdminMenu();


            if (LoggedIn != null)
            {
                currentUser = FileHandler.Username;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nLogin successful. Welcome, {currentUser}!");
                Console.WriteLine($"Account Role: {LoggedIn}");
                Console.ResetColor();

                // Direct admin users to the admin menu
                if (LoggedIn == "admin")
                {
                    AdminMenu adminMenu = new AdminMenu();
                    adminMenu.ShowAdminMenu(); // You will define ShowAdminMenu in AdminMenu class
                }
                return true;
            }

            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid credentials. Please try again."); Console.ResetColor();
                return false;
            }
        }

        private void Register()
        {
            register:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Register New Account");

            Console.Write("Enter a new username: ");
            FileHandler.Username = Console.ReadLine();

            Console.Write("Enter a new password: ");
            FileHandler.Password = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("What is your role? (type admin or user): ");
            string? role = Console.ReadLine();
            Console.ResetColor();

            if (role?.ToLower() != "admin" && role?.ToLower() != "user")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid input. Try again please!\n"); Console.ResetColor();
                goto register;
            }
            
            
            bool isRegistered = FileHandler.RegisterUser(FileHandler.Username, FileHandler.Password, role.ToLower());
            if (isRegistered)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nRegistration successful! You can now log in.\n"); Console.ResetColor();
                return;
            } 
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Username already exists. Please try a different username. and try again"); Console.ResetColor();
                goto register;
            }
                
        }

        private void MainMenu()
        {
        begin:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\t\tMain Menu");
            Console.WriteLine("=============================================================================="); Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("1. Build Resume");
            Console.WriteLine("2. Edit Existing Resume (Must be generated by this program before)");
            Console.WriteLine("3. Logout");
            Console.WriteLine("4. Exit");

            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================================================================="); Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nChoice: ");
            choice = Console.ReadLine(); Console.ResetColor();

            switch (choice)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    _BuildNewResume();
                    Console.ResetColor();
                    break;
                case "2":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    _EditExistingResume();
                    Console.ResetColor();
                    goto begin;
                case "3":
                    Console.Clear();
                    currentUser = "";
                    authStatus = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nLogging out......"); Console.ResetColor();
                    return;
                case "4":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nExiting Resume Builder..."); Console.ResetColor();
                    break;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Please choose again."); Console.ResetColor();
                    goto begin;
            }
        }

       protected void _BuildNewResume() //For generating a new resume from scratch
        {
            Console.Clear();
            Console.WriteLine("Collecting Resume Data....Yipee\n");

            _resume = new Resume();
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                // Collect data for each section
                Console.WriteLine(title);
                Console.WriteLine("==============================");
                Console.WriteLine("     PERSONAL INFORMATION");
                Console.WriteLine("==============================\n");
                _resume.PersonalInfo.CollectData();
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(title);
                Console.WriteLine("==============================");
                Console.WriteLine("       WORK EXPERIENCE");
                Console.WriteLine("==============================\n");
                _resume.WorkExperience.CollectData();
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(title);
                Console.WriteLine("==============================");
                Console.WriteLine("          EDUCATION");
                Console.WriteLine("==============================\n");
                _resume.Education.CollectData();
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(title);
                Console.WriteLine("==============================");
                Console.WriteLine("           SKILLS");
                Console.WriteLine("==============================\n");
                _resume.Skills.CollectData();
                Console.Clear();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError caught: " + ex.Message);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            _ExportAndEdit();
        }

       protected void _EditExistingResume() //editing existing resume
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"User: {currentUser}");
            Console.WriteLine("Loading existing resume....\n"); Console.ForegroundColor = ConsoleColor.Yellow;

            //Retrieve list of resume files for the user
            List<string> resumeFiles = FileHandler.GetUserResumes(currentUser);

            if (resumeFiles.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nResumes for {currentUser}:\n"); Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (string resume in resumeFiles)
                {
                    Console.WriteLine($"- {resume}");
                }
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            _resume = FileHandler.LoadFromTxtFile(currentUser);

            if (_resume == null) //checks if the FileHandler returns null or not
            {Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nCould not load resume.Returning to main menu....\n"); Console.ResetColor();
                return;
            }

            Console.ReadKey();
            Console.WriteLine();

            _ExportAndEdit();
        }

       protected void _ExportAndEdit()
        {
            while(true)
            { 
                Console.Clear();
                Console.WriteLine(title);

                Console.WriteLine("|Current Resume Data|");

            choices:
                _DisplayResume(); // Display the collected information
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Edit a section");
                Console.WriteLine("2. Export Current Generated Resume");
                Console.WriteLine("3. Exit");
                Console.Write("Choice: ");
                choice = Console.ReadLine();
                Console.ResetColor();
                switch (choice)
                {
                    case "1":
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            _EditSection();
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nError caught: " + ex.Message); Console.ResetColor(); continue; 
                        }
                        goto choices;
                    case "2":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(title); Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        FileHandler.SaveResume(_resume, currentUser);
                        Console.ResetColor();
                        return;
                    case "3":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nExiting Resume Builder....."); Console.ResetColor();
                        return;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Please choose again."); Console.ResetColor();
                        goto choices;

                }
            }
        }

       protected void _EditSection()
        {
            Console.Clear();

        editChoices:
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nWhich section would you like to edit?");
            Console.WriteLine("1. Personal Information");
            Console.WriteLine("2. Work Experience");
            Console.WriteLine("3. Education");
            Console.WriteLine("4. Skills");
            Console.Write("Choice: ");
            string? editChoice = Console.ReadLine();
            Console.ResetColor();

            Console.Clear();
            switch (editChoice)
            {
                case "1":
                    Console.WriteLine("\nEditing Personal Information\n");
                    _resume.PersonalInfo.CollectData();
                    break;
                case "2":
                    Console.WriteLine("\nEditing Work Experience\n");
                    _resume.WorkExperience.CollectData();
                    break;
                case "3":
                    Console.WriteLine("\nEditing Education\n");
                    _resume.Education.CollectData();
                    break;
                case "4":
                    Console.WriteLine("\nEditing Skills\n");
                    _resume.Skills.CollectData();
                    break;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Please choose again."); Console.ResetColor();
                    goto editChoices;
            }
        }

       protected void _DisplayResume()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nGenerated Resume:\n");

            Console.WriteLine(_resume.PersonalInfo);
            Console.WriteLine(_resume.WorkExperience);
            Console.WriteLine(_resume.Education);
            Console.WriteLine(_resume.Skills);
            Console.ResetColor();
        }
	}
}