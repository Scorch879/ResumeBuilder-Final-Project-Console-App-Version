using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using System.Text;
using System.Data;

namespace ResumeBuilderApp
{
public class EngineeringResume : Resume
{
    // Custom sections for engineering resumes
    public List<Section> CustomSections { get; private set; } = new List<Section>();

        public Projects Projects { get; set; } = new Projects();
        public Certifications Certifications { get; set; } = new Certifications();

        public List<Course> Courses { get; set; } = new List<Course>();
        public CharacterReferences CharacterReference { get; set; } = new CharacterReferences();

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

                // Personal Information
                document.Add(new Paragraph($"{PersonalInfo.Name}").SetFontSize(20).SetBold().SetMarginTop(10).AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"Email: {PersonalInfo.Email}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"Phone: {PersonalInfo.PhoneNumber}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph());
                FileHandler.AddLine(document);
                document.Add(new Paragraph($"{PersonalInfo.Description}"));
                document.Add(new Paragraph());
                FileHandler.AddLine(document);
                document.Add(new Paragraph());

                // Work Experience
                document.Add(new Paragraph("WORK EXPERIENCE").SetBold());
                document.Add(new Paragraph($"Company: {WorkExperience.Company}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"Job Title: {WorkExperience.JobTitle}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"Duration: {WorkExperience.Duration}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph());
                FileHandler.AddLine(document);

                // Education
                document.Add(new Paragraph("EDUCATION").SetBold());
                document.Add(new Paragraph($"Degree: {Education.Degree}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"School: {Education.School}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph($"Year of Graduation: {Education.YearOfGraduation}").AddStyle(noSpaceStyle));
                document.Add(new Paragraph());
                FileHandler.AddLine(document);

                // Skills
                document.Add(new Paragraph("SKILLS").SetBold());
                foreach (var skill in Skills.SkillList)
                {
                    document.Add(new Paragraph($"- {skill}"));
                }
                document.Add(new Paragraph());
                FileHandler.AddLine(document);

                //Titles
                foreach (var section in CustomSections)
                {
                    if (section is Projects)
                    {
                        document.Add(new Paragraph("PROJECTS").SetBold());
                        FileHandler.AddLine(document);
                    }
                    else if (section is Certifications)
                    {
                        document.Add(new Paragraph("CERTIFICATIONS").SetBold());
                        FileHandler.AddLine(document);
                    }
                    else if (section is Courses)
                    {
                        document.Add(new Paragraph("COURSES AND TRAINING").SetBold());
                        FileHandler.AddLine(document);
                    }
                       
                    else if (section is CharacterReferences)
                    {
                        document.Add(new Paragraph("CHARACTER REFERENCES").SetBold());
                        FileHandler.AddLine(document);
                    }

                    // Custom Sections
                    if (section is Projects projectsSection)
                    {
                        foreach (var project in projectsSection.ProjectList)
                        {
                            document.Add(new Paragraph($"- {project.ProjectName}"));
                            document.Add(new Paragraph($"  Description: {project.ProjectDescription}").AddStyle(noSpaceStyle));
                            document.Add(new Paragraph($"  Duration: {project.Duration}").AddStyle(noSpaceStyle));
                            FileHandler.AddLine(document);
                        }
                    }
                    else if (section is Certifications certificationsSection)
                    {
                        foreach (var certification in certificationsSection.CertificationList)
                        {
                            document.Add(new Paragraph($"- {certification}").AddStyle(noSpaceStyle));
                        }
                    }
                    else if (section is Courses coursesSection)
                    {
                        foreach (var course in coursesSection.CourseList)
                        {
                            document.Add(new Paragraph($"- {course}").AddStyle(noSpaceStyle));
                        }
                    }
                    else if (section is CharacterReferences referencesSection)
                    {
                        foreach (var reference in referencesSection.ReferenceList)
                        {
                            document.Add(new Paragraph($"- {reference.ReferenceName}, {reference.Relationship}").AddStyle(noSpaceStyle));
                            document.Add(new Paragraph($"  Contact: {reference.ContactInfo}").AddStyle(noSpaceStyle));
                        }
                    }

                    document.Add(new Paragraph());
                    FileHandler.AddLine(document);
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

    //for text
    public void SaveToTxt(string txtPath)
    {
        using (StreamWriter writer = new StreamWriter(txtPath))
        {
            writer.WriteLine("ResumeType: Engineering");

            // Personal Information
            writer.WriteLine("\nPersonal Information");
            writer.WriteLine($"Name: {PersonalInfo.Name}");
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
            foreach (var section in CustomSections)
            {
                if (section is Projects projectsSection)
                {
                    writer.WriteLine("\nProjects");
                    foreach (var project in projectsSection.ProjectList)
                    {
                        writer.WriteLine($"- {project.ProjectName}");
                        writer.WriteLine($"  Description: {project.ProjectDescription}");
                        writer.WriteLine($"  Duration: {project.Duration}");
                    }
                }
                else if (section is Certifications certificationsSection)
                {
                    writer.WriteLine("\nCertifications");
                    foreach (var certification in certificationsSection.CertificationList)
                    {
                        writer.WriteLine($"- {certification}");
                    }
                }
                else if (section is Courses coursesSection)
                {
                    writer.WriteLine("\nCourses and Training");
                    foreach (var course in coursesSection.CourseList)
                    {
                        writer.WriteLine($"- {course}");
                    }
                }
                else if (section is CharacterReferences referencesSection)
                {
                    writer.WriteLine("\nCharacter References");
                    foreach (var reference in referencesSection.ReferenceList)
                    {
                        writer.WriteLine($"- {reference.ReferenceName}, {reference.Relationship}");
                        writer.WriteLine($"  Contact: {reference.ContactInfo}");
                    }
                }
            }
        }
    }

        /// <summary>
        /// 
        /// End of Saving Methods
        /// </summary

        public static EngineeringResume LoadFromTxt(string filePathTxt)
        {
            EngineeringResume resume = new EngineeringResume();

            try
            {
                using (StreamReader reader = new StreamReader(filePathTxt))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                  
                        if (line.StartsWith("Personal Information"))
                        {
                            // Parse personal information
                            resume.PersonalInfo.Name = reader.ReadLine()?.Split(": ")[1];
                            resume.PersonalInfo.Email = reader.ReadLine()?.Split(": ")[1];
                            resume.PersonalInfo.PhoneNumber = reader.ReadLine()?.Split(": ")[1];
                            resume.PersonalInfo.Description = reader.ReadLine()?.Split(": ")[1];

                            
                        }
                        else if (line.StartsWith("Work Experience"))
                        {
                            // Parse work experience
                            resume.WorkExperience.Company = reader.ReadLine()?.Split(": ")[1];
                            resume.WorkExperience.JobTitle = reader.ReadLine()?.Split(": ")[1];
                            resume.WorkExperience.Duration = reader.ReadLine()?.Split(": ")[1];
                          
                        }
                        else if (line.StartsWith("Education"))
                        {
                            // Parse education
                            resume.Education.Degree = reader.ReadLine()?.Split(": ")[1];
                            resume.Education.School = reader.ReadLine()?.Split(": ")[1];
                            resume.Education.YearOfGraduation = reader.ReadLine()?.Split(": ")[1];
                           ;
                        }
                        else if (line.StartsWith("Skills"))
                        {
                            // Parse skills
                            while ((line = reader.ReadLine()) != null && line.StartsWith("-"))
                            {
                                resume.Skills.SkillList.Add(line.TrimStart('-').Trim());
                               
                            }
                        }
                        else if (line.StartsWith("Projects"))
                        {
                            // Parse projects
                            while ((line = reader.ReadLine()) != null && !line.StartsWith("Certifications") && !line.StartsWith("Courses and Training") && !line.StartsWith("Character References"))
                            {
                                if (line.StartsWith("-"))
                                {
                                    string projectName = line.TrimStart('-').Trim();
                                    string projectDescription = reader.ReadLine()?.Split(": ")[1] ?? "";
                                    string projectDuration = reader.ReadLine()?.Split(": ")[1] ?? "";
                                    resume.Projects.ProjectList.Add(new Project(projectName, projectDescription, projectDuration));
                                }
                            }
                        }
                        else if (line.StartsWith("Certifications"))
                        {
                            while ((line = reader.ReadLine()) != null && line.StartsWith("-"))
                            {
                                string[] certificationData = line.TrimStart('-').Trim().Split(",");
                                string certificationName = certificationData[0].Trim();
                                string issuingOrganization = certificationData[1].Trim();
                                string dateIssued = certificationData[2].Trim();
                                resume.Certifications.CertificationList.Add(new Certification(certificationName, issuingOrganization, dateIssued));
                            }
                        }
                        else if (line.StartsWith("Courses and Training"))
                        {
                            while ((line = reader.ReadLine()) != null && line.StartsWith("-"))
                            {
                                string[] courseData = line.TrimStart('-').Trim().Split(",");
                                string courseName = courseData[0].Trim();
                                string provider = courseData[1].Trim();
                                string duration = courseData[2].Trim();
                                resume.Courses.Add(new Course(courseName, provider, duration));
                            }
                        }
                        else if (line.StartsWith("Character References"))
                        {
                            // Parse character references
                            while ((line = reader.ReadLine()) != null && line.StartsWith("-"))
                            {
                                string[] referenceData = line.TrimStart('-').Trim().Split(",");
                                string referenceName = referenceData[0].Trim();
                                string relationship = referenceData[1].Trim();
                                string contactInfo = reader.ReadLine()?.Split(": ")[1] ?? "";
                                resume.CharacterReference.ReferenceList.Add(new CharacterReference(referenceName, relationship, contactInfo));
                            }
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PDF resume saved successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading resume from text file: " + ex.Message);
            }
            return resume;
        }



        //This adds Custom Sections
        public void AddCustomSection(Section section)
    {
        CustomSections.Add(section);
    }

    //Collect the custom sections for Engineering data
    public void CollectEngineeringData()
    {
        Console.WriteLine("\n-- Collecting Engineering Resume Data --");

        // Collect standard resume data
        PersonalInfo.CollectData();
        WorkExperience.CollectData();
        Education.CollectData();
        Skills.CollectData();

        // Collect custom sections
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nAdding Engineering-Specific Sections:");
        AddCustomSection(new Projects());
        AddCustomSection(new Certifications());
        AddCustomSection(new Courses());
        AddCustomSection(new CharacterReferences());

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


    public class Projects : Section
    {
        // List to hold multiple project entries
        public List<Project> ProjectList { get; private set; }

        public Projects()
        {
            ProjectList = new List<Project>(); // Initialize ProjectList
        }

        public override void CollectData()
        {
            Console.WriteLine("\nEnter your engineering projects (type 'done' to finish):");
            while (true)
            {
                Console.Write("Project Name: ");
                string? projectName = GetInput("Project Name");

                if (projectName?.ToLower() == "done") break;
                if (string.IsNullOrEmpty(projectName)) continue;

                Console.Write("Project Description: ");
                string? projectDescription = GetInput("Project Description");

                Console.Write("Project Duration (e.g., 6 months, 2019-2020): ");
                string? duration = GetInput("Duration");

                // Add the project to the list
                ProjectList.Add(new Project(projectName, projectDescription, duration));  // Now adding to ProjectList
            }
        }

        public override string ToString()
        {
            var projectDetails = new StringBuilder("Projects\n");
            foreach (var project in ProjectList)
            {
                projectDetails.AppendLine($"{project.ProjectName} ({project.Duration}) - {project.ProjectDescription}");
            }
            return projectDetails.ToString();
        }
    }

    // Define the Project class to store project details
    public class Project
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string Duration { get; set; }

        public Project(string projectName, string projectDescription, string duration)
        {
            this.ProjectName = projectName;
            this.ProjectDescription = projectDescription;
            this.Duration = duration;
        }

        // Overriding ToString to format the project output nicely
        public override string ToString()
        {
            return $"{ProjectName} ({Duration}) - {ProjectDescription}";
        }
    }


    //Certification Section
    public class Certification
    {
    private string certificationName, issuingOrganization, dateIssued;

    public string CertificationName { get => certificationName; set => certificationName = value; }
    public string IssuingOrganization { get => issuingOrganization; set => issuingOrganization = value; }
    public string DateIssued { get => dateIssued; set => dateIssued = value; }

    public Certification(string certificationName, string issuingOrganization, string dateIssued)
    {
        CertificationName = certificationName;
        IssuingOrganization = issuingOrganization;
        DateIssued = dateIssued;
    }
    public override string ToString()
    {
        return $"{CertificationName} from {IssuingOrganization} ({DateIssued})";
    }
}

    public class Certifications : Section
    {
        public List<Certification> CertificationList { get; private set; }

        public Certifications()
        {
            CertificationList = new List<Certification>();
        }

        public override void CollectData()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nEnter your certifications (type 'done' to finish):");
            while (true)
            {
                Console.Write("Certification Name: ");
                string certificationName = GetInput("Certification Name");

                if (certificationName?.ToLower() == "done") break;
                if (string.IsNullOrEmpty(certificationName)) continue;

                Console.Write("Issuing Organization: ");
                string? issuingOrganization = GetInput("Issuing Organization");

                Console.Write("Date Issued (e.g., 2020, June 2022): ");
                string? dateIssued = GetInput("Date Issued");

                // Add the certification to the list
                CertificationList.Add(new Certification(certificationName, issuingOrganization, dateIssued));
            }
        }

        public override string ToString()
        {
            if (CertificationList.Count == 0)
                return "No certifications added.";

            var certificationDetails = new StringBuilder("Certifications:\n");
            foreach (var cert in CertificationList)
            {
                certificationDetails.AppendLine($"- {cert}");
            }
            return certificationDetails.ToString();
        }
    }

    //Courdes and Training Section
    public class Course
    {
        public string CourseName { get; set; }
        public string Provider { get; set; }
        public string Duration { get; set; }

        public Course(string courseName, string provider, string duration)
        {
            CourseName = courseName;
            Provider = provider;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"{CourseName} - {Provider} ({Duration})";
        }
    }

public class Courses : Section
{
    public List<Course> CourseList { get; set; }

    public Courses()
    {
        CourseList = new List<Course>();
    }

    public override void CollectData()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nEnter your courses and training (type 'done' to finish):");
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Course Name: ");
            string? courseName = GetInput("Course Name");

            if (courseName?.ToLower() == "done") break;
            if (string.IsNullOrEmpty(courseName)) continue;

            Console.Write("Provider: ");
            string? provider = GetInput("Provider");

            Console.Write("Duration (e.g., 3 months, 2020): ");
            string? duration = GetInput("Duration");

            CourseList.Add(new Course(courseName, provider, duration));
        }
    }

    public override string ToString()
    {
        if (CourseList.Count == 0)
            return "No certifications added.";

        var courseDetails = new StringBuilder("Courses & Training:\n");
        foreach (var course in CourseList)
        {
            courseDetails.AppendLine($"{course.CourseName} - {course.Provider} ({course.Duration})");
        }
        return courseDetails.ToString();
    }
}

//Character References Sections
public class CharacterReference
{
    public string ReferenceName { get; set; }
    public string Relationship { get; set; }
    public string ContactInfo { get; set; }

    public CharacterReference(string referenceName, string relationship, string contactInfo)
    {
        ReferenceName = referenceName;
        Relationship = relationship;
        ContactInfo = contactInfo;
    }

    public override string ToString()
    {
        return $"{ReferenceName}, {Relationship}, Contact: {ContactInfo}";
    }
}

public class CharacterReferences : Section
{
    public List<CharacterReference> ReferenceList { get; private set; }

    public CharacterReferences()
    {
        ReferenceList = new List<CharacterReference>();
    }

    public override void CollectData()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nEnter your character references (type 'done' to finish):");
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Reference Name: ");
            string? referenceName = GetInput("Reference Name");

            if (referenceName?.ToLower() == "done") break;
            if (string.IsNullOrEmpty(referenceName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reference name cannot be empty. Please try again.");
                Console.ResetColor();
                continue;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Relationship: ");
            string? relationship = GetInput("relationship");

            if (string.IsNullOrEmpty(relationship))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Relationship cannot be empty. Please try again.");
                Console.ResetColor();
                continue;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Contact Info (e.g., email or phone): ");
            string? contactInfo = GetInput("contactInfo");

            if (string.IsNullOrEmpty(contactInfo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Contact information cannot be empty. Please try again.");
                Console.ResetColor();
                continue;
            }

            // Add the reference to the list
            ReferenceList.Add(new CharacterReference(referenceName, relationship, contactInfo));
        }
    }

    public override string ToString()
    {
        if (ReferenceList.Count == 0)
            return "No character references";
        var referenceDetails = new StringBuilder("Character References:\n");
        foreach (var reference in ReferenceList)
        {
            referenceDetails.AppendLine(reference.ToString());
        }
        return referenceDetails.ToString();
    }
}
}