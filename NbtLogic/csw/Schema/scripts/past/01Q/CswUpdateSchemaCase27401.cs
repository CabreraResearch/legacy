using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27401
    /// </summary>
    public class CswUpdateSchemaCase27401 : CswUpdateSchemaTo
    {
        public override void update()
        {

            /*
             * CswUpdateSchemaCase25759 makes the list options for physical state have "n/a" 
             * but then sets the default state of supply NT as "N/A" which causes the initial quantity lists to not show correctly.
             * 
             * The case sensitivity also breaks the CreateMaterial wizard as it is looking for the lower case "n/a"
            */

            //set the supply nodetype default value to "n/a" 
            CswNbtMetaDataNodeType SupplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            if( SupplyNT != null )
            {
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp PhysicalStateOCP = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOC.ObjectClassId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
                CswNbtMetaDataNodeTypeProp PhysicalStateNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( SupplyNT.NodeTypeId, PhysicalStateOCP.ObjectClassPropId );
                PhysicalStateNTP.DefaultValue.AsList.Value = "n/a";
            }

        }//Update()

    }//class CswUpdateSchemaCase27401

}//namespace ChemSW.Nbt.Schema