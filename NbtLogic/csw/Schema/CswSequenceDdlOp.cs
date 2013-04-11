using System;
using ChemSW.Core;
//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{

    public enum DdlSequenceOpType { Unknown, Add, Remove };
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    /// 
    public class CswSequenceDdlOp
    {
        public DdlSequenceOpType DdlSequenceOpType = DdlSequenceOpType.Unknown;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswSequenceDdlOp( CswSequenceName SequenceNameIn, string PrependIn, string PostpendIn, Int32 PadIn, Int32 InitialValueIn )
        {
            SequenceName = SequenceNameIn;
            Prepend = PrependIn;
            Postpend = PostpendIn;
            Pad = PadIn;
            InitialValue = InitialValueIn;
        }//ctor

        public CswSequenceName SequenceName;
        public string Prepend;
        public string Postpend;
        public Int32 Pad;
        public Int32 InitialValue = Int32.MinValue;
        public CswEnumDdlProcessStatus DdlProcessStatus;


    }//class CswSequenceDdlOp

}//ChemSW.Nbt.Schema
