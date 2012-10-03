using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public abstract class CswUpdateSchemaTo
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        //        public abstract CswSchemaVersion SchemaVersion { get; }
        //public abstract string Description { set; get; }

        private string _Description = string.Empty;

        public virtual string Description
        {
            set { _Description = value; }
            get { return ( _Description ); }
        }

        /// <summary>
        /// The logic to execute in each Schema Script
        /// </summary>
        public abstract void update();

        /// <summary>
        /// The author of the script
        /// </summary>
        public abstract CswDeveloper Author { get; }

        /// <summary>
        /// The FogBugz Case number associated with this script
        /// </summary>
        public abstract Int32 CaseNo { get; }

        /// <summary>
        /// Get the Case number as a link to FogBugz
        /// </summary>
        public string getCaseLink()
        {
            string Ret = string.Empty;
            if( CaseNo > 0 )
            {
                Ret = @"<a href=""https://fogbugz.chemswlive.com/default.asp?" + CaseNo + @""">Case " + CaseNo + "</a>";
    }
            else
            {
                Ret = "No case link for case " + CaseNo;
}
            return Ret;
        }
    }
}
