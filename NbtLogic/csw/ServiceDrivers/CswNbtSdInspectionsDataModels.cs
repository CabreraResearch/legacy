using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{

    public class CswNbtSdInspectionsDataModels
    {
        [DataContract]
        public class InspectionData
        {
            [DataMember]
            public Collection<CswNbtInspectionDesign> Designs;

            [DataMember]
            public Collection<CswNbtInspection> Inspections;

            public InspectionData()
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

            [DataContract]
            public class CswNbtInspection
            {
                public CswNbtInspection()
                {
                    Questions = new Collection<QuestionAnswer>();
                }

                public DateTime DueDateAsDate
                {
                    get { return DateTime.Parse( DueDate ); }
                    set { DueDate = value.ToShortDateString(); }
                }

                [DataMember]
                public Int32 DesignId { get; set; }

                [DataMember]
                public string DueDate { get; set; }

                [DataMember]
                public Int32 InspectionId { get; set; }

                [DataMember]
                public string InspectionPointName { get; set; }

                [DataMember]
                public string LocationPath { get; set; }

                [DataMember]
                public string RouteName { get; set; }

                [DataMember]
                public string Status { get; set; }

                [DataMember]
                public string Action { get; set; }

                [DataMember]
                public bool ReadOnly { get; set; }

                [DataMember]
                public QuestionCounts Counts { get; set; }

                [DataMember]
                public Collection<QuestionAnswer> Questions { get; set; }

                [DataContract]
                public class QuestionCounts
                {
                    public QuestionCounts()
                    {
                        Total = 0;
                        Answered = 0;
                        UnAnswered = 0;
                        Ooc = 0;
                    }

                    [DataMember]
                    public Int32 Total { get; set; }

                    [DataMember]
                    public Int32 Answered { get; set; }

                    [DataMember]
                    public Int32 UnAnswered { get; set; }

                    [DataMember]
                    public Int32 Ooc { get; set; }
                }

                [DataContract]
                public class QuestionAnswer
                {
                    [DataMember]
                    public string Answer { get; set; }

                    [DataMember]
                    public Int32 AnswerId { get; set; }

                    [DataMember]
                    public string Comments { get; set; }

                    [DataMember]
                    public string CorrectiveAction { get; set; }

                    [DataMember]
                    public DateTime DateAnswered { get; set; }

                    [DataMember]
                    public DateTime DateCorrected { get; set; }

                    [DataMember]
                    public Int32 QuestionId { get; set; }

                    [DataMember]
                    public string Status { get; set; }
                }
            }
        }


        [DataContract]
        [Description( "Returns Inspections submitted for update which require further attention. Completed Inspections are not returned." )]
        public class InspectionUpdateData
        {
            public InspectionUpdateData()
            {
                ActionRequired = new Collection<InspectionData.CswNbtInspection>();
                InComplete = new Collection<InspectionData.CswNbtInspection>();
                Failed = new Collection<InspectionData.CswNbtInspection>();
                Completed = new Collection<InspectionData.CswNbtInspection>();
                Cancelled = new Collection<InspectionData.CswNbtInspection>();
                Missed = new Collection<InspectionData.CswNbtInspection>();
            }

            [DataMember]
            [Description( "Inspections which were successfully updated but are now Action Required." )]
            public Collection<InspectionData.CswNbtInspection> ActionRequired { get; set; }

            [DataMember]
            [Description( "Inspections which were successfully updated but are not Completed." )]
            public Collection<InspectionData.CswNbtInspection> InComplete { get; set; }

            [DataMember]
            [Description( "Inspections which were submitted for update but failed." )]
            public Collection<InspectionData.CswNbtInspection> Failed { get; set; }

            [DataMember]
            [Description( "Inspections which were submitted for update but were already completed by another user." )]
            public Collection<InspectionData.CswNbtInspection> Completed { get; set; }

            [DataMember]
            [Description( "Inspections which were submitted for update but were cancelled by another user." )]
            public Collection<InspectionData.CswNbtInspection> Cancelled { get; set; }

            [DataMember]
            [Description( "Inspections which were submitted for update but were missed." )]
            public Collection<InspectionData.CswNbtInspection> Missed { get; set; }

        }



    }
}