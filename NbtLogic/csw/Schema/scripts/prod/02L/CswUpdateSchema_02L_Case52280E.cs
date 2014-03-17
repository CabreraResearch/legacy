using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52280E : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "Obsolete == false for existing Materials; Set Containers to pending update"; }
        }

        public override string AppendToScriptName()
        {
            return "E";
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                // Set Obsolete to false for all existing materials
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    foreach( CswNbtNode CurrentNode in MaterialNT.getNodes( false, false ) )
                    {
                        CswNbtNodePropWrapper ObsoletePropWrapper = CurrentNode.Properties[CswNbtPropertySetMaterial.PropertyName.Obsolete];
                        if( null != ObsoletePropWrapper )
                        {
                            ObsoletePropWrapper.AsLogical.Checked = CswEnumTristate.False;
                            CurrentNode.postChanges( false );
                        }
                    }
                }
            }

            // Set all containers to be pending update
            CswTableUpdate containerNodesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "case52280_setContainersPendingUpdate", "nodes" );
            DataTable NodesTable = containerNodesUpdate.getTable( @"where pendingupdate = '0' 
                                                                     and nodetypeid in (select nodetypeid from nodetypes 
                                                                                         where objectclassid = (select objectclassid from object_class 
                                                                                                                 where objectclass = '" + CswEnumNbtObjectClass.ContainerClass + "'))" );
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                NodesRow["pendingupdate"] = "1";
            }
            containerNodesUpdate.update( NodesTable );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema