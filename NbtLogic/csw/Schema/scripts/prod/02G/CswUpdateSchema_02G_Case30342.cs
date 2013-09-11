using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30342 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Site is no longer demo data"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30342; }
        }

        public override string ScriptName
        {
            get { return "Case30342Nodes"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType SiteNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
            if( null != SiteNt && SiteNt.getObjectClass().ObjectClass == CswEnumNbtObjectClass.LocationClass )
            {
                foreach( CswNbtObjClassLocation SiteNode in SiteNt.getNodes(true, false, false, false) )
                {
                    if( SiteNode.Name.Text == "Site 1" )
                    {
                        SiteNode.IsDemo = false;
                        //SiteNode.Name.Text = "Default Site"; //Sitename cannot be changed do to unique constraint.
                        SiteNode.postChanges( ForceUpdate: false );
                        break;
                    }
                }
            }

            CswNbtMetaDataObjectClass InventoryGroupClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            foreach( CswNbtObjClassInventoryGroup InventoryGroup in InventoryGroupClass.getNodes(true, false, false, false) )
            {
                if( InventoryGroup.Name.Text == "Default Inventory Group" )
                {
                    InventoryGroup.IsDemo = false;
                    InventoryGroup.postChanges( ForceUpdate: false );
                    break;
                }
            }

            CswNbtMetaDataObjectClass WorkUnitClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            foreach( CswNbtObjClassWorkUnit WorkUnit in WorkUnitClass.getNodes( true, false, false, false ) )
            {
                if( WorkUnit.Name.Text == "Default Work Unit" )
                {
                    WorkUnit.IsDemo = false;
                    WorkUnit.postChanges( ForceUpdate: false );
                    break;
                }
            }


        }
    }
}