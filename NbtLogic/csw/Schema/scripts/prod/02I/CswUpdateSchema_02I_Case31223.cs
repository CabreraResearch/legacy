using System;
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
    public class CswUpdateSchema_02I_Case31223 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31223; }
        }

        public override string Title
        {
            get { return "Remove Link/File properties from Documents views"; }
        }

        public override string AppendToScriptName()
        {
            return "A_V2";
        }

        public override void update()
        {
            // Assigned SDS property view - Chemical
            _alterAssignedSDSPropView();

            // Documents property view on Equipment
            _alterEquipmentsDocumentView();

            // Documents property view on Assemblies
            _alterAssemblysDocumentView();

            // Documents property view on Containers
            _alterContainersDocumentView();

            // Documents property view on all Materials
            _alterMaterialsDocumentView();

        } // update()

        private void _alterAssignedSDSPropView()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataNodeType DocumentNT = DocumentOC.FirstNodeType; //loop
            if( null != DocumentNT )
            {
                CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                CswNbtMetaDataNodeType ChemicalNT = ChemicalOC.FirstNodeType;

                CswNbtMetaDataNodeTypeProp AssignedSDSNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
                CswNbtMetaDataNodeTypeProp OwnerNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Owner );

                // Get the properties we need for the view
                CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp RevisionDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.RevisionDate );
                CswNbtMetaDataNodeTypeProp LanguageNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Language );
                CswNbtMetaDataNodeTypeProp FormatNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Format );
                CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.OpenFile );

                CswNbtView SDSDocsView = _CswNbtSchemaModTrnsctn.makeNewView( AssignedSDSNTP.PropName, CswEnumNbtViewVisibility.Property );
                SDSDocsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship ParentRelationship = SDSDocsView.AddViewRelationship( ChemicalOC, false );
                CswNbtViewRelationship SecondRelationship = SDSDocsView.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, false );

                SDSDocsView.AddViewPropertyAndFilter( SecondRelationship, ArchivedNTP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked, ShowInGrid: false );
                SDSDocsView.AddViewProperty( SecondRelationship, RevisionDateNTP );
                SDSDocsView.AddViewProperty( SecondRelationship, LanguageNTP );
                SDSDocsView.AddViewProperty( SecondRelationship, FormatNTP );
                SDSDocsView.AddViewProperty( SecondRelationship, OpenFileNTP );

                // We need this for the OCP view to be set correctly
                SDSDocsView.save();

                // Update the existing viewxml
                Int32 ViewId = AssignedSDSNTP.ViewId.get();
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateAssignedSDSViewXML_Case31223", "node_views" );
                DataTable NodeViewsTbl = TableUpdate.getTable( "where nodeviewid = " + ViewId );
                if( NodeViewsTbl.Rows.Count > 0 )
                {
                    NodeViewsTbl.Rows[0]["viewxml"] = SDSDocsView.ToString();
                }
                TableUpdate.update( NodeViewsTbl );

                // Also update the AssignedSDS OCP
                CswNbtMetaDataObjectClassProp AssignedSDSOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssignedSDSOCP, CswEnumNbtObjectClassPropAttributes.viewxml, SDSDocsView.ToString() );
            }
        }

        private void _alterContainersDocumentView()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
            if( null != DocumentNT )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;

                CswNbtMetaDataNodeTypeProp DocumentsNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Documents );
                CswNbtMetaDataNodeTypeProp OwnerNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Owner );

                CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp AcquiredDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.AcquiredDate );
                CswNbtMetaDataNodeTypeProp TitleNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Title );
                CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );
                CswNbtMetaDataNodeTypeProp ExpirationDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );

                CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.makeNewView( DocumentsNTP.PropName, CswEnumNbtViewVisibility.Property );
                DocumentsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship ParentRelationship = DocumentsView.AddViewRelationship( ContainerOC, false );
                CswNbtViewRelationship SecondRelationship = DocumentsView.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, false );

                DocumentsView.AddViewProperty( SecondRelationship, TitleNTP );
                DocumentsView.AddViewProperty( SecondRelationship, AcquiredDateNTP );
                DocumentsView.AddViewProperty( SecondRelationship, ExpirationDateNTP );
                DocumentsView.AddViewPropertyAndFilter( SecondRelationship, ArchivedNTP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked, ShowInGrid: false, ShowAtRuntime: true );
                DocumentsView.AddViewProperty( SecondRelationship, OpenFileNTP );

                // Update the existing viewxml
                Int32 ViewId = DocumentsNTP.ViewId.get();
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateContainerDocsViewXML_Case31223", "node_views" );
                DataTable NodeViewsTbl = TableUpdate.getTable( "where nodeviewid = " + ViewId );
                if( NodeViewsTbl.Rows.Count > 0 )
                {
                    NodeViewsTbl.Rows[0]["viewxml"] = DocumentsView.ToString();
                }
                TableUpdate.update( NodeViewsTbl );

            }
        }

        private void _alterAssemblysDocumentView()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Document" );
            if( null != DocumentNT )
            {
                CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
                CswNbtMetaDataNodeType AssemblyNT = AssemblyOC.FirstNodeType;

                CswNbtMetaDataNodeTypeProp DocumentsNTP = AssemblyNT.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Documents );
                CswNbtMetaDataNodeTypeProp OwnerNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Owner );

                CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp AcquiredDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.AcquiredDate );
                CswNbtMetaDataNodeTypeProp TitleNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Title );
                CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );
                CswNbtMetaDataNodeTypeProp ExpirationDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );

                CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.makeNewView( DocumentsNTP.PropName, CswEnumNbtViewVisibility.Property );
                DocumentsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship ParentRelationship = DocumentsView.AddViewRelationship( AssemblyOC, false );
                CswNbtViewRelationship SecondRelationship = DocumentsView.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, false );

                DocumentsView.AddViewProperty( SecondRelationship, TitleNTP );
                DocumentsView.AddViewProperty( SecondRelationship, AcquiredDateNTP );
                DocumentsView.AddViewProperty( SecondRelationship, ExpirationDateNTP );
                DocumentsView.AddViewProperty( SecondRelationship, ArchivedNTP );
                DocumentsView.AddViewProperty( SecondRelationship, OpenFileNTP );

                // Update the existing viewxml
                Int32 ViewId = DocumentsNTP.ViewId.get();
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateAssemblyDocsViewXML_Case31223", "node_views" );
                DataTable NodeViewsTbl = TableUpdate.getTable( "where nodeviewid = " + ViewId );
                if( NodeViewsTbl.Rows.Count > 0 )
                {
                    NodeViewsTbl.Rows[0]["viewxml"] = DocumentsView.ToString();
                }
                TableUpdate.update( NodeViewsTbl );

            }
        }

        private void _alterEquipmentsDocumentView()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Document" );
            if( null != DocumentNT )
            {
                CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
                CswNbtMetaDataNodeType EquipmentNT = EquipmentOC.FirstNodeType;

                CswNbtMetaDataNodeTypeProp DocumentsNTP = EquipmentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.PropertyName.Documents );
                CswNbtMetaDataNodeTypeProp OwnerNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Owner );

                CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp AcquiredDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.AcquiredDate );
                CswNbtMetaDataNodeTypeProp TitleNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Title );
                CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );
                CswNbtMetaDataNodeTypeProp ExpirationDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );

                CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.makeNewView( DocumentsNTP.PropName, CswEnumNbtViewVisibility.Property );
                DocumentsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship ParentRelationship = DocumentsView.AddViewRelationship( EquipmentOC, false );
                CswNbtViewRelationship SecondRelationship = DocumentsView.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, false );

                DocumentsView.AddViewProperty( SecondRelationship, TitleNTP );
                DocumentsView.AddViewProperty( SecondRelationship, AcquiredDateNTP );
                DocumentsView.AddViewProperty( SecondRelationship, ExpirationDateNTP );
                DocumentsView.AddViewProperty( SecondRelationship, ArchivedNTP );
                DocumentsView.AddViewProperty( SecondRelationship, OpenFileNTP );

                // Update the existing viewxml
                Int32 ViewId = DocumentsNTP.ViewId.get();
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateEquipDocsViewXML_Case31223", "node_views" );
                DataTable NodeViewsTbl = TableUpdate.getTable( "where nodeviewid = " + ViewId );
                if( NodeViewsTbl.Rows.Count > 0 )
                {
                    NodeViewsTbl.Rows[0]["viewxml"] = DocumentsView.ToString();
                }
                TableUpdate.update( NodeViewsTbl );

            }
        }

        private void _alterMaterialsDocumentView()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != DocumentNT )
            {
                CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
                foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
                {
                    CswNbtMetaDataNodeType MaterialNT = MaterialOC.FirstNodeType;
                    //CswNbtMetaDataObjectClassProp DocumentsOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.Documents );
                    CswNbtMetaDataNodeTypeProp DocumentsNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.Documents );
                    CswNbtMetaDataNodeTypeProp OwnerNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Owner );

                    CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Archived );
                    CswNbtMetaDataNodeTypeProp AcquiredDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.AcquiredDate );
                    CswNbtMetaDataNodeTypeProp TitleNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Title );
                    CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );
                    CswNbtMetaDataNodeTypeProp ExpirationDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );

                    CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.makeNewView( DocumentsNTP.PropName, CswEnumNbtViewVisibility.Property );
                    DocumentsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                    CswNbtViewRelationship ParentRelationship = DocumentsView.AddViewRelationship( MaterialPS, false );
                    CswNbtViewRelationship SecondRelationship = DocumentsView.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, false );

                    DocumentsView.AddViewProperty( SecondRelationship, TitleNTP );
                    DocumentsView.AddViewProperty( SecondRelationship, AcquiredDateNTP );
                    DocumentsView.AddViewProperty( SecondRelationship, ExpirationDateNTP );
                    DocumentsView.AddViewPropertyAndFilter( SecondRelationship, ArchivedNTP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked, ShowInGrid: false, ShowAtRuntime: true );
                    DocumentsView.AddViewProperty( SecondRelationship, OpenFileNTP );

                    // Update the existing viewxml
                    Int32 ViewId = DocumentsNTP.ViewId.get();
                    CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateMaterialDocsViewXML_Case31223", "node_views" );
                    DataTable NodeViewsTbl = TableUpdate.getTable( "where nodeviewid = " + ViewId );
                    if( NodeViewsTbl.Rows.Count > 0 )
                    {
                        NodeViewsTbl.Rows[0]["viewxml"] = DocumentsView.ToString();
                    }
                    TableUpdate.update( NodeViewsTbl );
                }//foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            }
        }
    }

}//namespace ChemSW.Nbt.Schema