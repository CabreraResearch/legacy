using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52670 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52670; }
        }

        public override string Title
        {
            get { return "Setup auditing"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            // clean up existing auditlevels set as 0 or 1
            CswTableSelect NodeTypesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "52670_nt_select", "nodetypes" );
            DataTable NodeTypesTable = NodeTypesSelect.getTable( new CswCommaDelimitedString() { "nodetypeid", "auditlevel" }, "where auditlevel in ('0', '1')" );
            foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeRow["nodetypeid"] ) );
                if( null != NodeType )
                {
                    if( NodeTypeRow["auditlevel"].ToString() == "0" )
                    {
                        NodeType.DesignNode.AuditLevel.Value = CswEnumAuditLevel.NoAudit;
                        NodeType.DesignNode.AuditLevel.setSubFieldModified( CswEnumNbtSubFieldName.Value, true );
                    }
                    else if( NodeTypeRow["auditlevel"].ToString() == "1" )
                    {
                        NodeType.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;
                        NodeType.DesignNode.AuditLevel.setSubFieldModified( CswEnumNbtSubFieldName.Value, true );
                    }
                }
            } // foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )

            // clean up existing auditlevels set as 0 or 1
            CswTableSelect NodeTypePropsSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "52670_ntp_select", "nodetype_props" );
            DataTable NodeTypePropsTable = NodeTypePropsSelect.getTable( new CswCommaDelimitedString() { "nodetypepropid", "auditlevel" }, "where auditlevel in ('0', '1')" );
            foreach( DataRow PropRow in NodeTypePropsTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["nodetypepropid"] ) );
                if( null != NodeTypeProp )
                {
                    if( PropRow["auditlevel"].ToString() == "0" )
                    {
                        NodeTypeProp.DesignNode.AuditLevel.Value = CswEnumAuditLevel.NoAudit;
                        NodeTypeProp.DesignNode.AuditLevel.setSubFieldModified( CswEnumNbtSubFieldName.Value, true );
                    }
                    else if( PropRow["auditlevel"].ToString() == "1" )
                    {
                        NodeTypeProp.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;
                        NodeTypeProp.DesignNode.AuditLevel.setSubFieldModified( CswEnumNbtSubFieldName.Value, true );
                    }
                }
            } // foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )
            // set audit info as provided in the CIS-52670
            Collection<Tuple<string, string>> AuditInfo = _makeAuditInfo();

            foreach( Tuple<string, string> a in AuditInfo )
            {
                CswNbtMetaDataNodeType AuditNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( a.Item1 );
                if( null != AuditNT )
                {
                    AuditNT.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;

                    CswNbtMetaDataNodeTypeProp AuditPropNTP = AuditNT.getNodeTypeProp( a.Item2 );
                    if( null != AuditPropNTP && false == AuditPropNTP.getFieldType().IsDisplayType() )
                    {
                        AuditPropNTP.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;
                    }
                }
            } // foreach( Tuple<string, string> a in AuditInfo )

            foreach( CswNbtMetaDataNodeType AuditedNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes().Where( nt => nt.AuditLevel == CswEnumAuditLevel.PlainAudit ) )
            {
                //assure that for every nodetype where anything is audited, the required (not servermanaged) properties are audited.
                foreach( CswNbtMetaDataNodeTypeProp RequiredNTP in AuditedNT.getNodeTypeProps().Where( ntp => ntp.IsRequired ) )
                {
                    if( false == RequiredNTP.getFieldType().IsDisplayType() )
                    {
                        RequiredNTP.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;
                    }
                }

                //assure that for every audited nodetype with a "name"-like property the property is audited
                foreach( Int32 NamePropId in AuditedNT.NameTemplatePropIds.ToIntCollection() )
                {
                    CswNbtMetaDataNodeTypeProp NamePropNTP = AuditedNT.getNodeTypeProp( NamePropId );
                    if( null != NamePropNTP && false == NamePropNTP.getFieldType().IsDisplayType() )
                    {
                        NamePropNTP.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit;
                    }
                }
            } // foreach( CswNbtMetaDataNodeType AuditedNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes().Where( nt => nt.AuditLevel == CswEnumAuditLevel.PlainAudit ) )
        } // update()

        private Collection<Tuple<string, string>> _makeAuditInfo()
        {

            Collection<Tuple<string, string>> AuditInfo = new Collection<Tuple<string, string>>();
            AuditInfo.Add( new Tuple<string, string>( "User", "Archived" ) );
            AuditInfo.Add( new Tuple<string, string>( "User", "Cost Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "User", "Employee ID" ) );
            AuditInfo.Add( new Tuple<string, string>( "Department", "Department Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Building", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Building", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Building", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Building", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Building", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "City" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "Corporate Entity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "Country" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "State" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "Vendor Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Vendor", "Vendor Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Report", "Report Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "BioSafety Class" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Cabinet Number" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Cabinet", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Due Date Interval" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Enabled" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Final Due Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Message" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Run Time" ) );
            AuditInfo.Add( new Tuple<string, string>( "Mail Report", "Warning Days" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Department" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Room", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Shelf", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Shelf", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Shelf", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Shelf", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Shelf", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Box", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Box", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Box", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Box", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Box", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Floor", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Floor", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Floor", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Floor", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Floor", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Site", "Control Zone" ) );
            AuditInfo.Add( new Tuple<string, string>( "Site", "Inventory Group" ) );
            AuditInfo.Add( new Tuple<string, string>( "Site", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Site", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Site", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Synonym", "Language" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Synonym", "Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Approved for Receiving" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Aqueous Solubility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "CAS No" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Compressed Gas" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Container Expiration Locked" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "DOT Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "EINECS" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Formula" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Hazard Categories" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Hazard Classes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Hazard Info" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Hazardous" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Is Tier II" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Isotope" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Label Codes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "LQNo" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Material Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Melting Point" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Molecular Weight" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "NFPA" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Open Expire Interval" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Pictograms" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "PPE" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Special Flags" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Storage and Handling" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Structure" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "UN Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Obsolete" ) );
            AuditInfo.Add( new Tuple<string, string>( "Chemical", "Part Number" ) );
            AuditInfo.Add( new Tuple<string, string>( "Supply", "Approved for Receiving" ) );
            AuditInfo.Add( new Tuple<string, string>( "Supply", "Container Expiration Locked" ) );
            AuditInfo.Add( new Tuple<string, string>( "Supply", "Is Constituent" ) );
            AuditInfo.Add( new Tuple<string, string>( "Biological", "Approved for Receiving" ) );
            AuditInfo.Add( new Tuple<string, string>( "Biological", "Container Expiration Locked" ) );
            AuditInfo.Add( new Tuple<string, string>( "Biological", "Is Constituent" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Component", "Active" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Component", "Hazardous Reporting" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Component", "High % Value" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Component", "Low % Value" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Component", "Target % Value" ) );
            AuditInfo.Add( new Tuple<string, string>( "Size", "Catalog No" ) );
            AuditInfo.Add( new Tuple<string, string>( "Size", "Container Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Size", "Description" ) );
            AuditInfo.Add( new Tuple<string, string>( "Size", "Initial Quantity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Size", "Unit Count" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Concentration" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Home Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Label Format" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Lot Controlled" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Notes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Opened Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Project" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Reserved For" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Specific Activity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container", "Tare Quantity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Acquired Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Archived" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Expiration Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "File Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Link " ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Revision Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Material Document", "Title" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "Acquired Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "Archived" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "Expiration Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "File Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "Link " ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Document", "Title" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Aliases" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Base Unit" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Conversion Factor" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Fractional" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Unit Conversion" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Weight", "Unit Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Aliases" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Base Unit" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Conversion Factor" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Fractional" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Unit Conversion" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Volume", "Unit Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Aliases" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Base Unit" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Conversion Factor" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Fractional" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Unit Conversion" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit_Each", "Unit Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Aliases" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Base Unit" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Conversion Factor" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Fractional" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Unit Conversion" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Time", "Unit Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Inventory Level", "Current Quantity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Inventory Level", "Level" ) );
            AuditInfo.Add( new Tuple<string, string>( "Inventory Level", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Inventory Level", "Material" ) );
            AuditInfo.Add( new Tuple<string, string>( "Inventory Level", "Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Classifications" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Jurisdiction" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Label Codes" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Material" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Pictograms" ) );
            AuditInfo.Add( new Tuple<string, string>( "GHS", "Signal Word" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Dispense Transaction", "Quantity Dispensed" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List", "Exclusive" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List", "List Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "LQNo", "Limit" ) );
            AuditInfo.Add( new Tuple<string, string>( "LQNo", "LQNo" ) );
            AuditInfo.Add( new Tuple<string, string>( "Enterprise Part", "GCAS" ) );
            AuditInfo.Add( new Tuple<string, string>( "Enterprise Part", "Request" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Expiration Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Investigation Notes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Manufactured Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Manufacturer" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Manufacturer Lot No" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Material" ) );
            AuditInfo.Add( new Tuple<string, string>( "Receipt Lot", "Under Investigation" ) );
            AuditInfo.Add( new Tuple<string, string>( "Manufacturing Equivalent Part", "Enterprise Part" ) );
            AuditInfo.Add( new Tuple<string, string>( "Manufacturing Equivalent Part", "Manufacturer" ) );
            AuditInfo.Add( new Tuple<string, string>( "Manufacturing Equivalent Part", "Material" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Group", "Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Group", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Container Group", "Sync Location" ) );
            AuditInfo.Add( new Tuple<string, string>( "Control Zone", "Fire Class Set Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Control Zone", "MAQ Offset %" ) );
            AuditInfo.Add( new Tuple<string, string>( "Control Zone", "Control Zone Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Aliases" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Base Unit" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Conversion Factor" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Fractional" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Unit Conversion" ) );
            AuditInfo.Add( new Tuple<string, string>( "Unit Radiation", "Unit Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount Set", "Set Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Category Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Class" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Gas Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Gas Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Liquid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Liquid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Solid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Closed Solid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Hazard Category" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Hazard Class" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Hazard Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Open Liquid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Open Liquid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Open Solid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Open Solid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Set Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Sort Order" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Gas Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Gas Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Liquid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Liquid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Solid Exempt Amount" ) );
            AuditInfo.Add( new Tuple<string, string>( "Fire Class Exempt Amount", "Storage Solid Exempt Footnotes" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Acquired Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Archived" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Country" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "File Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Format" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Language" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Link " ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Revision Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "SDS Document", "Title" ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "Acquired Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "Archived" ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "File Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "Link " ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "Revision Date" ) );
            AuditInfo.Add( new Tuple<string, string>( "C of A Document", "Title" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List CAS", "CAS No" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List CAS", "Regulatory List" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List CAS", "TPQ" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "CAS No" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Disposal Instructions" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "DOT Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "EINECS" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Hazard Categories" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Hazard Classes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Hazard Info" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Hazardous" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Is Tier II" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Isotope" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Label Codes" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "LQNo" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Material Id" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Material Type" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "NFPA" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Part Number" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Physical State" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Pictograms" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "PPE" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Special Flags" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Specific Gravity" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Storage and Handling" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Storage Compatibility" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Structure" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Tradename" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "UN Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Constituent", "Obsolete" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List List Code", "LOLI List Code" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List List Code", "LOLI List Name" ) );
            AuditInfo.Add( new Tuple<string, string>( "Regulatory List List Code", "Regulatory List" ) );
            return AuditInfo;
        } // _makeAuditInfo()
    } // class
} // namespace