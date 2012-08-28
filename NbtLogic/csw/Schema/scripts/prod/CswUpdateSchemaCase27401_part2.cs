using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27401 (part 2)
    /// </summary>
    public class CswUpdateSchemaCase27401_part2 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp materialOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );

            //make this server managed
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( materialOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeProp materialNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( sizeNT.NodeTypeId, materialOCP.PropId );
                materialNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }


        }//Update()

    }

}//namespace ChemSW.Nbt.Schema