using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29984 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 29984; }
        }

        public override string ScriptName
        {
            get { return "02F_Case29984"; }
        }

        public override void update()
        {
            _addMaterialComponentPermissions();
            _addTitleToDocumentsView();
            _addExpirationDateToContainersView();

        } // update()

        private void _addMaterialComponentPermissions()
        {
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            if( null != MaterialComponentOC )
            {
                CswNbtMetaDataNodeType MaterialComponentNT = MaterialComponentOC.getNodeTypes().FirstOrDefault();
                if( null != MaterialComponentNT )
                {
                    string[] FullPermissionRoles = { "CISPro_Admin", "CISPro_General", "CISPro_Receiver", "CISPro_Request_Fulfiller", "Administrator" };

                    CswEnumNbtNodeTypePermission[] AllPermissions =
                        {
                            CswEnumNbtNodeTypePermission.View,
                            CswEnumNbtNodeTypePermission.Create,
                            CswEnumNbtNodeTypePermission.Edit,
                            CswEnumNbtNodeTypePermission.Delete
                        };

                    foreach( string RoleName in FullPermissionRoles )
                    {
                        CswNbtObjClassRole Role = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( RoleName );
                        _CswNbtSchemaModTrnsctn.Permit.set( AllPermissions, MaterialComponentNT, Role, true );
                    }

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MaterialComponentNT, true );

                }//if( null != MaterialComponentNT )
            }//if( null != MaterialComponentOC )
        }//_addMaterialComponentPermissions()

        private void _addTitleToDocumentsView()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp DocumentsNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.Documents );
                    CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                    
                    if( null != MaterialDocumentNT )
                    {
                        CswNbtMetaDataObjectClass DocumentOC = MaterialDocumentNT.getObjectClass();
                        CswNbtMetaDataNodeTypeProp[] propsToAdd =
                            {
                                MaterialDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title )
                            };

                        CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.restoreView( DocumentsNTP.ViewId );
                        if( null != DocumentsView )
                        {
                            _addPropertiesToView( DocumentsView, DocumentsView.Root.ChildRelationships, DocumentOC, MaterialDocumentNT, propsToAdd );
                        }
                    }
                }
            }
        }

        private void _addExpirationDateToContainersView()
        {
            string[] ViewsToUpdate = { "Chemical Containers", "Biological Containers", "Supply Containers" };

            foreach( string ViewName in ViewsToUpdate )
            {
                CswNbtView View = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( ViewName, CswEnumNbtViewVisibility.Property );

                if( null != View )
                {
                    CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                    CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;

                    if( null != ContainerNT )
                    {
                        CswNbtMetaDataNodeTypeProp[] propsToAdd =
                            {
                                ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate )
                            };

                        _addPropertiesToView( View, View.Root.ChildRelationships, ContainerOC, ContainerNT, propsToAdd );
                    }
                }
            }
        }

        private void _addPropertiesToView( CswNbtView View, Collection<CswNbtViewRelationship> Relationships, CswNbtMetaDataObjectClass relatedOC, CswNbtMetaDataNodeType relatedNT, CswNbtMetaDataNodeTypeProp[] propsToAdd )
        {
            foreach( CswNbtViewRelationship ChildRelationship in Relationships )
            {

                if( ( ChildRelationship.SecondId == relatedNT.NodeTypeId && ChildRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId ) ||
                    ( ChildRelationship.SecondId == relatedOC.ObjectClassId && ChildRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId ) )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Property in propsToAdd )
                    {
                        View.AddViewProperty( ChildRelationship, Property );
                    }
                }
                if( ChildRelationship.ChildRelationships.Count > 0 )
                {
                    _addPropertiesToView( View, ChildRelationship.ChildRelationships, relatedOC, relatedNT, propsToAdd );
                }
            }
            View.save();
        }

    }//class CswUpdateSchema_02F_Case29984

}//namespace ChemSW.Nbt.Schema