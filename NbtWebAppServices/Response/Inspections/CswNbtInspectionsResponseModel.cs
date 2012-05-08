using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NbtWebAppServices.Response
{
    [DataContract]
    public class CswNbtInspectionsResponseModel
    {
        [DataMember]
        public CswNbtInspectionDesignsCollection Designs;

        [DataMember]
        public CswNbtInspectionsCollection Inspections;

        public CswNbtInspectionsResponseModel()
        {
            Designs = new CswNbtInspectionDesignsCollection();
            Inspections = new CswNbtInspectionsCollection();
        }

        [DataContract]
        public class CswNbtInspectionDesignsCollection
        {
            [DataMember]
            public Collection<CswNbtInspectionDesign> Designs;

            public CswNbtInspectionDesignsCollection()
            {
                Designs = new Collection<CswNbtInspectionDesign>();
            }

            public class CswNbtInspectionDesign
            {
                public CswNbtInspectionDesign()
                {
                    Sections = new Collection<CswNbtInspectionDesignSection>();
                }

                public Int32 DesignId { get; set; }
                public string Name { get; set; }
                public Collection<CswNbtInspectionDesignSection> Sections { get; set; }
            }

            public class CswNbtInspectionDesignSection
            {
                public CswNbtInspectionDesignSection()
                {
                    Properties = new Collection<CswNbtInspectionDesignSectionProperty>();
                }
                public Int32 SectionId { get; set; }
                public string Name { get; set; }
                public Int32 Order { get; set; }
                public Collection<CswNbtInspectionDesignSectionProperty> Properties { get; set; }
            }

            public class CswNbtInspectionDesignSectionProperty
            {
                public CswNbtInspectionDesignSectionProperty()
                {
                    Choices = new Collection<string>();
                }
                public Collection<string> Choices { get; set; }
                public Int32 QuestionId { get; set; }
                public string Text { get; set; }
                public string Type { get; set; }
                public string HelpText { get; set; }
            }
        }

        [DataContract]
        public class CswNbtInspectionsCollection
        {
            [DataMember]
            public Collection<CswNbtInspection> Inspections;

            public CswNbtInspectionsCollection()
            {
                Inspections = new Collection<CswNbtInspection>();
            }

            public class CswNbtInspection
            {
                public CswNbtInspection()
                {
                    Questions = new Collection<CswNbtInspectionQuestion>();
                }

                public Int32 DesignId { get; set; }
                public DateTime DueDate { get; set; }
                public Int32 InspectionId { get; set; }
                public string InspectionPointName { get; set; }
                public string LocationPath { get; set; }
                public string RouteName { get; set; }
                public string Status { get; set; }
                public Collection<CswNbtInspectionQuestion> Questions { get; set; }
            }

            public class CswNbtInspectionQuestion
            {
                public string Answer { get; set; }
                public Int32 AnswerId { get; set; }
                public string Comments { get; set; }
                public string CorrectiveAction { get; set; }
                public DateTime LastModifyDate { get; set; }
                public Int32 LastModifyUserId { get; set; }
                public string LastModifyUserName { get; set; }
                public Int32 QuestionId { get; set; }
                public string Status { get; set; }
            }

        }


    }
}