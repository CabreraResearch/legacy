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
            return "A_V3";
        }

        public override void update()
        {
            // Assigned SDS property view - Chemical
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtView SDSDocumentsView = _editView( "SDS Document", ChemicalOC, CswNbtObjClassChemical.PropertyName.AssignedSDS );
            _updateAssignedSDSOCP( SDSDocumentsView );

            // Documents property view on Equipment
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
            _editView( "Equipment Document", EquipmentOC, CswNbtObjClassEquipment.PropertyName.Documents );

            // Documents property view on Assemblies
            CswNbtMetaDataObjectClass EquipAssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
            _editView( "Assembly Document", EquipAssemblyOC, CswNbtObjClassEquipmentAssembly.PropertyName.Documents );

            // Documents property view on Containers
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            _editView( "Container Document", ContainerOC, CswNbtObjClassContainer.PropertyName.Documents );

            // Documents property view on all Materials
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                _editView( "Material Document", MaterialOC, CswNbtPropertySetMaterial.PropertyName.Documents );
            }

        } // update()

        private CswNbtView _editView( string DocumentNodeType, CswNbtMetaDataObjectClass ObjectClassOfProperty, string DocumentObjClassProp )
        {
            CswNbtView DocumentsView = null;

            CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( DocumentNodeType );
            if( null != DocumentNT )
            {
                CswNbtMetaDataNodeType NodeTypeOfProperty = ObjectClassOfProperty.FirstNodeType;
                if( null != NodeTypeOfProperty )
                {
                    // documents property that contains the viewxml
                    CswNbtMetaDataNodeTypeProp DocumentsNTP = NodeTypeOfProperty.getNodeTypePropByObjectClassProp( DocumentObjClassProp );
                    if( null != DocumentsNTP )
                    {
                        CswNbtMetaDataNodeTypeProp LinkNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Link );
                        CswNbtMetaDataNodeTypeProp FileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.File );
                        CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );

                        DocumentsView = _CswNbtSchemaModTrnsctn.restoreView( DocumentsNTP.ViewId );
                        if( false == DocumentsView.IsEmpty() )
                        {
                            // View.Root only gives us the <TreeView></TreeView> content. It does not give the first relationship. We are assuming
                            // that these views have two relationships: An OC (or PS) and a Document relationship
                            // To get at the Document relationship, we need to get the second relationship. If the view doesn't have this second relationship,
                            // we will just ignore it and won't make any changes
                            CswNbtViewRelationship ParentRel = DocumentsView.Root.ChildRelationships[0];
                            if( ParentRel.ChildRelationships.Count > 0 )
                            {
                                DocumentsView.removeViewProperty( LinkNTP );
                                DocumentsView.removeViewProperty( FileNTP );

                                CswNbtViewRelationship ChildRel = ParentRel.ChildRelationships[0];
                                if( null == ChildRel.findProperty( OpenFileNTP.NodeTypeId ) )
                                {
                                    DocumentsView.AddViewProperty( ChildRel, OpenFileNTP );
                                }
                                DocumentsView.save();
                            }
                        } //if( false == DocumentsView.IsEmpty() )

                    }//if( null != DocumentsNTP )

                }//if( null != NodeTypeOfProperty )

            }//if( null != DocumentNT )

            return DocumentsView;

        }//_editView()

        private void _updateAssignedSDSOCP( CswNbtView View )
        {
            // Update the ViewXML on the AssignedSDSOCP
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp AssignedSDSOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssignedSDSOCP, CswEnumNbtObjectClassPropAttributes.viewxml, View.ToString() );
        }
    }

}//namespace ChemSW.Nbt.Schema