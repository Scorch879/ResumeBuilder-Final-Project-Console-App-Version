using System.Collections.Generic;

namespace ResumeBuilderApp
{
    public class Resume
    {
        public PersonalInfo PersonalInfo { get; set; } = new PersonalInfo();
        public WorkExperience WorkExperience { get; set; } = new WorkExperience();
        public Education Education { get; set; } = new Education();
        public Skills Skills { get; set; } = new Skills();
    }
}
