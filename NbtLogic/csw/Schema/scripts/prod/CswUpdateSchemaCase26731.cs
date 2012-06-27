using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26731
    /// </summary>
    public class CswUpdateSchemaCase26731 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Remove extra "physical_state" property

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass);
            CswNbtMetaDataObjectClassProp Physical_StateOCP = MaterialOC.getObjectClassProp("physical_state");
            if( Physical_StateOCP != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( Physical_StateOCP, true );
            }

        }//Update()

    }//class CswUpdateSchemaCase26731

}//namespace ChemSW.Nbt.Schema