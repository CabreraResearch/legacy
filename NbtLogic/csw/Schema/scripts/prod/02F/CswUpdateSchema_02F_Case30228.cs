using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30228: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30228; }
        }

        public override void update()
        {

            #region Update all NTPs "Hidden" column to "false"

            CswTableUpdate nodeTypePropsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntp.setHidden", "nodetype_props" );
            DataTable nodetypePropsDT = nodeTypePropsTU.getTable();
            foreach( DataRow row in nodetypePropsDT.Rows )
            {
                row["hidden"] = CswConvert.ToDbVal( false );
            }
            nodeTypePropsTU.update( nodetypePropsDT );

            #endregion

            #region CISPro

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                if( null != UserJurisdictionNTP )
                {
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: UserNT.getFirstNodeTypeTab().TabId );
                }
            }

            #endregion

        } // update()

    }

}//namespace ChemSW.Nbt.Schema