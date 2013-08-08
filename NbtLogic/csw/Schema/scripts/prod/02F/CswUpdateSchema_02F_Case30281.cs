﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30281 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30281; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Container_Expiration_Lock, false, String.Empty, "Containers" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswEnumNbtModuleName.Containers, CswEnumNbtActionName.Container_Expiration_Lock );

            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true ) )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Container_Expiration_Lock, RoleNode.Node, RoleNode.Administrator.Checked == CswEnumTristate.True );
            }

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ExpirationLockedNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationLocked );
                ExpirationLockedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema