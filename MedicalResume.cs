using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using System.Text;

namespace ResumeBuilderApp
{
    public class MedicalResume : Resume
    {
        // Custom sections for medical resumes
        public List<Section> CustomSections { get; set; } = new List<Section>();
        public License License { get; set; } = new License(); 

        public void SaveToPdf(string pdfPath)
        {
            try
            {
                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        Style noSpaceStyle = new Style().SetMarginTop(0).SetMarginBottom(0);

                        // Placeholder box for image (optional)
                        iText.Kernel.Geom.Rectangle placeholderBox = new iText.Kernel.Geom.Rectangle(37, 700, 100, 100);
                        PdfCanvas canvas = new PdfCanvas(pdf.AddNewPage());
                        canvas.SetStrokeColor(ColorConstants.BLACK);
                        canvas.SetLineWidth(1);
                        canvas.Rectangle(placeholderBox);
                        canvas.Stroke();

                        document.Add(new Paragraph().SetMarginBottom(100));

                        // Default Sections
                        document.Add(new Paragraph($"{PersonalInfo.Name}").SetFontSize(20).SetBold().SetMarginTop(10).AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"{License.MedicalLicense}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Email: {PersonalInfo.Email}").AddStyle(noSpaceStyle));
                        document.Add(new Paragraph($"Phone: {PersonalInfo.PhoneNumber}").AddStyle(noSpaceStyle));
                        FileHandler.AddLine(document);
                        document.Add(new Paragraph($"{PersonalInfo.Description}"));
                        FileHandler.AddLine(document);

                        // Custom Sections
                        if (CustomSections != null)
                        {
                            foreach (var section in CustomSections)
                            {
                                if (section is MedicalExperience)
                                {
                                    document.Add(new Paragraph("MEDICAL EXPERIENCE").SetBold());
                                }
                                else if (section is Certificates)
                                {
                                    document.Add(new Paragraph("CERTIFICATES").SetBold());
                                }

                                if (section is MedicalExperience medicalExperienceSection)
                                {
                                    foreach (var experience in medicalExperienceSection.ExperienceList)
                                    {
                                        document.Add(new Paragraph($"- {experience.Position} at {experience.Institution} ({experience.Duration})").AddStyle(noSpaceStyle));
                                    }
                                }
                                else if (section is Certificates certificatesSection)
                                {
                                    foreach (var certificate in certificatesSection.CertificateList)
                                    {
                                        document.Add(new Paragraph($"- {certificate}").AddStyle(noSpaceStyle));
                                    }
                                }

                                document.Add(new Paragraph());
                                FileHandler.AddLine(document);
                            }
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("PDF resume saved successfully!");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError while saving PDF: " + ex.Message);
                Console.ResetColor();
            }
        }

        public void SaveToTxt(string txtPath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(txtPath))
                {
                    writer.WriteLine("ResumeType: Medical");

                    // Personal Information
                    writer.WriteLine("\nPersonal Information");
                    writer.WriteLine($"Name: {PersonalInfo.Name}");
                    writer.WriteLine($"Medical License: {License.MedicalLicense}");
                    writer.WriteLine($"Email: {PersonalInfo.Email}");
                    writer.WriteLine($"Phone: {PersonalInfo.PhoneNumber}");
                    writer.WriteLine($"Description: {PersonalInfo.Description}");

                    // Work Experience
                    writer.WriteLine("\nWork Experience");
                    writer.WriteLine($"Company: {WorkExperience.Company}");
                    writer.WriteLine($"Job Title: {WorkExperience.JobTitle}");
                    writer.WriteLine($"Duration: {WorkExperience.Duration}");

                    // Education
                    writer.WriteLine("\nEducation");
                    writer.WriteLine($"Degree: {Education.Degree}");
                    writer.WriteLine($"School: {Education.School}");
                    writer.WriteLine($"Year of Graduation: {Education.YearOfGraduation}");

                    // Skills
                    writer.WriteLine("\nSkills");
                    foreach (var skill in Skills.SkillList)
                    {
                        writer.WriteLine($"- {skill}");
                    }

                    // Custom Sections
                    if (CustomSections != null)
                    {
                        foreach (var section in CustomSections)
                        {
                            if (section is MedicalExperience medicalExperienceSection)
                            {
                                writer.WriteLine("MEDICAL EXPERIENCE");
                                foreach (var experience in medicalExperienceSection.ExperienceList)
                                {
                                    writer.WriteLine($"- {experience.Position} at {experience.Institution} ({experience.Duration})");
                                }
                            }
                            else if (section is Certificates certificatesSection)
                            {
                                writer.WriteLine("CERTIFICATES");
                                foreach (var certificate in certificatesSection.CertificateList)
                                {
                                    writer.WriteLine($"- {certificate}");
                                }
                            }

                            writer.WriteLine();
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Resume saved to text file successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error saving to text file: " + ex.Message);
                Console.ResetColor();
            }
        }

        public static MedicalResume LoadFromTxt(string txtPath)
        {
            var medicalResume = new MedicalResume();
            try
            {
                using (StreamReader reader = new StreamReader(txtPath))
                {
                    string line;
                    bool inMedicalExperience = false;
                    bool inCertificates = false;

                    // Read the file line by line
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Skip empty lines or extra spaces
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("Name:"))
                        {
                            medicalResume.PersonalInfo.Name = line.Replace("Name:", "").Trim();
                        }
                        else if (line.StartsWith("Email:"))
                        {
                            medicalResume.PersonalInfo.Email = line.Replace("Email:", "").Trim();
                        }
                        else if (line.StartsWith("Phone:"))
                        {
                            medicalResume.PersonalInfo.PhoneNumber = line.Replace("Phone:", "").Trim();
                        }
                        else if (line.StartsWith("Description:"))
                        {
                            medicalResume.PersonalInfo.Description = line.Replace("Description:", "").Trim();
                        }
                        else if (line.StartsWith("Medical License:"))
                        {
                            medicalResume.License.MedicalLicense = line.Replace("Medical License:", "").Trim();
                        }
                        else if (line.StartsWith("Work Experience"))
                        {
                            inMedicalExperience = true;
                            inCertificates = false;
                        }
                        else if (line.StartsWith("Certificates"))
                        {
                            inMedicalExperience = false;
                            inCertificates = true;
                        }
                        else if (inMedicalExperience)
                        {
                            if (line.StartsWith("-"))
                            {
                                string[] parts = line.Substring(1).Split(new[] { "at", " (" }, StringSplitOptions.None);
                                if (parts.Length == 3)
                                {
                                    var experience = new MedicalEntry
                                    {
                                        Position = parts[0].Trim(),
                                        Institution = parts[1].Trim(),
                                        Duration = parts[2].Replace(")", "").Trim()
                                    };
                                    var medicalExperienceSection = medicalResume.CustomSections.OfType<MedicalExperience>().FirstOrDefault();
                                    medicalExperienceSection?.ExperienceList.Add(experience);
                                }
                            }
                        }
                        else if (inCertificates)
                        {
                            if (line.StartsWith("-"))
                            {
                                var certificatesSection = medicalResume.CustomSections.OfType<Certificates>().FirstOrDefault();
                                certificatesSection?.CertificateList.Add(line.Substring(1).Trim());
                            }
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Resume loaded from text file successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error loading from text file: " + ex.Message);
                Console.ResetColor();
            }

            return medicalResume;
        }

        // This adds Custom Sections
        public void AddCustomSection(Section section)
        {
            CustomSections.Add(section);
        }

        public void CollectMedicalData()
        {
            Console.WriteLine("\n-- Collecting Medical Resume Data --\n");
            // Collect standard resume data
            PersonalInfo.CollectData();
            WorkExperience.CollectData();
            Education.CollectData();
            Skills.CollectData();

            // Collect custom sections
            Console.WriteLine("\nAdding Medical-Specific Sections:\n");
            AddCustomSection(new License());
            AddCustomSection(new MedicalExperience());
            AddCustomSection(new Certificates());
       
            foreach (var section in CustomSections)
            {
                section.CollectData();
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            // Append Personal Info and other sections
            result.AppendLine($"{PersonalInfo}\n{WorkExperience}\n{Education}\n{Skills}");

            // Add Medical License
            result.AppendLine($"Medical License: {License.MedicalLicense}\n");

            // Add custom sections
            foreach (var section in CustomSections)
            {
                if (section is MedicalExperience medicalExperienceSection)
                {
                    result.AppendLine("Medical Experience:");
                    foreach (var entry in medicalExperienceSection.ExperienceList)
                    {
                        result.AppendLine($"  - {entry.Position} at {entry.Institution} ({entry.Duration})");
                    }
                }
                else if (section is Certificates certificatesSection)
                {
                    result.AppendLine("\nMedical Certificates:");
                    foreach (var certificate in certificatesSection.CertificateList)
                    {
                        result.AppendLine($"  - {certificate}");
                    }
                }
            }

            return result.ToString();
        }
    }

    public class License : Section
    {
        public string MedicalLicense { get; set; }

        public override void CollectData()
        {
            // Collect Medical License
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nEnter Medical License: ");
            MedicalLicense = GetInput("Medical License"); Console.ResetColor();
        }

        public override string ToString()
        {
            return $"Medical License: {MedicalLicense}";
        }
    }

    public class MedicalExperience : Section  // Renamed class from WorkExperience to MedicalExperience
    {
        public List<MedicalEntry> ExperienceList { get; set; } = new List<MedicalEntry>();  // Renamed List to ExperienceList

        public override void CollectData()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nEnter Medical Experience (type 'done' to stop):");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter Position: ");
                Console.ResetColor();
                string position = GetInput("Position");

                if (position.ToLower() == "done") break;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter Institution: ");
                Console.ResetColor();
                string institution = GetInput("Institution");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter Duration: ");
                Console.ResetColor();
                string duration = GetInput("Duration");

                // Create a new MedicalEntry object and add it to the list
                ExperienceList.Add(new MedicalEntry
                {
                    Position = position,
                    Institution = institution,
                    Duration = duration
                });
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Medical Experience:");

            // Display each medical experience in a readable format
            foreach (var entry in ExperienceList)
            {
                sb.AppendLine($"  - {entry.Position} at {entry.Institution} ({entry.Duration})");
            }

            return sb.ToString();
        }
    }

    public class MedicalEntry  // Renamed WorkEntry to MedicalEntry
    {
        public string Position { get; set; }
        public string Institution { get; set; }
        public string Duration { get; set; }
    }

    public class Certificates : Section
    {
        public List<string> CertificateList { get; set; } = new List<string>();

        public override void CollectData()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Clear();
            Console.WriteLine("\nEnter Certificates (type 'done' to stop):");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter Certificate Title, Issuer, Date: ");
                Console.ResetColor();
                string input = GetInput("Certificate Title, Issuer, Date");

                if (input.ToLower() == "done") break;

                CertificateList.Add(input);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Medical Certificates:");

            // Display each certificate in a readable format
            foreach (var certificate in CertificateList)
            {
                sb.AppendLine($"  - {certificate}");
            }

            return sb.ToString();
        }
    }
}
