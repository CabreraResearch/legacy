using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28557
    /// </summary>
    public class CswUpdateSchema_01W_Case28557 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28557; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false ) )
            {
                if( RoleNode.Name.Text == "CISPro_Receiver" )
                {
                    CswNbtMetaDataObjectClass MaterialDocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
                    foreach (CswNbtMetaDataNodeType MaterialDocumentNT in MaterialDocumentOC.getNodeTypes())
                    {
                        if( MaterialDocumentNT.NodeTypeName == "Material Document" )
                        {
                            _CswNbtSchemaModTrnsctn.Permit.set(CswNbtPermit.NodeTypePermission.Create, MaterialDocumentNT, RoleNode, true);
                            _CswNbtSchemaModTrnsctn.Permit.set(CswNbtPermit.NodeTypePermission.Edit, MaterialDocumentNT, RoleNode, true);
                        }
                    }

                }
            }
        }//Update()

    }//class CswUpdateSchemaCase_01W_28557

}//namespace ChemSW.Nbt.Schema