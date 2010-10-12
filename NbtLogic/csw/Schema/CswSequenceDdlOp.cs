using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
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
        public CswSequenceDdlOp( string SequenceNameIn, string PrependIn, string PostpendIn, string PadIn, Int32 InitialValueIn )
        {
            SequenceName = SequenceNameIn;
            Prepend = PrependIn;
            Postpend = PostpendIn;
            Pad = PadIn;
            InitialValue = InitialValueIn;
        }//ctor

        public string SequenceName;
        public string Prepend;
        public string Postpend;
        public string Pad;
        public Int32 InitialValue = Int32.MinValue;
        public DdlProcessStatus DdlProcessStatus;


    }//class CswSequenceDdlOp

}//ChemSW.Nbt.Schema
