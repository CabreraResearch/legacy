using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27434
    /// </summary>
    public class CswUpdateSchemaCase27434 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp StatusNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.StatusPropertyName );
                StatusNtp.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp LocationVerifiedNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.LocationVerifiedPropertyName );
                LocationVerifiedNtp.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp SourceContainerNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.SourceContainerPropertyName );
                SourceContainerNtp.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp DisposedNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.DisposedPropertyName );
                DisposedNtp.removeFromAllLayouts();
            }

            CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNt )
            {
                CswNbtMetaDataFieldType Memo = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Memo );
                CswNbtMetaDataNodeTypeProp SahNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNt, Memo, "Storage and Handling" ) );
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNt.getNodeTypeTab( "Hazards" );
                if( null == HazardsTab )
                {
                    HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNt, "Hazards", Int32.MinValue );
                }
                SahNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId );

                CswNbtMetaDataFieldType Text = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Text );
                CswNbtMetaDataNodeTypeProp IsoNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNt, Text, "Isotope" ) );
                CswNbtMetaDataNodeTypeTab PhysicalTab = ChemicalNt.getNodeTypeTab( "Physical" );
                if( null == PhysicalTab )
                {
                    PhysicalTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNt, "Physical", Int32.MinValue );
                }
                IsoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, PhysicalTab.TabId );
            }

        }//Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema