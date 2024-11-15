using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Canvas;

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

            using (StreamWriter sw = new StreamWriter(userData, true))
            {
                sw.WriteLine($"{username}:{password}:{role}:");
            }

            //Create user folder
            string userFolder = Path.Combine(usersFolder, username);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
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


        //Retrieve all user resumes for Admin?
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
            string userFolder = Path.Combine(usersFolder, username);
            if (!Directory.Exists(userFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nUser {username} does not have a resume folder.\n"); Console.ResetColor();
                return;
            }


        FileName:
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

            //Saving methods to pdf and to text
            new PDF().ExportToPdf(resume, pdfPath);
            new Txt().ExportToTxt(resume, txtPath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Resume saved for user " + username); Console.ResetColor();
            Thread.Sleep(1600);
            Console.Clear();

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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File not found: {fileName}"); Console.ResetColor();
                return null;
            }

            //Store the information retrieved in a new Resume object
            Resume resume = new Resume();

            try
            {
                using (StreamReader reader = new StreamReader(filePathTxt))
                {
                    // Assuming a consistent format, read each line and insert the data to the resume fields
                    reader.ReadLine(); // Skip the "Personal Information" header
                    resume.PersonalInfo.Name = reader.ReadLine()?.Split(": ")[1];
                    resume.PersonalInfo.Email = reader.ReadLine()?.Split(": ")[1];
                    resume.PersonalInfo.PhoneNumber = reader.ReadLine()?.Split(": ")[1];
                    resume.PersonalInfo.Description = reader.ReadLine()?.Split(": ")[1];

                    reader.ReadLine(); //skip blank space

                    reader.ReadLine();//Skip work experience header
                    resume.WorkExperience.Company = reader.ReadLine()?.Split(": ")[1];
                    resume.WorkExperience.JobTitle = reader.ReadLine()?.Split(": ")[1];
                    resume.WorkExperience.Duration = reader.ReadLine()?.Split(": ")[1];

                    reader.ReadLine(); //skip blank space

                    reader.ReadLine(); //Skip education header
                    resume.Education.Degree = reader.ReadLine()?.Split(": ")[1];
                    resume.Education.School = reader.ReadLine()?.Split(": ")[1];
                    resume.Education.YearOfGraduation = reader.ReadLine()?.Split(": ")[1];

                    reader.ReadLine(); //Skip empty line

                    reader.ReadLine(); //skips skills header

                    string? skill;
                    while ((skill = reader.ReadLine()) != null)
                    {
                        resume.Skills.SkillList.Add(skill.TrimStart('-').Trim());
                    }
                }

                Console.WriteLine("\nResume Loaded Successfully\n");
                Console.Write("Press any key to continue");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error loading resume: " + ex.Message); Console.ResetColor();
            }

            return resume;
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
            Console.Write("\n\nEnter the username to approve/reject the password change request: ");
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
                    Console.WriteLine($"Pending request for {usernameToApprove}.");

                    Console.Write("Approve this request? (y/n): ");
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
                        Console.WriteLine($"Password change request for {usernameToApprove} has been rejected.");
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
            Console.WriteLine("==============================================================================\n");
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Please enter the user to be deleted: ");
            username = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================================================\n");
            Console.ForegroundColor = ConsoleColor.Yellow;

            var lines = new List<string>(File.ReadAllLines(userData));
            bool userFound = false; //presume not found

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(username + ":"))
                {
                    lines.RemoveAt(i);
                    userFound = true;
                    break;
                }
            }

            if (userFound) //if it exists
            {
                File.WriteAllLines(userData, lines);
                string userFolder = Path.Combine(usersFolder, username);

                if (Directory.Exists(userFolder))
                {
                    Directory.Delete(userFolder, true);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"User: {username} has been deleted!"); Console.ResetColor();
                }
                return true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nUser {username} was not found...\n"); Console.ResetColor();
            return false;
        }
    }

    public class PDF
    {
        public void ExportToPdf(Resume resume, string filePath)
        {
            try
            {
                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        ///this is from iText library
                        Style noSpaceStyle = new Style().SetMarginTop(0).SetMarginBottom(0); //noSpace style


                        // Placeholder box for image
                        iText.Kernel.Geom.Rectangle placeholderBox = new iText.Kernel.Geom.Rectangle(37, 700, 100, 100); // x, y, width, height
                        PdfCanvas canvas = new PdfCanvas(pdf.AddNewPage());
                        canvas.SetStrokeColor(ColorConstants.BLACK);
                        canvas.SetLineWidth(1);
                        canvas.Rectangle(placeholderBox);
                        canvas.Stroke();

                        document.Add(new Paragraph().SetMarginBottom(100));

                        //PERSONAL INFORMATION
                        document.Add(new Paragraph($"{resume.PersonalInfo.Name}").SetFontSize(20).SetBold().SetMarginTop(10)
                            .AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Email: {resume.PersonalInfo.Email}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Phone: {resume.PersonalInfo.PhoneNumber}").AddStyle(noSpaceStyle));

                        document.Add(new Paragraph());
                        document.Add(new Paragraph());
                        AddLine(document);
                        document.Add(new Paragraph($"{resume.PersonalInfo.Description}"));
                        document.Add(new Paragraph());
                        document.Add(new Paragraph());

                        AddLine(document);
                        document.Add(new Paragraph());

                        // Work Experience
                        document.Add(new Paragraph("WORK EXPERIENCE").SetBold());
                        document.Add(new Paragraph($"Company: {resume.WorkExperience.Company}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Job Title: {resume.WorkExperience.JobTitle}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Duration: {resume.WorkExperience.Duration}").AddStyle(noSpaceStyle));

                        document.Add(new Paragraph());
                        AddLine(document);
                        document.Add(new Paragraph());

                        // Education
                        document.Add(new Paragraph("EDUCATION").SetBold());
                        document.Add(new Paragraph($"Degree: {resume.Education.Degree}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"School: {resume.Education.School}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Year of Graduation: {resume.Education.YearOfGraduation}").AddStyle(noSpaceStyle));

                        document.Add(new Paragraph());
                        AddLine(document);
                        document.Add(new Paragraph());

                        // Skills
                        document.Add(new Paragraph("SKILLS").SetBold());
                        foreach (var skill in resume.Skills.SkillList)
                        {
                            document.Add(new Paragraph($"- {skill}"));
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("PDF resume saved successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError caught: " + ex); Console.ResetColor();
            }
        }

        //Method to add the line breaker like a really long line like in MS Word
        private void AddLine(Document document)
        {
            LineSeparator line = new LineSeparator(new SolidLine(1)); //Thickness is set to 1 
            line.SetWidth(UnitValue.CreatePercentValue(100)); //It needs to be a Percent value so that the line will stretch from margin to margin
            line.SetFontColor(ColorConstants.GRAY); //Sets the color of the line to Gray or near to black at least
            document.Add(line);
        }
    }

    public class Txt
    {
        public void ExportToTxt(Resume resume, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // writer.WriteLine($"{username}");
                    writer.WriteLine("Personal Information");
                    writer.WriteLine($"Name: {resume.PersonalInfo.Name}");
                    writer.WriteLine($"Email: {resume.PersonalInfo.Email}");
                    writer.WriteLine($"Phone: {resume.PersonalInfo.PhoneNumber}");
                    writer.WriteLine($"Description: {resume.PersonalInfo.Description}\n");

                    writer.WriteLine("Work Experience");
                    writer.WriteLine($"Company: {resume.WorkExperience.Company}");
                    writer.WriteLine($"Job Title: {resume.WorkExperience.JobTitle}");
                    writer.WriteLine($"Duration: {resume.WorkExperience.Duration}\n");

                    writer.WriteLine("Education");
                    writer.WriteLine($"Degree: {resume.Education.Degree}");
                    writer.WriteLine($"School: {resume.Education.School}");
                    writer.WriteLine($"Year of Graduation: {resume.Education.YearOfGraduation}\n");

                    writer.WriteLine("Skills");
                    foreach (var skill in resume.Skills.SkillList)
                    {
                        writer.WriteLine($"- {skill}");
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nText file saved successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error saving resume: " + ex.Message); Console.ResetColor();
            }
        }
    }
}