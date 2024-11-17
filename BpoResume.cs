using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using System.Text;


namespace ResumeBuilderApp
{
    public class BPOResume : Resume
    {
        // Custom sections for engineering resumes
        public List<Section> CustomSections { get; set; } = new List<Section>();

        /// <summary>
        /// Saving logics here
        /// </summary>

        //for pdf
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

                        // Default Sections (same as before)
                        document.Add(new Paragraph($"{PersonalInfo.Name}").SetFontSize(20).SetBold().SetMarginTop(10).AddStyle(noSpaceStyle));
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
                                if (section is BPOVolunteerExperiences)
                                {
                                    document.Add(new Paragraph("VOLUNTEER EXPERIENCE").SetBold());
                                }
                                else if (section is Languages)
                                {
                                    document.Add(new Paragraph("LANGUAGES").SetBold());
                                }
                                else if (section is Interests)
                                {
                                    document.Add(new Paragraph("INTERESTS").SetBold());
                                }

                                if (section is BPOVolunteerExperiences volunteerSection)
                                {
                                    foreach (var experience in volunteerSection.VolunteerList)
                                    {
                                        document.Add(new Paragraph($"- {experience}").AddStyle(noSpaceStyle));
                                    }
                                }
                                else if (section is Languages languagesSection)
                                {
                                    foreach (var language in languagesSection.LanguageList)
                                    {
                                        document.Add(new Paragraph($"- {language}").AddStyle(noSpaceStyle));
                                    }
                                }
                                else if (section is Interests interestsSection)
                                {
                                    foreach (var interest in interestsSection.InterestList)
                                    {
                                        document.Add(new Paragraph($"- {interest}").AddStyle(noSpaceStyle));
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

        //Save to text method
        public void SaveToTxt(string txtPath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(txtPath))
                {
                    writer.WriteLine("ResumeType: Engineering");

                    // Personal Information
                    writer.WriteLine($"Name: {PersonalInfo.Name}");
                    writer.WriteLine($"Email: {PersonalInfo.Email}");
                    writer.WriteLine($"Phone: {PersonalInfo.PhoneNumber}");
                    writer.WriteLine($"Description: {PersonalInfo.Description}");
                    writer.WriteLine();

                    // Work Experience
                    writer.WriteLine("WORK EXPERIENCE");
                    writer.WriteLine($"Company: {WorkExperience.Company}");
                    writer.WriteLine($"Job Title: {WorkExperience.JobTitle}");
                    writer.WriteLine($"Duration: {WorkExperience.Duration}");
                    writer.WriteLine();

                    // Education
                    writer.WriteLine("EDUCATION");
                    writer.WriteLine($"Degree: {Education.Degree}");
                    writer.WriteLine($"School: {Education.School}");
                    writer.WriteLine($"Year of Graduation: {Education.YearOfGraduation}");
                    writer.WriteLine();

                    // Skills
                    writer.WriteLine("SKILLS");
                    foreach (var skill in Skills.SkillList)
                    {
                        writer.WriteLine($"- {skill}");
                    }
                    writer.WriteLine();

                    // Custom Sections
                    if (CustomSections != null)
                    {
                        foreach (var section in CustomSections)
                        {
                            if (section is BPOVolunteerExperiences)
                            {
                                writer.WriteLine("VOLUNTEER EXPERIENCE");
                                foreach (var experience in (section as BPOVolunteerExperiences).VolunteerList)
                                {
                                    writer.WriteLine($"- {experience}");
                                }
                            }
                            else if (section is Languages)
                            {
                                writer.WriteLine("LANGUAGES");
                                foreach (var language in (section as Languages).LanguageList)
                                {
                                    writer.WriteLine($"- {language}");
                                }
                            }
                            else if (section is Interests)
                            {
                                writer.WriteLine("INTERESTS");
                                foreach (var interest in (section as Interests).InterestList)
                                {
                                    writer.WriteLine($"- {interest}");
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


        //LoadFromTxt method similar to the fileHandler
         public static BPOResume LoadFromTxt(string txtPath)
         {
             var bpoResume = new BPOResume();

             try
             {
                 using (StreamReader reader = new StreamReader(txtPath))
                 {
                     string line;
                     while ((line = reader.ReadLine()) != null)
                     {
                         // Parsing Personal Information
                         if (line.StartsWith("Name:"))
                         {
                             bpoResume.PersonalInfo.Name = line.Substring(5).Trim();
                         }
                         else if (line.StartsWith("Email:"))
                         {
                             bpoResume.PersonalInfo.Email = line.Substring(6).Trim();
                         }
                         else if (line.StartsWith("Phone:"))
                         {
                             bpoResume.PersonalInfo.PhoneNumber = line.Substring(6).Trim();
                         }
                         else if (line.StartsWith("Description:"))
                         {
                             bpoResume.PersonalInfo.Description = line.Substring(12).Trim();
                         }

                         // Parsing Work Experience
                         else if (line.StartsWith("WORK EXPERIENCE"))
                         {
                             bpoResume.WorkExperience = new WorkExperience();
                             while ((line = reader.ReadLine()) != null && !line.StartsWith("EDUCATION"))
                             {
                                 if (line.StartsWith("Company:"))
                                 {
                                     bpoResume.WorkExperience.Company = line.Substring(8).Trim();
                                 }
                                 else if (line.StartsWith("Job Title:"))
                                 {
                                     bpoResume.WorkExperience.JobTitle = line.Substring(10).Trim();
                                 }
                                 else if (line.StartsWith("Duration:"))
                                 {
                                     bpoResume.WorkExperience.Duration = line.Substring(9).Trim();
                                 }
                             }
                         }

                         // Parsing Education
                         else if (line.StartsWith("EDUCATION"))
                         {
                             bpoResume.Education = new Education();
                             while ((line = reader.ReadLine()) != null && !line.StartsWith("SKILLS"))
                             {
                                 if (line.StartsWith("Degree:"))
                                 {
                                     bpoResume.Education.Degree = line.Substring(7).Trim();
                                 }
                                 else if (line.StartsWith("School:"))
                                 {
                                     bpoResume.Education.School = line.Substring(7).Trim();
                                 }
                                 else if (line.StartsWith("Year of Graduation:"))
                                 {
                                     bpoResume.Education.YearOfGraduation = line.Substring(19).Trim();
                                 }
                             }
                         }

                         // Parsing Skills
                         else if (line.StartsWith("SKILLS"))
                         {
                             bpoResume.Skills = new Skills();
                             while ((line = reader.ReadLine()) != null && !line.StartsWith("VOLUNTEER EXPERIENCE") && !line.StartsWith("LANGUAGES") && !line.StartsWith("INTERESTS"))
                             {
                                 if (line.StartsWith("-"))
                                 {
                                     bpoResume.Skills.SkillList.Add(line.Substring(2).Trim());
                                 }
                             }
                         }

                         // Parsing Volunteer Experience
                         else if (line.StartsWith("VOLUNTEER EXPERIENCE"))
                         {
                             var volunteerExperience = new BPOVolunteerExperiences();
                             while ((line = reader.ReadLine()) != null && !line.StartsWith("LANGUAGES"))
                             {
                                 if (line.StartsWith("-"))
                                 {
                                     volunteerExperience.VolunteerList.Add(line.Substring(2).Trim());
                                 }
                             }
                             bpoResume.CustomSections.Add(volunteerExperience);
                         }

                         // Parsing Languages
                         else if (line.StartsWith("LANGUAGES"))
                         {
                             var languages = new Languages();
                             while ((line = reader.ReadLine()) != null && !line.StartsWith("INTERESTS"))
                             {
                                 if (line.StartsWith("-"))
                                 {
                                     languages.LanguageList.Add(line.Substring(2).Trim());
                                 }
                             }
                             bpoResume.CustomSections.Add(languages);
                         }

                         // Parsing Interests
                         else if (line.StartsWith("INTERESTS"))
                         {
                             var interests = new Interests();
                             while ((line = reader.ReadLine()) != null)
                             {
                                 if (line.StartsWith("-"))
                                 {
                                     interests.InterestList.Add(line.Substring(2).Trim());
                                 }
                             }
                             bpoResume.CustomSections.Add(interests);
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

             return bpoResume;
         } 

        /// <summary>
        /// End of Saving Methods
        /// </summary

        //This adds Custom Sections
        public void AddCustomSection(Section section)
        {
            CustomSections.Add(section);
        }

        //Collect the custom sections for Engineering data
        public void CollectBPOData()
        {
            Console.WriteLine("\n-- Collecting BPO Resume Data --\n");

            // Collect standard resume data
            PersonalInfo.CollectData();
            WorkExperience.CollectData();
            Education.CollectData();
            Skills.CollectData();

            // Collect custom sections
            Console.WriteLine("\nAdding BPO-Specific Sections:\n");
            AddCustomSection(new BPOVolunteerExperiences());
            AddCustomSection(new Languages());
            AddCustomSection(new Interests());

            foreach (var section in CustomSections)
            {
                section.CollectData();
            }
        }

        public override string ToString()
        {
            string result = $"{PersonalInfo}\n{WorkExperience}\n{Education}\n{Skills}\n";

            foreach (var section in CustomSections)
            {
                result += $"{section}\n";
            }

            return result;
        }

    }
   
    //Custom Sections
    public class BPOVolunteerExperiences : Section
    {
        public List<string> VolunteerList { get; private set; }

        public BPOVolunteerExperiences()
        {
            VolunteerList = new List<string>();
        }

        public override void CollectData()
        {
            Console.WriteLine("\nEnter your Volunteer Experience (type 'done' to finish):");
            while (true)
            {
                Console.Write("Volunteer Experience: ");
                string? experience = GetInput("experience");
                if (experience?.ToLower() == "done") break;

                if (!string.IsNullOrEmpty(experience))
                    VolunteerList.Add(experience);
            }
        }

        public override string ToString()
        {
            return $"Volunteer Experience:\n" + string.Join("\n", VolunteerList);
        }
    }

    public class Languages : Section
    {
        public List<string> LanguageList { get; private set; }

        public Languages()
        {
            LanguageList = new List<string>();
        }

        public override void CollectData()
        {
            Console.WriteLine("\nEnter the languages you speak and their proficiency (type 'done' to finish):");
            while (true)
            {
                Console.Write("Language (and Proficiency): ");
                string? language = GetInput("language");
                if (language?.ToLower() == "done") break;

                if (!string.IsNullOrWhiteSpace(language))
                {
                    LanguageList.Add(language);
                }
            }
        }

        public override string ToString()
        {
            return "\nLanguages:\n" + string.Join("\n", LanguageList);
        }
    }

    public class Interests : Section
    {
        public List<string> InterestList { get; private set; }

        public Interests()
        {
            InterestList = new List<string>();
        }

        public override void CollectData()
        {
            Console.WriteLine("\nEnter your personal interests (type 'done' to finish):");
            while (true)
            {
                Console.Write("Interest: ");
                string? interest = GetInput("interest");
                if (interest?.ToLower() == "done") break;

                if (!string.IsNullOrWhiteSpace(interest))
                {
                    InterestList.Add(interest);
                }
            }
        }

        public override string ToString()
        {
            return "\nInterests:\n" + string.Join("\n", InterestList);
        }
    }

}