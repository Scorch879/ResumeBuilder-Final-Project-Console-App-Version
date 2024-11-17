﻿namespace ResumeBuilderApp
{
    class AdminMenu : ResumeBuilder
    {
        public void ShowAdminMenu()
        {
        begin:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==============================================================================");
            Console.WriteLine("\t\t\t        Main Menu");
            Console.WriteLine("=============================================================================="); Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("1. Build Resume");
            Console.WriteLine("2. Edit Existing Resume (Must be generated by this program before)");
            Console.WriteLine("3. Delete Existing Resumes");
            Console.WriteLine("4. Deletion of User Accounts");
            Console.WriteLine("5. Approval for Password Requests");
            Console.WriteLine("6. Logout");

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
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    FileHandler.DeleteFile(currentUser);
                    Console.ResetColor();
                    goto begin;
                case "4":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    FileHandler.DeleteUser();
                    Console.ResetColor();
                    goto begin;
                case "5":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    FileHandler.AdminApprovePasswordChanges();
                    Console.ResetColor();
                    goto begin;
                case "6":
                    Console.Clear();
                    currentUser = "";
                    authStatus = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nLogging out......"); Console.ResetColor();
                    return;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid option. Please choose again.\n"); Console.ResetColor();
                    goto begin;
            }
        }
    }
}
