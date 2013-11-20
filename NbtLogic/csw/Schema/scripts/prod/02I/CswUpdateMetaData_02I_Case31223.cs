using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case31223 : CswUpdateSchemaTo
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
            get { return ""; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            /*
             * NOTE: This is an OC update because I am using the view created on the NTP to update the OCP (see comment on line 38 below) 
             */

            // Assigned SDS property view - Chemical
            CswNbtView AssignedSDSView = _editView( CswEnumNbtObjectClass.SDSDocumentClass, CswEnumNbtObjectClass.ChemicalClass, CswNbtObjClassChemical.PropertyName.AssignedSDS );
            // Update the ViewXML on the AssignedSDSOCP
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp AssignedSDSOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssignedSDSOCP, CswEnumNbtObjectClassPropAttributes.viewxml, AssignedSDSView.ToString() );

            // Documents property view on all Materials
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                _editView( CswEnumNbtObjectClass.DocumentClass, MaterialOC.ObjectClassName, CswNbtPropertySetMaterial.PropertyName.Documents, "Material Document" );
            }

            // Documents property view on Equipment
            _editView( CswEnumNbtObjectClass.DocumentClass, CswEnumNbtObjectClass.EquipmentClass, CswNbtPropertySetMaterial.PropertyName.Documents, "Equipment Document" );

            // Documents property view on Assemblies
            _editView( CswEnumNbtObjectClass.DocumentClass, CswEnumNbtObjectClass.EquipmentAssemblyClass, CswNbtPropertySetMaterial.PropertyName.Documents, "Assembly Document" );

            // Documents property view on Containers
            _editView( CswEnumNbtObjectClass.DocumentClass, CswEnumNbtObjectClass.ContainerClass, CswNbtPropertySetMaterial.PropertyName.Documents, "Container Document" );

        } // update()

        private CswNbtView _editView( string DocumentOCName, string ObjectClassOfGridProp, string GridProperty, string DocumentNTName = "" )
        {
            CswNbtView DocumentsView = null;

            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( DocumentOCName );
            CswNbtMetaDataNodeType DocumentNT = DocumentNTName != "" ? _CswNbtSchemaModTrnsctn.MetaData.getNodeType( DocumentNTName ) : DocumentOC.FirstNodeType;
            if( null != DocumentNT )
            {
                CswNbtMetaDataNodeTypeProp LinkNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Link );
                CswNbtMetaDataNodeTypeProp FileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.File );
                CswNbtMetaDataNodeTypeProp OpenFileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.OpenFile );

                CswNbtMetaDataObjectClass ObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjectClassOfGridProp );
                CswNbtMetaDataNodeType NodeType = ObjectClass.FirstNodeType;
                CswNbtMetaDataNodeTypeProp NodeTypeProp = NodeType.getNodeTypePropByObjectClassProp( GridProperty );
                if( null != NodeTypeProp )
                {
                    DocumentsView = _CswNbtSchemaModTrnsctn.restoreView( NodeTypeProp.ViewId );
                    DocumentsView.removeViewProperty( LinkNTP );
                    DocumentsView.removeViewProperty( FileNTP );

                    CswNbtViewRelationship Parent = DocumentsView.Root.ChildRelationships[0].ChildRelationships[0];
                    DocumentsView.AddViewProperty( Parent, OpenFileNTP );
                    DocumentsView.save();
                }
            }
            return DocumentsView;
        }
    }

}//namespace ChemSW.Nbt.Schema