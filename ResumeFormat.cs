namespace ResumeBuilderApp
{
    class ResumeFormat
    {
        public void StrandardFormat()
        {
            /* using (StreamReader reader = new StreamReader(filePathTxt))
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

         }

             */
        }
    }
}
