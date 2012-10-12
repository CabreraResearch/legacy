using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtWcfInspectionsDataModel
    {
        [DataMember]
        public Collection<CswNbtInspectionDesign> Designs;

        [DataMember]
        public Collection<CswNbtInspection> Inspections;

        public CswNbtWcfInspectionsDataModel()
        {
            Designs = new Collection<CswNbtInspectionDesign>();
            Inspections = new Collection<CswNbtInspection>();
        }

        public class CswNbtInspectionDesign
        {
            public CswNbtInspectionDesign()
            {
                Sections = new Collection<Section>();
            }

            public Int32 DesignId { get; set; }
            public string Name { get; set; }
            public Collection<Section> Sections { get; set; }

            public class Section
            {
                public Section()
                {
                    Properties = new Collection<SectionProperty>();
                }

                public Int32 SectionId { get; set; }
                public string Name { get; set; }
                public Int32 Order { get; set; }
                public bool ReadOnly { get; set; }
                public Collection<SectionProperty> Properties { get; set; }
            }

            public class AnswerChoice
            {
                public string Text { get; set; }
                public bool IsCompliant { get; set; }
            }

            public class SectionProperty
            {
                public SectionProperty()
                {
                    Choices = new Collection<AnswerChoice>();
                }

                public Collection<AnswerChoice> Choices { get; set; }
                public Int32 QuestionId { get; set; }
                public string Text { get; set; }
                public string Type { get; set; }
                public string HelpText { get; set; }
                public string PreferredAnswer { get; set; }
                public bool ReadOnly { get; set; }
            }
        }

        public class CswNbtInspection
        {
            public CswNbtInspection()
            {
                Questions = new Collection<QuestionAnswer>();
            }

            public Int32 DesignId { get; set; }
            public DateTime DueDate { get; set; }
            public Int32 InspectionId { get; set; }
            public string InspectionPointName { get; set; }
            public string LocationPath { get; set; }
            public string RouteName { get; set; }
            public string Status { get; set; }
            public string Action { get; set; }
            public bool ReadOnly { get; set; }
            public QuestionCounts Counts { get; set; }
            public Collection<QuestionAnswer> Questions { get; set; }

            public class QuestionCounts
            {
                public QuestionCounts()
                {
                    Total = 0;
                    Answered = 0;
                    UnAnswered = 0;
                    Ooc = 0;
                }
                public Int32 Total { get; set; }
                public Int32 Answered { get; set; }
                public Int32 UnAnswered { get; set; }
                public Int32 Ooc { get; set; }
            }

            public class QuestionAnswer
            {
                public string Answer { get; set; }
                public Int32 AnswerId { get; set; }
                public string Comments { get; set; }
                public string CorrectiveAction { get; set; }
                public DateTime DateAnswered { get; set; }
                public DateTime DateCorrected { get; set; }
                public Int32 QuestionId { get; set; }
                public string Status { get; set; }
            }
        }
    }
}