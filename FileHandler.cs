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
        // Method to save based on file extension
        public static void SaveToFile(Resume resume)
        {
            string? filePathPdf, filePathTxt, fileName;

            beginning:
            //Input name of the file
            Console.WriteLine("\nExporting....");
            Console.Write("Enter the file name: ");
            fileName = Console.ReadLine();

            // Get the Documents path and prompt the user for the file name
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Get the path of the document folder
            string folderPath = Path.Combine(documentsPath, "Resume Text Files Data");

            //Checks if the folder exists or not
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
                
            // Combine path and user input for the complete file path
            filePathPdf = Path.Combine(documentsPath, fileName + ".pdf"); //pdf going to the MyDocuments folder
            filePathTxt = Path.Combine(folderPath, fileName + ".txt"); //txt file going to the folder "Resume Text Files Data" in MyDocuments

            try
            {
                //Check if the file exists or not
                if (File.Exists(filePathPdf) || File.Exists(filePathTxt))
                {
                    Console.Write("This name is already taken. Do you want to overwrite it? (y/n): ");
                    string? choice = Console.ReadLine();
                    if (choice?.ToLower() != "y")
                    {
                        Console.WriteLine("Save Canceled");
                        Console.WriteLine("Do you want to export it? (y/n): ");
                            choice = Console.ReadLine();
                            if (choice?.ToLower() == "y")
                                goto beginning;
                        return;
                    }
                }

                new Txt().ExportToTxt(resume, filePathTxt); //Saves the file into a pdf also
                new PDF().ExportToPdf(resume, filePathPdf); //Saving the file into a pdf

               
                Console.Write("\nResume Saved Successfully");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void LoadingIndicator()
        {

            while (true)
            {
                Console.Write("Saving PDF");
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(500); //delay for dots
                }
                Console.Write("\b\b\b   \b\b\b"); //reset dots 
                Console.WriteLine();
            }
        }

        public static Resume LoadFromTxtFile()
         {
            start:
            //User enters the file name
            Console.Write("Enter the file name to load (without file extension): ");
            string? fileName = Console.ReadLine()?.Trim();

            if (fileName == null)
            {
                Console.WriteLine("No file name entered. Try Again Please");
                goto start;
            }

            // Get the Documents path and prompt the user for the file name
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //get the path of the document folder 
            string folderPath = Path.Combine(documentsPath, "Resume Text Files Data"); //gets the path of the folder 

            string filePathTxt = Path.Combine(folderPath, fileName + ".txt"); //txt file going to the folder "Resume Text Files Data" in MyDocuments

            Console.WriteLine($"Trying to load resume file: {fileName}");

            //checks if it exists
            if (!File.Exists(filePathTxt))
            {
                Console.WriteLine($"File not found: {fileName}");
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
                    while((skill = reader.ReadLine()) != null)
                    {
                        resume.Skills.SkillList.Add(skill.TrimStart('-').Trim());
                    }
                }

                Console.WriteLine("\nResume Loaded Successfully\n");
                Console.Write("Press any key to continue");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading resume: " + ex.Message);
            }
            
            return resume;
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

                        // Skills
                        document.Add(new Paragraph("SKILLS").SetBold());
                        foreach (var skill in resume.Skills.SkillList)
                        {
                            document.Add(new Paragraph($"- {skill}"));
                        }

                        Console.WriteLine("PDF resume saved successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError caught: " + ex);
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

                    Console.WriteLine("\nText file saved successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving resume: " + ex.Message);
            }
        }
    }

}