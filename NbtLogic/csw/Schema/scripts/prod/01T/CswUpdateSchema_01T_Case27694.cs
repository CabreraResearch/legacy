using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27694
    /// </summary>
    public class CswUpdateSchema_01T_Case27694 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType reqItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
            CswNbtMetaDataNodeType invGrpNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( null != reqItemNT && null != invGrpNT )
            {
                CswNbtMetaDataNodeTypeProp invGrpNTP = reqItemNT.getNodeTypeProp( "Inventory Group" );
                if( null != invGrpNTP )
                {
                    foreach( CswNbtObjClassInventoryGroup invGrpNode in invGrpNT.getNodes( false, false ) )
                    {
                        if( invGrpNode.Name.Text.Equals( "Default Inventory Group" ) )
                        {
                            invGrpNTP.DefaultValue.AsRelationship.RelatedNodeId = invGrpNode.NodeId;
                            invGrpNTP.DefaultValue.AsRelationship.CachedNodeName = invGrpNode.Name.Text;
                        }
                    }
                }
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27694; }
        }

        //Update()

    }//class CswUpdateSchemaCaseCswUpdateSchema_01T_Case27694

}//namespace ChemSW.Nbt.Schema