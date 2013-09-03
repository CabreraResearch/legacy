using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using System;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30339_UserProfilex2 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30339; }
        }

        public override void update()
        {
            try
            {
                CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.UserClass, LayoutType: CswEnumNbtLayoutType.Edit );
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                
                //Step 1: Make sure all admins have Edit on Users. Revoke View for everyone else.
                {
                    CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                    foreach ( CswNbtMetaDataNodeType UserNt in UserOc.getLatestVersionNodeTypes() )
                    {
                        foreach( CswNbtObjClassRole Role in RoleOC.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false ) )
                        {
                            if( Role.Administrator.Checked == CswEnumTristate.True )
                            {
                                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypeTabPermission.Edit, UserNt, Role, value: true );
                            }
                            else
                            {
                                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypeTabPermission.View, UserNt, Role, value: false );
                            }
                        }
                    }
                    
                }
                //Step 2: Make sure everyone has edit on Profile.
                {
                    Collection<CswNbtObjClassRole> AllRolesRoles = new Collection<CswNbtObjClassRole>();

                    foreach ( CswNbtObjClassRole Role in RoleOC.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false ) )
                    {
                        AllRolesRoles.Add( Role );
                    }
                    LayoutMgr["Profile"].setPermit( CswEnumNbtNodeTypeTabPermission.Edit, true, AllRolesRoles );
                }

            }
            catch ( Exception )
            {
                throw new CswDniException( "User layout update failed." );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema