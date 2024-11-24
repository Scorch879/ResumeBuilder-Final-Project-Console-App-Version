using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;

namespace ResumeBuilderApp
{
    public static class FileHandler
    {
        private static string? username, password;
        private static string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string baseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Resume Text Files Data");
        private static string userDataFolder = Path.Combine(baseFolder, "UserData");
        private static string usersFolder = Path.Combine(baseFolder, "Users");
        private static string userData = Path.Combine(userDataFolder, "UserData.txt");

        public static string? Username
        {
            get => username;
            set => username = value;
        }

        public static string? Password
        {
            get => password;
            set => password = value;
        }

        static FileHandler()
        {
            //Ensures these 2 directories exists
            if (!Directory.Exists(userDataFolder))
            {
                Directory.CreateDirectory(userDataFolder);
            }

            if (!Directory.Exists(usersFolder))
            {
                Directory.CreateDirectory(usersFolder);
            }
        }

        //Register users
        public static bool RegisterUser(string username, string password, string role)
        {
            //Check if it exists
            if (UserExists(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nUser: {username} already exists!\n"); Console.ResetColor();
                return false;
            }

            //Create user folder
            string userFolder = Path.Combine(usersFolder, username);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            using (StreamWriter sw = new StreamWriter(userData, true))
            {
                sw.WriteLine($"{username}:{password}:{role}:");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nUser registered successfully!!\n"); Console.ResetColor();
            return true;
        }

        public static string? Login(string username, string password)
        {
            if (!File.Exists(userData)) return null;

            string[] lines = null;

            // Ensure file is not in use by another process
            try
            {
                lines = File.ReadAllLines(userData); // Read all lines to allow modifications
            }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The file is currently in use. Please try again later.");
                Console.ResetColor();
                return null;
            }

            // Loop through each line in the file to find the username and check the password
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(':');

                // Check if the username matches and the password is verified
                if (parts[0] == username && VerifyPassword(password, parts[1]))
                {
                    // Check if the password has been updated (flag check)
                    if (parts.Length > 3 && parts[3] == "password_updated")
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nMessage from Admin: Your password has been recently updated.\n");
                        Console.ResetColor();

                        // Clear the "password_updated" flag
                        parts[3] = "";

                        // Update the line with the cleared flag
                        lines[i] = string.Join(":", parts);

                        try
                        {
                            File.WriteAllLines(userData, lines); // Save the updated lines to the file
                        }
                        catch (IOException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error saving changes to the file. Please try again later.");
                            Console.ResetColor();
                            return null;
                        }

                        return parts[2]; // Return the role, either "admin" or "user"
                    }

                    // Check if the password change is pending approval
                    if (parts.Length > 3 && parts[3] == "password_requested")
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nMessage from Admin: Your password change request is pending approval.\n");
                        Console.WriteLine("Please wait...");
                        Console.ResetColor();
                        Thread.Sleep(1600);
                        return null; // User cannot log in if their password change is pending approval
                    }

                    // Handle password rejection case
                    if (parts.Length > 3 && parts[3] == "password_rejected")
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYour password change request was rejected by the admin.\n");
                        Console.WriteLine("Please wait...");
                        Console.ResetColor();
                        Thread.Sleep(1600);

                        // Clear the rejection flag
                        parts[3] = "";
                        lines[i] = string.Join(":", parts);

                        try
                        {
                            File.WriteAllLines(userData, lines); // Save the updated lines
                        }
                        catch (IOException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error saving changes to the file. Please try again later.");
                            Console.ResetColor();
                            return null;
                        }

                        return null; // Deny login due to rejection
                    }

                    // If the password was updated or rejected, continue the login process
                    return parts[2]; // Return the user role ("admin" or "user")
                }
            }

            // If no matching username and password combination found, return null
            return null; // login failed
        }

        public static void DeleteFile(string username)
        {
            Console.Clear();
            //Retrieve list of resume files for the user
        response:
            List<string> resumeFiles = FileHandler.GetUserResumes(username);

            if (resumeFiles.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nResumes for {username}:"); Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (string resume in resumeFiles)
                {
                    Console.WriteLine($"- {resume}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo resumes found for the user.\n");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nEnter File to be delete (Filename w/o Extension or type exit to return): ");
            string? fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid filename\n");
                goto response;
            }

            if (fileName.ToLower() == "exit")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Returning to menu....");
                return;
            }

            // Add file extension to the filename
            string fileWithExtension = fileName + ".txt";

            // Get the user’s resume folder path
            string userResumesFolder = Path.Combine(usersFolder, username);

            // Construct the full path of the file to delete
            string filePath = Path.Combine(userResumesFolder, fileWithExtension);

            if (File.Exists(filePath))
            {
                try
                {
                    // Delete the file
                    File.Delete(filePath);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nResume {fileWithExtension} deleted successfully.\n");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error deleting the file: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File does not exist. Please try again");
                Console.ResetColor();
                goto response;
            }
        }

        //Retrieve all user resumes 
        public static List<string> GetUserResumes(string username)
        {
            List<string> resumeFiles = new List<string>();
            string userFolder = Path.Combine(usersFolder, username);

            if (Directory.Exists(userFolder))
            {
                // Search for txt files in the user's specific folder using wildcard search
                foreach (var filePath in Directory.GetFiles(userFolder, "*.txt"))
                {
                    resumeFiles.Add(Path.GetFileName(filePath)); // Add file name to the list
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nUser {username} does not have a resume folder.\n"); Console.ResetColor();
            }

            return resumeFiles;
        }

        //Save resume to user's folder
        public static void SaveResume(Resume resume, string username)
        {
        FileName:
            bool status = false;

            string userFolder = Path.Combine(usersFolder, username);
            if (!Directory.Exists(userFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nUser {username} does not have a resume folder.\n"); Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Please enter filename of your resume (No file extension needed): ");
            string? fileName = Console.ReadLine();
            Console.ResetColor();

            if (fileName == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please don't leave the filename empty. Try again.\n"); Console.ResetColor();
                goto FileName;
            }

            string pdfPath = Path.Combine(documentsFolder, fileName + ".pdf");
            string txtPath = Path.Combine(userFolder, fileName + ".txt");

            if (File.Exists(txtPath) || File.Exists(pdfPath))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("File Name already taken. Try again.\n"); Console.ResetColor();
                goto FileName;
            }
            try
            {
                // Saving methods to pdf and to text and includes different types of resume
                if (resume is EngineeringResume Engineering)
                {
                    Engineering.SaveToPdf(pdfPath);
                    Engineering.SaveToTxt(txtPath);
                    status = true;
                }
                else if (resume is BPOResume BPO)
                {
                    BPO.SaveToPdf(pdfPath);
                    BPO.SaveToTxt(txtPath);
                    status = true;
                }
                else if (resume is MedicalResume Medical)
                {
                    Medical.SaveToPdf(pdfPath);
                    Medical.SaveToTxt(txtPath);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while saving the resume: {ex.Message}");
                Console.ResetColor();
            }

            if (status)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Resume saved for user " + username); Console.ResetColor();
                Thread.Sleep(1600);
                Console.Clear();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nResume failed to save for user " + username); Console.ResetColor();
                Thread.Sleep(1600);
                Console.Clear();
            }
             

        }

        //Verify the password
        private static bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            string hashOfInput = inputPassword;
            return hashOfInput == storedPasswordHash;
        }

        //Checks if the user already exists
        private static bool UserExists(string username)
        {
            if (!File.Exists(userData)) return false;

            using (StreamReader reader = new StreamReader(userData))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(':');
                    if (parts[0] == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Searching method
        public static Resume LoadFromTxtFile(string username)
        {
        start:
            //User enters the file name
            Console.Write("\nEnter the file name to load (without file extension): ");
            string? fileName = Console.ReadLine()?.Trim();

            if (fileName == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No file name entered. Try Again Please"); Console.ResetColor();
                goto start;
            }

            // Get the Documents path and prompt the user for the file name
            string folderPath = Path.Combine(usersFolder, username); //gets the path of the folder 

            string filePathTxt = Path.Combine(folderPath, fileName + ".txt"); //txt file going to the folder "Resume Text Files Data" in MyDocuments

            Console.WriteLine($"Trying to load resume file: {fileName}");

            //checks if it exists
            if (!File.Exists(filePathTxt))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File not found: {fileName}"); Console.ResetColor();
                return null;
            }

            Resume resume;
            using (StreamReader reader = new StreamReader(filePathTxt))
            {
                string? resumeType = reader.ReadLine()?.Split(": ")[1];
                switch (resumeType)
                {
                    case "Engineering":
                        resume = new EngineeringResume();
                        resume = EngineeringResume.LoadFromTxt(filePathTxt);
                        break;
                    case "BPO":
                        resume = new BPOResume();
                        resume = BPOResume.LoadFromTxt(filePathTxt);
                        break;
                    case "Medical":
                        resume = new MedicalResume();
                        resume = MedicalResume.LoadFromTxt(filePathTxt);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown resume type.");
                        Console.ResetColor();
                        return null;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nResume Loaded Successfully\n");
            Console.Write("Press any key to continue"); Console.ResetColor(); Console.ForegroundColor = ConsoleColor.Cyan;
            return resume;
        }

        public static string typeResume(string username)
        {
            GetUserResumes(username);
        start:
            //User enters the file name
            Console.Write("\nEnter the file name to load (without file extension): ");
            string? fileName = Console.ReadLine()?.Trim();

            if (fileName == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No file name entered. Try Again Please"); Console.ResetColor();
                goto start;
            }

            string? resumeType;
            // Get the Documents path and prompt the user for the file name
            string folderPath = Path.Combine(usersFolder, username); //gets the path of the folder 

            string filePathTxt = Path.Combine(folderPath, fileName + ".txt"); //txt file going to the folder "Resume Text Files Data" in MyDocume

            using (StreamReader reader = new StreamReader(filePathTxt))
            {
                resumeType = reader.ReadLine()?.Split(": ")[1];
            }

            if (resumeType != null)
                return resumeType;
            else
                throw new Exception("Unvalidated Resume");
        }

        //User requests password change
        public static void RequestPassChange(string username, string newPassword)
        {
            if (!File.Exists(userData))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No user data found."); Console.ResetColor();
                return;
            }

            Console.Write("Please enter your desired new password: ");
            string? desiredNewPassword = Console.ReadLine();

            var lines = new List<string>(File.ReadLines(userData));
            bool userFound = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split(":");

                // Ensure the line has the expected number of elements (at least 4)
                if ((parts.Length == 3 || parts.Length == 4) && parts[0] == username)
                {
                    parts[1] = desiredNewPassword;        // Temporarily store new password

                    // Set the 4th element (the flag) to "password_requested" to mark the request
                    if (parts.Length == 3)
                    {
                        lines[i] = string.Join(":", parts) + ":password_requested"; // Add flag if it's missing
                    }
                    else
                    {
                        parts[3] = "password_requested";  // Update existing flag
                        lines[i] = string.Join(":", parts);
                    }

                    File.WriteAllLines(userData, lines);  // Save changes
                   
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Password change requested for user: {username}. Your new password will be applied after admin approval.");
                    Console.ResetColor();
                    userFound = true;
                    break;
                }
            }

            if (!userFound)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Username not found. Please check your input."); Console.ResetColor();
            }
        }

        //User deletes resume
       
        /// 
        /// Admin Privileges Below as well as User Request if Password has been forgotten
        ///

        //Admin Privilege (Changing user password)
        public static void ChangePasswordUser(string username, string? newPassword)
        {
            if (!File.Exists(userData))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No user data found."); Console.ResetColor();
                return;
            }

            var lines = new List<string>(File.ReadLines(userData));

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split(":");
                if (parts[0] == username && parts.Length > 3 && parts[3] == "password_requested")
                {
                    // Prompt user for new password
                    Console.Write("Please enter your new password: ");
                    newPassword = Console.ReadLine();

                    // Update password and set flag to "password_updated"
                    parts[1] = newPassword;              // Update password
                    parts[3] = "password_updated";       // Set flag for one-time notification
                    lines[i] = string.Join(":", parts);  // Update the line

                    File.WriteAllLines(userData, lines); // Save changes
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Password successfully changed for user: {username}");
                    Console.ResetColor();
                    break;
                }
            }
            return;
        }

        //Admin Privilege (Admin Approval)
        public static void AdminApprovePasswordChanges()
        {
            if (!File.Exists(userData))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No user data found."); Console.ResetColor();
                return;
            }

            var lines = File.ReadAllLines(userData);
            bool hasPendingChanges = false;

            // Display users with pending password change requests
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nUsers with pending password change requests:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;

            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(':');
                if (parts.Length > 3 && parts[3] == "password_requested")
                {
                    hasPendingChanges = true;
                    Console.WriteLine($"User: {parts[0]} has requested a password change.");
                }
            }

            if (!hasPendingChanges)
            {
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.WriteLine("No pending password change requests."); Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("==============================================================================\n"); Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================================================\n");
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Allow admin to approve or reject
            Console.Write("\nEnter the username to approve/reject the password change request: ");
            string usernameToApprove = Console.ReadLine();

            bool userFound = false;

            for (int i = 0; i < lines.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var parts = lines[i].Split(':');
                if (parts[0] == usernameToApprove && parts.Length > 3 && parts[3] == "password_requested")
                {
                    approval:
                    userFound = true;
                    Console.WriteLine($"\nPending request for {usernameToApprove}.");

                    Console.Write("\nApprove this request? (y/n): ");
                    string approvalResponse = Console.ReadLine();

                    if (approvalResponse.ToLower() == "y")
                    {
                        // Approve password change: Clear the "password_requested" flag
                        parts[3] = "password_updated"; // Clear the password updated flag
                        lines[i] = string.Join(":", parts); // Update the line

                        File.WriteAllLines(userData, lines); // Save the changes

                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPassword change request for {usernameToApprove} has been approved.");
                        Console.ResetColor();
                    }
                    else if (approvalResponse.ToLower() == "n")
                    {
                        // Reject password change: Set the 'password_rejected' flag
                        parts[3] = "password_rejected"; // Set the password rejected flag
                        lines[i] = string.Join(":", parts); // Update the line

                        File.WriteAllLines(userData, lines); // Save the changes

                        // Reject password change:flag it has password_rejected
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nPassword change request for {usernameToApprove} has been rejected.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Invalid Input!"); Console.ResetColor();
                        goto approval;
                    }
                    break;
                }
            }

            if (!userFound)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User not found or no pending request for that user.");
                Console.ResetColor();
            }
        }

        //Admin Privilege (Delete User and their associated resume folders
        public static bool DeleteUser()
        {
            if (!File.Exists(userData))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No user data found."); Console.ResetColor();
                return false;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nAccount Control | Deletion of Accounts\n");
            Console.WriteLine("==============================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;

            var lines = File.ReadAllLines(userData);
            Console.WriteLine("Existing Users:\n");
            foreach (var line in lines)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var parts = line.Split(':');
                if (parts.Length > 0)
                    Console.WriteLine($"- {parts[0]}"); // Display the username (first element)
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================================================\n");
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Please enter the user to be deleted: ");
            username = Console.ReadLine();

          
            var userList = new List<string>(File.ReadAllLines(userData));
            bool userFound = false; //presume not found

            for (int i = 0; i < userList.Count; i++)
            {
                if (userList[i].StartsWith(username + ":"))
                {
                    userList.RemoveAt(i);
                    userFound = true;
                    break;
                }
            }


            if (userFound) //if it exists
            {
                File.WriteAllLines(userData, userList);
                string userFolder = Path.Combine(usersFolder, username);

                if (Directory.Exists(userFolder))
                {
                    Directory.Delete(userFolder, true);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"User: {username} has been deleted!\n"); Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Press any key to continue");
                    Console.ReadKey();
                    Console.Clear();
                    return true;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nUser {username} was not found...\n"); Console.ResetColor();
            return false;
        }

        //Formatting for other classes to use with pdf saving
        //Method to add the line breaker like a really long line like in MS Word
        public static void AddLine(Document document)
        {
            LineSeparator line = new LineSeparator(new SolidLine(1)); //Thickness is set to 1 
            line.SetWidth(UnitValue.CreatePercentValue(100)); //It needs to be a Percent value so that the line will stretch from margin to margin
            line.SetFontColor(ColorConstants.GRAY); //Sets the color of the line to Gray or near to black at least
            document.Add(line);
        }
    }

}