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
                IEnumerable<CswNbtMetaDataNodeTypeTab> TaskNodeTypeTabs = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeTabs( TaskNodeType.NodeTypeId );
                int TabIdToAdd = 0;
                int TabIndex = 0;
                foreach( CswNbtMetaDataNodeTypeTab TaskNodeTypeTab in TaskNodeTypeTabs )
                {
                    PartsProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, TabId: TaskNodeTypeTab.TabId );
                    if( 0 == TabIndex || TaskNodeTypeTab.TabName.Equals( "Completion" ) )
                    {
                        TabIdToAdd = TaskNodeTypeTab.TabId;
                    }
                    TabIndex++;
                }
                PartsProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, TabId: TabIdToAdd );
            }

        }//Update()
    }//class CswUpdateSchemaCase26094B
}//namespace ChemSW.Nbt.Schema