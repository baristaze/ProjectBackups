using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public partial class Education : IDBEntity
    {
        public NameLongIdPair School { get; set; }
        public NameLongIdPair Concentration { get; set; }
        public NameLongIdPair Degree { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }

        public EducationLevel EducationLevel
        {
            get
            {
                return EducationLevelHelper.ParseFromFacebook(this.Type);
            }
        }

        public override string ToString()
        {
            string s = String.Empty;
            if (!String.IsNullOrWhiteSpace(this.Type))
            {
                s += this.Type;
            }

            if (this.School != null && !String.IsNullOrWhiteSpace(this.School.Name))
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    s += " - ";
                }

                s += this.School.Name;
            }

            return s;
        }

        public static int ComparisonByEducationLevel(Education first, Education second)
        {
            return first.EducationLevel.CompareTo(second.EducationLevel);
        }

        public static Education CreateFromFacebookEdu(dynamic jsonEdu)
        {
            if (jsonEdu == null || jsonEdu.type == null || String.IsNullOrWhiteSpace(jsonEdu.type))
            {
                return null;
            }

            Education edu = new Education();
            edu.Type = jsonEdu.type;
            edu.School = FacebookHelpers.ConvertToNameIdPair(jsonEdu.school); // it checks null
            edu.Degree = FacebookHelpers.ConvertToNameIdPair(jsonEdu.degree); // it checks null

            if (jsonEdu.concentration != null && jsonEdu.concentration.Count > 0)
            {
                // get the first one only...
                edu.Concentration = FacebookHelpers.ConvertToNameIdPair(jsonEdu.concentration[0]); // it checks null
            }

            NameLongIdPair year = FacebookHelpers.ConvertToNameIdPair(jsonEdu.year);
            if (year != null && !String.IsNullOrWhiteSpace(year.Name))
            {
                int y = 0;
                if (Int32.TryParse(year.Name, out y))
                {
                    edu.Year = y;
                }
            }

            return edu;
        }
    }

    public partial class EducationHistory : List<Education>
    {
        public EducationHistory() : base() { }

        public EducationHistory(List<Education> items) : base(items) { }

        public EducationLevel SortAndGetHighestEducationLevel()
        {
            this.Sort(Education.ComparisonByEducationLevel);
            this.Reverse();

            if (this.Count > 0)
            {
                return this[0].EducationLevel;
            }

            return EducationLevel.Unknown;
        }

        // from oldest to newest by default. we WILL change the order
        public static EducationHistory CreateFromFacebookArray(dynamic eduArray)
        {
            EducationHistory history = new EducationHistory();
            if (eduArray == null || eduArray.Count == 0)
            {
                // return empty
                return history;
            }

            for (int x = 0; x < eduArray.Count; x++)
            {
                Education edu = Education.CreateFromFacebookEdu(eduArray[x]);
                if (edu != null)
                {
                    history.Add(edu);
                }
            }

            return history;
        }
    }
}
