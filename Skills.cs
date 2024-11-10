﻿using System;
using System.Collections.Generic;

namespace ResumeBuilderApp
{
    public class Skills : Section
    {
        private List<string> _skillList = new List<string>();

        public List<string> SkillList 
        { 
            get => this._skillList; 
            set => this._skillList = value; 
        }

        public override void CollectData()
        {
            Console.WriteLine("Enter skills (type 'done' to finish):");
            while (true)
            {
                string? skill = Console.ReadLine();
                if (skill == null || skill == string.Empty)
                {
                    Console.Write("Cannot enter empty skill!\n");
                    continue;
                }

                if (skill?.ToLower() == "done") 
                    break;

                SkillList.Add(skill);
            }
        }

        public override string ToString() => $"Skills: {string.Join(", ", SkillList)}\n";
    }
}
