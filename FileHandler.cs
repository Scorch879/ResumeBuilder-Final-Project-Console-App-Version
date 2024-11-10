using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Drawing;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;


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
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //code to get the path of the document folder 

            Console.WriteLine("\nExport Format: " + choice);
            Console.Write("Enter the file name: ");
            fileName = Console.ReadLine();

            // Combine path and user input for the complete file path
            filePath = System.IO.Path.Combine(documentsPath, fileName); //combining the inputted filename with the file path 

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