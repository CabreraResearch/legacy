using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public enum AppVenue { Generic, NBT }
    public abstract class CswScmUpdt_TstCse
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set
            {
                _CswNbtSchemaModTrnsctn = value;
                _CswScmUpdt_TestTools.CswNbtSchemaModTrnsctn = value;
            }//
        }
        private string _Name = string.Empty;
        protected CswScmUpdt_TestTools _CswScmUpdt_TestTools = null;
        public string Name { get { return ( _Name ); } }
        public CswScmUpdt_TstCse( string Name )
        {
            _Name = Name;
            _CswScmUpdt_TestTools = new CswScmUpdt_TestTools();
        }//ctor


        protected AppVenue _AppVenue = AppVenue.Generic;
        public AppVenue AppVenue
        {
            get
            {
                return ( _AppVenue );
            }//get
        }//AppVenue


        public abstract void runTest();
    }//CswSchemaUpdaterTestCase

}//ChemSW.Nbt.Schema
