using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28690
    /// </summary>
    public class CswUpdateSchema_02B_Case28690B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28690; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType SupplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            CswNbtMetaDataNodeType BiologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            if( null != SupplyNT )
            {
                _migrateMaterialNodeTypePropsToNonChemical( SupplyNT );
            }
            if( null != BiologicalNT )
            {
                _migrateMaterialNodeTypePropsToNonChemical( BiologicalNT );
            }
        } // update()

        private void _migrateMaterialNodeTypePropsToNonChemical( CswNbtMetaDataNodeType MaterialNT )
        {
            //Update the NodeTypeProps' OCPs to point to NonChemical's OCPs - if NonChemical does not have an OCP with the given NTP name, the NTP is deleted.
            CswNbtMetaDataObjectClass NonChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );

            List<int> DoomedNTPIds = new List<int>();
            CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_ntp_update", "nodetype_props" );
            DataTable NTPTable = NTPUpdate.getTable( "where nodetypeid = " + MaterialNT.NodeTypeId + " and objectclasspropid is not null" );
            foreach( DataRow NTPRow in NTPTable.Rows )
            {
                string NTPropName = NTPRow["propname"].ToString();
                bool MoveProp = ( NTPropName == CswNbtPropertySetMaterial.PropertyName.MaterialId ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.TradeName ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.Supplier ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.PartNumber ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.Receive ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.Request ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.C3ProductId ||
                                    NTPropName == CswNbtPropertySetMaterial.PropertyName.C3SyncDate ||
                                    NTPropName == CswNbtObjClass.PropertyName.Save || 
                                    NTPropName == "Biological Name" );//Special Case
                if( MoveProp )
                {
                    if( NTPropName == "Biological Name" )//Special Case
                    {
                        NTPropName = CswNbtPropertySetMaterial.PropertyName.TradeName;
                    }
                    NTPRow["objectclasspropid"] = NonChemicalOC.getObjectClassProp( NTPropName ).ObjectClassPropId;
                }
                else
                {
                    NTPRow["objectclasspropid"] = DBNull.Value;
                    DoomedNTPIds.Add( CswConvert.ToInt32( NTPRow["nodetypepropid"] ) );
                }
            }
            NTPUpdate.update( NTPTable );

            foreach( int DoomedNTPId in DoomedNTPIds )
            {
                CswNbtMetaDataNodeTypeProp DoomedNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( DoomedNTPId );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( DoomedNTP );
            }

            //special case - remove SDS grid prop
            CswNbtMetaDataNodeTypeProp AssignedSDSNTP = MaterialNT.getNodeTypeProp( "Assigned SDS" );
            if( null != AssignedSDSNTP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( AssignedSDSNTP );
            }

            //remove hazards tab
            CswNbtMetaDataNodeTypeTab HazardsTab = MaterialNT.getNodeTypeTab( "Hazards" );
            if( null != HazardsTab )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( HazardsTab );
            }
        }

    }//class CswUpdateSchema_02B_Case28690B
}//namespace ChemSW.Nbt.Schema