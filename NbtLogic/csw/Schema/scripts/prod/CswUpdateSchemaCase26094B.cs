using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26094B
    /// </summary>
    public class CswUpdateSchemaCase26094B : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );

            IEnumerable<CswNbtMetaDataNodeType> TaskNodeTypes = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
            foreach( CswNbtMetaDataNodeType TaskNodeType in TaskNodeTypes )
            {
                CswNbtMetaDataNodeTypeProp PartsProp = TaskNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PartsPropertyName );
                CswNbtMetaDataNodeTypeTab TaskNodeTypeTab = TaskNodeType.getNodeTypeTab( "Completion" );
                if( null == TaskNodeTypeTab )
                {
                    TaskNodeTypeTab = TaskNodeType.getFirstNodeTypeTab();
                }
                if( TaskNodeTypeTab != null )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( PartsProp );
                    PartsProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, TaskNodeTypeTab.TabId );
                }
            }

        }//Update()
    }//class CswUpdateSchemaCase26094B
}//namespace ChemSW.Nbt.Schema