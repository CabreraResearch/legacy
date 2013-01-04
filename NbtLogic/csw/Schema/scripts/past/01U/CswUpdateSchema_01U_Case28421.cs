using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28421
    /// </summary>
    public class CswUpdateSchema_01U_Case28421 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28421; }
        }

        public override void update()
        {
            // this is also in CswNbtModuleRuleCISPro
            if( false == _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp LocationContainersGridNTP = LocationNT.getNodeTypeProp( "Containers" );
                    if( null != LocationContainersGridNTP )
                    {
                        LocationContainersGridNTP.removeFromAllLayouts();
                        CswNbtMetaDataNodeTypeTab LocationContainersTab = LocationNT.getNodeTypeTab( "Containers" );
                        if( LocationContainersTab != null )
                        {
                            _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( LocationContainersTab );
                        }
                    }
                }
            }
        } //Update()

    }//class CswUpdateSchemaCase28421

}//namespace ChemSW.Nbt.Schema