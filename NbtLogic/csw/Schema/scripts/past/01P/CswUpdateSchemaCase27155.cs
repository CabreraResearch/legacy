using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27155
    /// </summary>
    public class CswUpdateSchemaCase27155 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass roleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );

            CswNbtMetaDataObjectClassProp actionPermissionsOCP = roleOC.getObjectClassProp( CswNbtObjClassRole.ActionPermissionsPropertyName );
            if( null != actionPermissionsOCP )
            {
                foreach( CswNbtMetaDataNodeTypeProp permissions in actionPermissionsOCP.getNodeTypeProps() )
                {
                    permissions.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );
                }
            }

            CswNbtMetaDataObjectClassProp nodeTypePermissionsOCP = roleOC.getObjectClassProp( CswNbtObjClassRole.NodeTypePermissionsPropertyName );
            if( null != nodeTypePermissionsOCP )
            {
                foreach( CswNbtMetaDataNodeTypeProp permissions in nodeTypePermissionsOCP.getNodeTypeProps() )
                {
                    permissions.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27155

}//namespace ChemSW.Nbt.Schema