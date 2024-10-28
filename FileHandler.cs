using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;

namespace ResumeBuilderApp
{ 
    public static class FileHandler
    {
        // Method to save based on file extension
        public static void SaveToFile(Resume resume)
        {
            string? choice, filePath, fileName;

            Console.Write("Choose Export Format (1 > Text File  | 2 > PDF): ");
            choice = Console.ReadLine();

            // Get the Documents path and prompt the user for the file name
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Console.WriteLine("\nExport Format: " + choice);
            Console.Write("Enter the file name: ");
            fileName = Console.ReadLine();

            // Combine path and user input for the complete file path
            filePath = Path.Combine(documentsPath, fileName);

            switch (choice)
            {
                case "1":
                    new Txt().ExportToTxt(resume, filePath + ".txt");
                    break;
                case "2":
                    new PDF().ExportToPdf(resume, filePath + ".pdf");
                    break;
            }
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

                        // Personal Info
                        document.Add(new Paragraph("Personal Information"));
                        document.Add(new Paragraph($"Name: {resume.PersonalInfo.Name}"));
                        document.Add(new Paragraph($"Email: {resume.PersonalInfo.Email}"));
                        document.Add(new Paragraph($"Phone: {resume.PersonalInfo.PhoneNumber}\n"));

                        // Work Experience
                        document.Add(new Paragraph("Work Experience"));
                        document.Add(new Paragraph($"Company: {resume.WorkExperience.Company}"));
                        document.Add(new Paragraph($"Job Title: {resume.WorkExperience.JobTitle}"));
                        document.Add(new Paragraph($"Duration: {resume.WorkExperience.Duration}\n"));

                        // Education
                        document.Add(new Paragraph("Education"));
                        document.Add(new Paragraph($"Degree: {resume.Education.Degree}"));
                        document.Add(new Paragraph($"School: {resume.Education.School}"));
                        document.Add(new Paragraph($"Year of Graduation: {resume.Education.YearOfGraduation}\n"));

                        // Skills
                        document.Add(new Paragraph("Skills"));
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
                Console.WriteLine("\nError caught: " + ex.Message);
            }
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
                    writer.WriteLine($"Phone: {resume.PersonalInfo.PhoneNumber}\n");

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

                    Console.WriteLine("Text resume saved successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving resume: " + ex.Message);
            }
        }
    }
}