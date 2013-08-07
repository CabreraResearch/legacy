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
                CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.UserClass, LayoutType: CswEnumNbtLayoutType.Edit );

                //User tab
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.FavoriteActions );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.FavoriteViews );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.DateFormat );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.TimeFormat );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.Language );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.DefaultLocation );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.DefaultPrinter );
                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.PageSize );

                LayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );

                LayoutMgr.First.moveProp( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Role );
                LayoutMgr.First.moveProp( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.WorkUnit );
                LayoutMgr.First.moveProp( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.EmployeeId );
                LayoutMgr.First.moveProp( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Barcode );
                LayoutMgr.First.moveProp( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.LastLogin );
                LayoutMgr.First.moveProp( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.AccountLocked );
                LayoutMgr.First.moveProp( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FailedLoginCount );
                LayoutMgr.First.moveProp( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Archived );

                //Profile tab
                LayoutMgr["Profile"].copyProp( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr["Profile"].copyProp( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr["Profile"].copyProp( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr["Profile"].copyProp( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr["Profile"].copyProp( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr["Profile"].copyProp( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr["Profile"].copyProp( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.PageSize );

                LayoutMgr["Profile"].copyProp( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteActions );
                LayoutMgr["Profile"].copyProp( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteViews );
                LayoutMgr["Profile"].copyProp( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DateFormat );
                LayoutMgr["Profile"].copyProp( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.TimeFormat );
                LayoutMgr["Profile"].copyProp( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Language );
                LayoutMgr["Profile"].copyProp( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );
                LayoutMgr["Profile"].copyProp( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultLocation );
                LayoutMgr["Profile"].copyProp( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultPrinter );

                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                Collection<CswNbtObjClassRole> Roles = new Collection<CswNbtObjClassRole>();
                foreach( CswNbtObjClassRole Role in RoleOC.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false ) )
                {
                    Roles.Add( Role );
                }
                LayoutMgr["Profile"].setPermit( CswEnumNbtNodeTypeTabPermission.Edit, true, Roles );

            }
            catch( Exception )
            {
                throw new CswDniException("User layout update failed.");
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema