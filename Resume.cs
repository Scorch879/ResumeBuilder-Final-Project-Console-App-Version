using System.Collections.Generic;

namespace ResumeBuilderApp
{
    public class Resume 
    {
        public PersonalInfo PersonalInfo { get; set; }
        public WorkExperience WorkExperience { get; set; }
        public Education Education { get; set; }
        public Skills Skills { get; set; }
        public EngineeringResume? EngineeringResume { get; set; }
        public BPOResume? BpoResume { get; set; }
        public MedicalResume? MedicalResume { get; set; }
        public Resume()
        {
            // Initialize sections
            PersonalInfo = new PersonalInfo();
            WorkExperience = new WorkExperience();
            Education = new Education();
            Skills = new Skills();
        }

        public void CollectData()
        {
            // Collect data for each section (default)
            PersonalInfo.CollectData();
            WorkExperience.CollectData();
            Education.CollectData();
            Skills.CollectData();
        }

    }
}
