using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    class CswUpdateMetaData_02I_Case31210 : CswUpdateSchemaTo
    {

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31210; }
        }

        public override string Title
        {
            get { return "Add new props to DocumentPS & SDSDocumentOC"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Create OpenUrl DocumentPS property
            CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31210_jctpsocp_update", "jct_propertyset_ocprop" );
            DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();

            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            foreach( CswNbtMetaDataObjectClass DocOC in DocumentPS.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp OpenFileOCP = DocOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );
                if( null == OpenFileOCP )
                {
                    OpenFileOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DocOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                    {
                        PropName = CswNbtPropertySetDocument.PropertyName.OpenFile,
                        FieldType = CswEnumNbtFieldType.Button
                    } );

                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = OpenFileOCP.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( DocumentPS.PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
            JctPSOCPUpdate.update( JctPSOCPTable );

            // Create ChemWatch property on SDSDocumentOC
            CswNbtMetaDataObjectClass SDSDocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataObjectClassProp ChemWatchOCP = SDSDocumentOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.ChemWatch );
            if( null == ChemWatchOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( SDSDocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( SDSDocumentOC )
                    {
                        PropName = CswNbtObjClassSDSDocument.PropertyName.ChemWatch,
                        FieldType = CswEnumNbtFieldType.Text,
                        ReadOnly = true
                    } );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema