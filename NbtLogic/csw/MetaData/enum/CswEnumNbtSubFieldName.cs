using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Names of SubFields of Properties
    /// There are SERIOUS repercussions if these are changed!  Beware!  (e.g. import)
    /// </summary>
    public sealed class CswEnumNbtSubFieldName : CswEnum<CswEnumNbtSubFieldName>
    {
        private CswEnumNbtSubFieldName( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtSubFieldName> _All { get { return CswEnum<CswEnumNbtSubFieldName>.All; } }
        public static explicit operator CswEnumNbtSubFieldName( string str )
        {
            CswEnumNbtSubFieldName ret = Parse( str );
            return ( ret != null ) ? ret : CswEnumNbtSubFieldName.Unknown;
        }
        public static readonly CswEnumNbtSubFieldName Unknown = new CswEnumNbtSubFieldName( "Unknown" );

        //public static readonly CswEnumNbtSubFieldNameTemp AllowedAnswers = new CswEnumNbtSubFieldNameTemp( "AllowedAnswers" );
        public static readonly CswEnumNbtSubFieldName Answer = new CswEnumNbtSubFieldName( "Answer" );
        public static readonly CswEnumNbtSubFieldName Barcode = new CswEnumNbtSubFieldName( "Barcode" );
        public static readonly CswEnumNbtSubFieldName Blob = new CswEnumNbtSubFieldName( "Blob" );
        public static readonly CswEnumNbtSubFieldName Checked = new CswEnumNbtSubFieldName( "Checked" );
        public static readonly CswEnumNbtSubFieldName Column = new CswEnumNbtSubFieldName( "Column" );
        public static readonly CswEnumNbtSubFieldName Comments = new CswEnumNbtSubFieldName( "Comments" );
        //public static readonly CswEnumNbtSubFieldNameTemp CompliantAnswers = new CswEnumNbtSubFieldNameTemp( "CompliantAnswers" );
        public static readonly CswEnumNbtSubFieldName ContentType = new CswEnumNbtSubFieldName( "ContentType" );
        public static readonly CswEnumNbtSubFieldName CorrectiveAction = new CswEnumNbtSubFieldName( "CorrectiveAction" );
        public static readonly CswEnumNbtSubFieldName DateAnswered = new CswEnumNbtSubFieldName( "DateAnswered" );
        public static readonly CswEnumNbtSubFieldName DateCorrected = new CswEnumNbtSubFieldName( "DateCorrected" );
        public static readonly CswEnumNbtSubFieldName Href = new CswEnumNbtSubFieldName( "Href" );
        public static readonly CswEnumNbtSubFieldName Icon = new CswEnumNbtSubFieldName( "Icon" );
        public static readonly CswEnumNbtSubFieldName Id = new CswEnumNbtSubFieldName( "Id" );
        public static readonly CswEnumNbtSubFieldName Image = new CswEnumNbtSubFieldName( "Image" );
        public static readonly CswEnumNbtSubFieldName Interval = new CswEnumNbtSubFieldName( "Interval" );
        public static readonly CswEnumNbtSubFieldName IsCompliant = new CswEnumNbtSubFieldName( "IsCompliant" );
        public static readonly CswEnumNbtSubFieldName Mol = new CswEnumNbtSubFieldName( "Mol" );
        public static readonly CswEnumNbtSubFieldName Name = new CswEnumNbtSubFieldName( "Name" );
        public static readonly CswEnumNbtSubFieldName NodeID = new CswEnumNbtSubFieldName( "NodeID" );
        public static readonly CswEnumNbtSubFieldName NodeType = new CswEnumNbtSubFieldName( "NodeType" );
        public static readonly CswEnumNbtSubFieldName Number = new CswEnumNbtSubFieldName( "Number" );
        public static readonly CswEnumNbtSubFieldName Options = new CswEnumNbtSubFieldName( "Options" );
        public static readonly CswEnumNbtSubFieldName Password = new CswEnumNbtSubFieldName( "Password" );
        public static readonly CswEnumNbtSubFieldName Path = new CswEnumNbtSubFieldName( "Path" );
        //public static readonly CswEnumNbtSubFieldNameTemp Required = new CswEnumNbtSubFieldNameTemp( "Required" );
        public static readonly CswEnumNbtSubFieldName Row = new CswEnumNbtSubFieldName( "Row" );
        public static readonly CswEnumNbtSubFieldName Sequence = new CswEnumNbtSubFieldName( "Sequence" );
        public static readonly CswEnumNbtSubFieldName StartDateTime = new CswEnumNbtSubFieldName( "StartDateTime" );
        public static readonly CswEnumNbtSubFieldName Text = new CswEnumNbtSubFieldName( "Text" );
        public static readonly CswEnumNbtSubFieldName Type = new CswEnumNbtSubFieldName( "Type" );
        public static readonly CswEnumNbtSubFieldName Units = new CswEnumNbtSubFieldName( "Units" );
        public static readonly CswEnumNbtSubFieldName Value = new CswEnumNbtSubFieldName( "Value" );
        public static readonly CswEnumNbtSubFieldName ViewID = new CswEnumNbtSubFieldName( "ViewID" );
        public static readonly CswEnumNbtSubFieldName ChangedDate = new CswEnumNbtSubFieldName( "ChangedDate" );
        public static readonly CswEnumNbtSubFieldName Base = new CswEnumNbtSubFieldName( "Base" );
        public static readonly CswEnumNbtSubFieldName Exponent = new CswEnumNbtSubFieldName( "Exponent" );
        public static readonly CswEnumNbtSubFieldName Health = new CswEnumNbtSubFieldName( "Health" );
        public static readonly CswEnumNbtSubFieldName Flammability = new CswEnumNbtSubFieldName( "Flammability" );
        public static readonly CswEnumNbtSubFieldName Reactivity = new CswEnumNbtSubFieldName( "Reactivity" );
        public static readonly CswEnumNbtSubFieldName Special = new CswEnumNbtSubFieldName( "Special" );
    } // class SubFieldName

}//namespace ChemSW.Nbt.MetaData
