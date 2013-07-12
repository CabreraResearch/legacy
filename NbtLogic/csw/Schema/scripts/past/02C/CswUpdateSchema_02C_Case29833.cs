using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29833
    /// </summary>
    public class CswUpdateSchema_02C_Case29833 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29833; }
        }

        public override void update()
        {
            //NOTE - see RunBeforeEveryExecutionOfUpdater_Case29833.cs

            //CswNbtMetaDataNodeType SDSNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            //if( null != SDSNT )
            //{
            //    _migrateExistingSDSDocNT( SDSNT );
            //}
            //else//If the existing "SDS Document" NT has been renamed, make a new one
            //{
            //    _createNewSDSDocNT();
            //}
            ////Remove SDSModule/MaterialDocumentNT junction
            //CswNbtMetaDataNodeType MatDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            //if( null != MatDocNT )
            //{
            //    _CswNbtSchemaModTrnsctn.deleteModuleNodeTypeJunction( CswEnumNbtModuleName.SDS, MatDocNT.NodeTypeId );
            //}
        } // update()

        //private void _migrateExistingSDSDocNT( CswNbtMetaDataNodeType SDSNT )
        //{
        //    //Update the SDS NodeTypeProps' OCPs to point to SDSClass's OCPs
        //    CswNbtMetaDataObjectClass SDSDocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
        //
        //    List<int> DoomedNTPIds = new List<int>();
        //    CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29833_ntp_update", "nodetype_props" );
        //    DataTable NTPTable = NTPUpdate.getTable( "where nodetypeid = " + SDSNT.NodeTypeId );
        //    foreach( DataRow NTPRow in NTPTable.Rows )
        //    {
        //        string NTPropName = NTPRow["propname"].ToString();
        //        bool MoveProp = ( NTPropName == CswNbtPropertySetDocument.PropertyName.AcquiredDate ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.ArchiveDate ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.Archived ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.File ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.FileType ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.Link ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.Owner ||
        //                            NTPropName == CswNbtPropertySetDocument.PropertyName.Title ||
        //                            NTPropName == CswNbtObjClass.PropertyName.Save ||
        //                            NTPropName == "Material" || //Special Case
        //                            NTPropName == CswNbtObjClassSDSDocument.PropertyName.RevisionDate || //NTP
        //                            NTPropName == CswNbtObjClassSDSDocument.PropertyName.Language ||
        //                            NTPropName == CswNbtObjClassSDSDocument.PropertyName.Format );
        //        if( MoveProp )
        //        {
        //            if( NTPropName == "Material" )//Special Case
        //            {
        //                NTPropName = CswNbtPropertySetDocument.PropertyName.Owner;
        //            }
        //            NTPRow["objectclasspropid"] = SDSDocOC.getObjectClassProp( NTPropName ).ObjectClassPropId;
        //        }
        //        else//The only props that should be removed is "Document Class" and "Expiration Date"
        //        {
        //            NTPRow["objectclasspropid"] = DBNull.Value;
        //            DoomedNTPIds.Add( CswConvert.ToInt32( NTPRow["nodetypepropid"] ) );
        //        }
        //    }
        //    NTPUpdate.update( NTPTable );
        //
        //    foreach( int DoomedNTPId in DoomedNTPIds )
        //    {
        //        CswNbtMetaDataNodeTypeProp DoomedNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( DoomedNTPId );
        //        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( DoomedNTP );
        //    }
        //}
        //
        //private void _createNewSDSDocNT()
        //{
        //    CswEnumNbtNodeTypePermission[] NTPermissions = 
        //    { 
        //        CswEnumNbtNodeTypePermission.View, 
        //        CswEnumNbtNodeTypePermission.Create, 
        //        CswEnumNbtNodeTypePermission.Edit, 
        //        CswEnumNbtNodeTypePermission.Delete 
        //    };
        //
        //    //New Document NT
        //    CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
        //    CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
        //    CswNbtMetaDataNodeType SDSNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOC.ObjectClassId, "SDS Document", "Materials" );
        //    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswEnumNbtModuleName.SDS, SDSNT.NodeTypeId );
        //    //Default Title
        //    CswNbtMetaDataNodeTypeProp TitleNTP = SDSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
        //    TitleNTP.DefaultValue.AsText.Text = "Safety Data Sheet";
        //    TitleNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
        //    //Set Owner FK to Material OC (This needs to be done explicitly for the NTP - see Case 26605)
        //    CswNbtMetaDataNodeTypeProp OwnerNTP = SDSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
        //    OwnerNTP.PropName = "Material";
        //    OwnerNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), ChemicalOC.ObjectClassId );
        //    //NT Permission
        //    CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
        //    if( null != RoleNode )
        //    {
        //        _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, SDSNT, RoleNode, true );
        //    }
        //}

    }//class CswUpdateSchema_02B_Case29833

}//namespace ChemSW.Nbt.Schema