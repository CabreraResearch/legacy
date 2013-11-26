using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30941 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30941; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override string Title
        {
            get { return "Give CISPro_Receiver Create Material Permission"; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswNbtObjClassRole CISProReceiverRole = null;
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false, false, true ) )
                {
                    if( RoleNode.Name.Text == "CISPro_Receiver" )
                    {
                        CISProReceiverRole = RoleNode;
                        break;
                    }
                }
                if( null != CISProReceiverRole )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Create_Material, CISProReceiverRole, true );
                    CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
                    foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
                    {
                        foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                        {
                            _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MaterialNT, CISProReceiverRole, true );
                            _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialNT, CISProReceiverRole, true );
                            _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, MaterialNT, CISProReceiverRole, true );
                            _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, MaterialNT, CISProReceiverRole, true );
                        }
                    }
                }
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema