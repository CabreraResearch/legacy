using System.Data;
using System.Collections.Generic;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case28899_01 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28899; }
        }

        public override void update()
        {
            //Create action
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Delete_Demo_Data, true, "", "System" );

        } // update()

    }//class  CswUpdateSchema_02B_Case28899_01

}//namespace ChemSW.Nbt.Schema