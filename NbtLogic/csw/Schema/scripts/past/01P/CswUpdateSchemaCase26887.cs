using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26887
    /// </summary>
    public class CswUpdateSchemaCase26887 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass materialSubclassOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "MaterialSubclassClass" );
            if( null != materialSubclassOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( materialSubclassOC );
            }

        }//Update()

    }//class CswUpdateSchemaCase26887

}//namespace ChemSW.Nbt.Schema