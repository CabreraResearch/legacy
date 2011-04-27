using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-14
    /// </summary>
    public class CswUpdateSchemaTo01H14 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 14 ); } }
        public CswUpdateSchemaTo01H14( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20620
            // Store all sequence values in field1_numeric

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
                {
                    if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode )
                    {
                        foreach( CswNbtNode Node in NodeType.getNodes( false, true ) )
                        {
                            // This will set the value of SequenceNumber correctly
                            Node.Properties[Prop].AsBarcode.setBarcodeValueOverride( Node.Properties[Prop].AsBarcode.Barcode, false );
                            Node.postChanges( false );
                        }
                    } 
                    else if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence )
                    {
                        foreach( CswNbtNode Node in NodeType.getNodes( false, true ) )
                        {
                            // This will set the value of SequenceNumber correctly
                            Node.Properties[Prop].AsSequence.setSequenceValueOverride( Node.Properties[Prop].AsSequence.Sequence, false );
                            Node.postChanges( false );
                        }
                    }
                } // foreach(CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps)
            } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )

            // Case 20312
            CswNbtMetaDataNodeType PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString(CswSchemaUpdater.HamletNodeTypes.Physical_Inspection) );
            Int32 SetupTabId;
            CswNbtMetaDataNodeTypeTab SetupTab = PhysicalInspectionNT.getNodeTypeTab( "Setup" );
            if( null != SetupTab )
                SetupTabId = SetupTab.TabId;
            else
                SetupTabId = PhysicalInspectionNT.getFirstNodeTypeTab().TabId;

            CswNbtMetaDataNodeTypeProp LocationProp = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            CswNbtMetaDataNodeTypeProp TargetNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataNodeTypeProp LocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
            LocationProp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), TargetNTP.PropId, CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString(), LocationNTP.PropId );
            LocationProp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), TargetNTP.PropId, CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString(), LocationNTP.PropId );

        } // update()

    }//class CswUpdateSchemaTo01H14

}//namespace ChemSW.Nbt.Schema


