using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26757
    /// </summary>
    public class CswUpdateSchemaCase26757 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp materialSubclassOCP = materialOC.getObjectClassProp( "materialsubclass" );

            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( materialSubclassOCP, true );

        }//Update()

    }//class CswUpdateSchemaCase26757

}//namespace ChemSW.Nbt.Schema