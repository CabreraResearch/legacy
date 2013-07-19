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
    public class CswUpdateSchema_02D_Case29863_UserProfile : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29863; }
        }

        public override void update()
        {
            try
            {
                CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.UserClass, TabName: "Profile", LayoutType: CswEnumNbtLayoutType.Edit );

                //User tab
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.FavoriteActions );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.FavoriteViews );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.DateFormat );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.TimeFormat );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.Language );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.DefaultLocation );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.DefaultPrinter );
                LayoutMgr.removePropFromLayoutFirstTab( PropName: CswNbtObjClassUser.PropertyName.PageSize );

                LayoutMgr.movePropOnFirstTab( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr.movePropOnFirstTab( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr.movePropOnFirstTab( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr.movePropOnFirstTab( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr.movePropOnFirstTab( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr.movePropOnFirstTab( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr.movePropOnFirstTab( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );

                LayoutMgr.movePropOnFirstTab( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Role );
                LayoutMgr.movePropOnFirstTab( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.WorkUnit );
                LayoutMgr.movePropOnFirstTab( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.EmployeeId );
                LayoutMgr.movePropOnFirstTab( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Barcode );
                LayoutMgr.movePropOnFirstTab( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.LastLogin );
                LayoutMgr.movePropOnFirstTab( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.AccountLocked );
                LayoutMgr.movePropOnFirstTab( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FailedLoginCount );
                LayoutMgr.movePropOnFirstTab( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Archived );

                //Identity tab
                LayoutMgr.copyPropToIdentityTab( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr.copyPropToIdentityTab( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Barcode );

                //Profile tab
                LayoutMgr.copyPropToTab( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr.copyPropToTab( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr.copyPropToTab( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr.copyPropToTab( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr.copyPropToTab( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr.copyPropToTab( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr.copyPropToTab( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.PageSize );

                LayoutMgr.copyPropToTab( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteActions );
                LayoutMgr.copyPropToTab( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteViews );
                LayoutMgr.copyPropToTab( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DateFormat );
                LayoutMgr.copyPropToTab( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.TimeFormat );
                LayoutMgr.copyPropToTab( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Language );
                LayoutMgr.copyPropToTab( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );
                LayoutMgr.copyPropToTab( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultLocation );
                LayoutMgr.copyPropToTab( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultPrinter );

                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                Collection<CswNbtObjClassRole> Roles = new Collection<CswNbtObjClassRole>();
                foreach( CswNbtObjClassRole Role in RoleOC.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false ) )
                {
                    Roles.Add( Role );
                }
                LayoutMgr.setTabPermission( CswEnumNbtNodeTypeTabPermission.Edit, true, Roles );

            }
            catch( Exception Ex )
            {
                throw new CswDniException("User layout update failed.");
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema