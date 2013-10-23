using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30400: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30400; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "_D"; }
        }

        public override string Title
        {
            get { return "User Property Layout Update"; }
        }

        public override void update()
        {
            try
            {
                CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.UserClass, LayoutType: CswEnumNbtLayoutType.Edit );

                LayoutMgr.First.removeProp( PropName: CswNbtObjClassUser.PropertyName.CostCode );

                //User Tab
                LayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Role );
                LayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassUser.PropertyName.CurrentWorkUnit );

                LayoutMgr.First.moveProp( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Barcode );
                LayoutMgr.First.moveProp( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.EmployeeId );
                LayoutMgr.First.moveProp( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.LastLogin );
                LayoutMgr.First.moveProp( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.AccountLocked );
                LayoutMgr.First.moveProp( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FailedLoginCount );
                LayoutMgr.First.moveProp( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Archived );
                LayoutMgr.First.moveProp( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );
                LayoutMgr.First.moveProp( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.AvailableWorkUnits );

                //Profile tab
                LayoutMgr["Profile"].copyProp( Row: 1, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Username );
                LayoutMgr["Profile"].copyProp( Row: 2, Column: 1, PropName: CswNbtObjClassUser.PropertyName.FirstName );
                LayoutMgr["Profile"].copyProp( Row: 3, Column: 1, PropName: CswNbtObjClassUser.PropertyName.LastName );
                LayoutMgr["Profile"].copyProp( Row: 4, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Phone );
                LayoutMgr["Profile"].copyProp( Row: 5, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Email );
                LayoutMgr["Profile"].copyProp( Row: 6, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Password );
                LayoutMgr["Profile"].copyProp( Row: 7, Column: 1, PropName: CswNbtObjClassUser.PropertyName.Role );
                LayoutMgr["Profile"].copyProp( Row: 8, Column: 1, PropName: CswNbtObjClassUser.PropertyName.CurrentWorkUnit );

                LayoutMgr["Profile"].copyProp( Row: 1, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteActions );
                LayoutMgr["Profile"].copyProp( Row: 2, Column: 2, PropName: CswNbtObjClassUser.PropertyName.FavoriteViews );
                LayoutMgr["Profile"].copyProp( Row: 3, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DateFormat );
                LayoutMgr["Profile"].copyProp( Row: 4, Column: 2, PropName: CswNbtObjClassUser.PropertyName.TimeFormat );
                LayoutMgr["Profile"].copyProp( Row: 5, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Language );
                LayoutMgr["Profile"].copyProp( Row: 6, Column: 2, PropName: CswNbtObjClassUser.PropertyName.Jurisdiction );
                LayoutMgr["Profile"].copyProp( Row: 7, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultLocation );
                LayoutMgr["Profile"].copyProp( Row: 8, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultPrinter );
                LayoutMgr["Profile"].copyProp( Row: 9, Column: 2, PropName: CswNbtObjClassUser.PropertyName.DefaultBalance );
                LayoutMgr["Profile"].copyProp( Row: 10, Column: 2, PropName: CswNbtObjClassUser.PropertyName.PageSize );
            }
            catch( Exception Ex )
            {
                throw new CswDniException( "User layout update failed.", Ex );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema