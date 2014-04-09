using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53248 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53248; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ": MLM2: CertDef nodetype creation and configuration"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        private List<CswNbtMetaDataNodeType> _newNodeTypes;

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass );
            _newNodeTypes = new List<CswNbtMetaDataNodeType>();

            _createNodeType( CertDefOC, "CertDef SPX" );
            _createNodeType( CertDefOC, "CertDef SPR" );
            _createNodeType( CertDefOC, "CertDef EP" );

            foreach( CswNbtMetaDataNodeType nt in _newNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab FirstTab = nt.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeTab IdentityTab = nt.getIdentityTab();
                CswNbtMetaDataNodeTypeTab SpecsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( nt, "Specifications" );
                CswNbtMetaDataNodeTypeTab VersionsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( nt, "Versions" );

                // Properties on Identity Tab
                CswNbtMetaDataNodeTypeProp certdefid = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.CertDefId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, certdefid, true, IdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, nt.NodeTypeId, certdefid, true );

                CswNbtMetaDataNodeTypeProp material = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.Material );
                material.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.View].AsViewReference.ViewId = _createMaterialPropertyView( material, nt.NodeTypeName );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, material, true, IdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, nt.NodeTypeId, material, true );

                CswNbtMetaDataNodeTypeProp version = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.Version );

                CswNbtMetaDataNodeTypeProp newdraft = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.NewDraft );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, newdraft, true, IdentityTab.TabId );
                newdraft.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Properties on Specs Tab
                CswNbtMetaDataNodeTypeProp certdefspecs = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.CertDefSpecs );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, certdefspecs, true, SpecsTab.TabId );
                certdefspecs.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Properties on Versions Tab 
                CswNbtMetaDataNodeTypeProp versions = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.Versions );

                // Properties on CertDef Tab
                CswNbtMetaDataNodeTypeProp currentapproved = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.CurrentApproved );
                CswNbtMetaDataNodeTypeProp obsolete = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.Obsolete );
                CswNbtMetaDataNodeTypeProp retaincount = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.RetainCount );
                CswNbtMetaDataNodeTypeProp retainquantity = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.RetainQuantity );
                CswNbtMetaDataNodeTypeProp retainexpriation = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.RetainExpiration );
                CswNbtMetaDataNodeTypeProp approved = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.Approved );
                CswNbtMetaDataNodeTypeProp approveddate = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.ApprovedDate );
                CswNbtMetaDataNodeTypeProp qualifiedmanufonly = nt.getNodeTypePropByObjectClassProp( CswNbtObjClassCertificateDefinition.PropertyName.QualifiedManufacturerOnly );

                if( nt.NodeTypeName != "CertDef SPX" )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, version, true, IdentityTab.TabId );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, nt.NodeTypeId, version, true );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, versions, true, VersionsTab.TabId );
                    versions.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, currentapproved, true, FirstTab.TabId );
                    currentapproved.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, obsolete, true, FirstTab.TabId );
                    obsolete.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, retaincount, true, FirstTab.TabId );
                    retaincount.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, retainquantity, true, FirstTab.TabId );
                    retainquantity.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, retainexpriation, true, FirstTab.TabId );
                    retainexpriation.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, approved, true, FirstTab.TabId );
                    approved.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, approveddate, true, FirstTab.TabId );
                    approveddate.removeFromLayout( CswEnumNbtLayoutType.Add );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nt.NodeTypeId, qualifiedmanufonly, true, FirstTab.TabId );
                    qualifiedmanufonly.removeFromLayout( CswEnumNbtLayoutType.Add );
                }
                else
                {
                    version.removeFromAllLayouts();
                    versions.removeFromAllLayouts();
                    currentapproved.removeFromAllLayouts();
                    obsolete.removeFromAllLayouts();
                    retaincount.removeFromAllLayouts();
                    retainquantity.removeFromAllLayouts();
                    retainexpriation.removeFromAllLayouts();
                    approved.removeFromAllLayouts();
                    approveddate.removeFromAllLayouts();
                    qualifiedmanufonly.removeFromAllLayouts();
                }
            }

        }//update()

        private void _createNodeType( CswNbtMetaDataObjectClass objectClass, string nodeTypeName )
        {
            CswNbtMetaDataNodeType NewNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( objectClass )
            {
                Category = "MLM",
                IconFileName = "doc.png",
                NodeTypeName = nodeTypeName,
                NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertificateDefinition.PropertyName.CertDefId )
            } );

            _newNodeTypes.Add( NewNodeType );
        }//_createNodeType()

        private CswNbtViewId _createMaterialPropertyView( CswNbtMetaDataNodeTypeProp materialprop, string nodetype )
        {
            CswNbtViewId ViewId = null;
            CswNbtView View = _CswNbtSchemaModTrnsctn.makeSafeView( "CertDefMaterialProp_" + nodetype, CswEnumNbtViewVisibility.Property );

            if( nodetype == "CertDef EP" )
            {
                CswNbtMetaDataObjectClass EPOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EnterprisePartClass );
                View.AddViewRelationship( EPOC, true );
                materialprop.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Target].AsMetaDataList.setValue( CswEnumNbtViewRelatedIdType.ObjectClassId, EPOC.ObjectClassId );
            }
            else
            {
                CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( "MaterialSet" );
                View.AddViewRelationship( MaterialPS, true );
                materialprop.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Target].AsMetaDataList.setValue( CswEnumNbtViewRelatedIdType.PropertySetId, MaterialPS.PropertySetId );
            }

            View.save();
            ViewId = View.ViewId;



            return ViewId;
        }
    }
}