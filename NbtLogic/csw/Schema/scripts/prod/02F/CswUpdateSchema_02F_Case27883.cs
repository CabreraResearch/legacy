using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case27883 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27993; }
        }

        public override string ScriptName
        {
            get { return "02F_Case27883_B"; }
        }

        public override void update()
        {
            //Set the default value for the Available Work Units prop
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, true, IncludeHiddenNodes: true ) )
            {
                CswPrimaryKey WorkUnitId = UserNode.WorkUnitId;
                if( null == WorkUnitId )
                {
                    WorkUnitId = UserNode.GetFirstAvailableWorkUnitNodeId();
                }

                CswNbtNode WorkUnitNode = _CswNbtSchemaModTrnsctn.Nodes[WorkUnitId];
                UserNode.AvailableWorkUnits.AddValue( WorkUnitId.ToString() );
                UserNode.postChanges( false );
            }

            //Move the Available Work Units prop the the Profile tab
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab firstTab = UserNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp AvailWorkUnitsNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.AvailableWorkUnits );
                AvailWorkUnitsNTP.removeFromAllLayouts();
                AvailWorkUnitsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId );

                CswNbtMetaDataNodeTypeTab profileTab = UserNT.getNodeTypeTab( "Profile" );
                CswNbtMetaDataNodeTypeProp WorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                WorkUnitNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, profileTab.TabId );
            }

            //Remove extra white space from Work Units name template
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            foreach( CswNbtMetaDataNodeType WorkUnitNT in WorkUnitOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NameNTP = WorkUnitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassWorkUnit.PropertyName.Name );
                WorkUnitNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( NameNTP.PropName ) );
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema