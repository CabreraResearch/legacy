using System.Data;
using System.Linq;
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
    public class CswUpdateMetaData_02L_Case52017: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52017; }
        }

        public override string Title
        {
            get { return "Create Biological Object Class"; }
        }

        public override void update()
        {

            _createBiologicalOC();
            _linkBiologicalOCtoPS();

        } // update()

        private void _createBiologicalOC()
        {
            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.BiologicalClass, "dna.png", true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = "Species Origin",
                    FieldType = CswEnumNbtFieldType.Text,
                    AuditLevel = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Reference Number",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Type",
                FieldType = CswEnumNbtFieldType.List,
                AuditLevel = true,
                ListOptions = "DNA,RNA,Protein,Organism"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Reference Type",
                FieldType = CswEnumNbtFieldType.List,
                AuditLevel = true,
                ListOptions = "ATCC,NIH,CDC"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Biosafety Level",
                FieldType = CswEnumNbtFieldType.List,
                AuditLevel = true,
                ListOptions = ""
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Storage Conditions",
                FieldType = CswEnumNbtFieldType.List,
                AuditLevel = true,
                ListOptions = "37C,25C,5C,-20C,-80C"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Vectors",
                FieldType = CswEnumNbtFieldType.MultiList,
                AuditLevel = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Physical State",
                FieldType = CswEnumNbtFieldType.List,
                AuditLevel = true,
                ListOptions = "solid,liquid,gas,n/a"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Legacy Material Id",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Tradename",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Approved for Receiving",
                FieldType = CswEnumNbtFieldType.Logical,
                AuditLevel = true,
                
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = "Receive",
                    FieldType = CswEnumNbtFieldType.Button
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Request",
                FieldType = CswEnumNbtFieldType.Button
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "C3ProductId",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Tradename",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Material Id",
                FieldType = CswEnumNbtFieldType.Sequence,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Supplier",
                FieldType = CswEnumNbtFieldType.Relationship,
                AuditLevel = true,
                FkValue = 314,
                FkType = "ObjectClassId"
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Part Number",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Documents",
                FieldType = CswEnumNbtFieldType.Grid,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Is Constituent",
                FieldType = CswEnumNbtFieldType.Logical,
                AuditLevel = true,

            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Container Expiration Locked",
                FieldType = CswEnumNbtFieldType.Logical,
                AuditLevel = true,

            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Synonyms",
                FieldType = CswEnumNbtFieldType.Grid,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Picture",
                FieldType = CswEnumNbtFieldType.Image,
                AuditLevel = true,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( BiologicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = "Biological Name",
                FieldType = CswEnumNbtFieldType.Text,
                AuditLevel = true,
            } );

        }//createBiologicalOC()

        private void _linkBiologicalOCtoPS()
        {

            //Update the NodeTypeProps' OCPs to point to Biological's OCPs - if NonChemical does not have an OCP with the given NTP name, the NTP is deleted.
            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );

            // Populate jct_propertyset_objectclass
                CswTableUpdate JctOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_jct_update", "jct_propertyset_objectclass" );
                DataTable JctOCTable = JctOCUpdate.getEmptyTable();

                DataRow NewRow = JctOCTable.NewRow();
                NewRow["objectclassid"] = BiologicalOC.ObjectClassId;
                NewRow["propertysetid"] = CswConvert.ToDbVal( _CswNbtSchemaModTrnsctn.MetaData.getPropertySetId( CswEnumNbtPropertySetName.MaterialSet ) );
                JctOCTable.Rows.Add( NewRow );

                JctOCUpdate.update( JctOCTable );


           // Populate jct_propertyset_objectclassprop
                CswTableUpdate JctOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_jct2_update", "jct_propertyset_ocprop" );
                DataTable JctOCPTable = JctOCPUpdate.getEmptyTable();

                foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in BiologicalOC.getObjectClassProps() )
                {
                    string[] PropsToAdd = new string[]
                        {
                            CswNbtObjClassBiological.PropertyName.TradeName,
                            CswNbtObjClassBiological.PropertyName.LegacyMaterialId,
                            CswNbtObjClassBiological.PropertyName.ApprovedForReceiving,
                            CswNbtObjClassBiological.PropertyName.C3ProductId,
                            CswNbtObjClassBiological.PropertyName.ContainerExpirationLocked,
                            CswNbtObjClassBiological.PropertyName.MaterialId,
                            CswNbtObjClassBiological.PropertyName.Supplier,
                            CswNbtObjClassBiological.PropertyName.PartNumber,
                            CswNbtObjClassBiological.PropertyName.Receive,
                            CswNbtObjClassBiological.PropertyName.Request,
                            CswNbtObjClassBiological.PropertyName.IsConstituent,
                            CswNbtObjClassBiological.PropertyName.Documents,
                            CswNbtObjClassBiological.PropertyName.Synonyms
                        };

                    if( PropsToAdd.Contains( ObjectClassProp.PropName ) )
                    {
                        NewRow = JctOCPTable.NewRow();
                        NewRow["objectclasspropid"] = ObjectClassProp.PropId;
                        NewRow["propertysetid"] = CswConvert.ToDbVal( _CswNbtSchemaModTrnsctn.MetaData.getPropertySetId( CswEnumNbtPropertySetName.MaterialSet ) );
                        JctOCPTable.Rows.Add( NewRow );
                    }// if the prop is a PSP, insert a jct record

                } //  foreach property in the biological class

                JctOCPUpdate.update( JctOCPTable );

        }//linkBiologicalOCtoPS


    }

}//namespace ChemSW.Nbt.Schema