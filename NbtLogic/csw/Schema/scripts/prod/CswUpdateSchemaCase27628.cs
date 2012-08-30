using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27628
    /// </summary>
    public class CswUpdateSchemaCase27628 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            //All the Object classes with a Documents gird property
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass equipmentAssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            //get the props we'll be adding to the grid via the OC so we can avoid several 'if' statements
            CswNbtMetaDataObjectClass documentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClassProp linkOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
            CswNbtMetaDataObjectClassProp fileOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.File );

            //update the Material Documents grid
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp documentsNTP = materialNT.getNodeTypeProp( "Documents" );
                if( null != documentsNTP )
                {
                    CswNbtView docsView = _CswNbtSchemaModTrnsctn.restoreView( documentsNTP.ViewId );
                    CswNbtViewRelationship parent = docsView.Root.ChildRelationships[0];
                    CswNbtViewRelationship firstChild = parent.ChildRelationships[0];
                    CswNbtMetaDataNodeType materialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                    if( null != materialDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp linkNTP = materialDocumentNT.getNodeTypePropByObjectClassProp( linkOCP.ObjectClassPropId );
                        CswNbtMetaDataNodeTypeProp fileNTP = materialDocumentNT.getNodeTypePropByObjectClassProp( fileOCP.ObjectClassPropId );
                        docsView.AddViewProperty( firstChild, linkNTP );
                        docsView.AddViewProperty( firstChild, fileNTP );
                        docsView.save();
                    }
                }
            }

            //update the equipment assembly documents grid
            foreach( CswNbtMetaDataNodeType equipmentAssemblyNT in equipmentAssemblyOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp documentsNTP = equipmentAssemblyNT.getNodeTypeProp( "Documents" );
                if( null != documentsNTP )
                {
                    CswNbtView docsView = _CswNbtSchemaModTrnsctn.restoreView( documentsNTP.ViewId );
                    CswNbtViewRelationship parent = docsView.Root.ChildRelationships[0];
                    CswNbtViewRelationship firstChild = parent.ChildRelationships[0];
                    CswNbtMetaDataNodeType assemblyDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Document" );
                    if( null != assemblyDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp linkNTP = assemblyDocumentNT.getNodeTypePropByObjectClassProp( linkOCP.ObjectClassPropId );
                        CswNbtMetaDataNodeTypeProp fileNTP = assemblyDocumentNT.getNodeTypePropByObjectClassProp( fileOCP.ObjectClassPropId );
                        docsView.AddViewProperty( firstChild, linkNTP );
                        docsView.AddViewProperty( firstChild, fileNTP );
                        docsView.save();
                    }
                }
            }

            //update the equipment documents grid
            foreach( CswNbtMetaDataNodeType equipmentNT in equipmentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp documentsNTP = equipmentNT.getNodeTypeProp( "Documents" );
                if( null != documentsNTP )
                {
                    CswNbtView docsView = _CswNbtSchemaModTrnsctn.restoreView( documentsNTP.ViewId );
                    CswNbtViewRelationship parent = docsView.Root.ChildRelationships[0];
                    CswNbtViewRelationship firstChild = parent.ChildRelationships[0];
                    CswNbtMetaDataNodeType equipmentDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Document" );
                    if( null != equipmentDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp linkNTP = equipmentDocumentNT.getNodeTypePropByObjectClassProp( linkOCP.ObjectClassPropId );
                        CswNbtMetaDataNodeTypeProp fileNTP = equipmentDocumentNT.getNodeTypePropByObjectClassProp( fileOCP.ObjectClassPropId );
                        docsView.AddViewProperty( firstChild, linkNTP );
                        docsView.AddViewProperty( firstChild, fileNTP );
                        docsView.save();
                    }
                }
            }

            //update the container documents grid
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp documentsNTP = containerNT.getNodeTypeProp( "Documents" );
                if( null != documentsNTP )
                {
                    CswNbtView docsView = _CswNbtSchemaModTrnsctn.restoreView( documentsNTP.ViewId );
                    CswNbtViewRelationship parent = docsView.Root.ChildRelationships[0];
                    CswNbtViewRelationship firstChild = parent.ChildRelationships[0];
                    CswNbtMetaDataNodeType containerDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
                    if( null != containerDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp linkNTP = containerDocumentNT.getNodeTypePropByObjectClassProp( linkOCP.ObjectClassPropId );
                        CswNbtMetaDataNodeTypeProp fileNTP = containerDocumentNT.getNodeTypePropByObjectClassProp( fileOCP.ObjectClassPropId );
                        docsView.AddViewProperty( firstChild, linkNTP );
                        docsView.AddViewProperty( firstChild, fileNTP );
                        docsView.save();
                    }
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema