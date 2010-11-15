using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-09
    /// </summary>
    public class CswUpdateSchemaTo01H09 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 09 ); } }
        public CswUpdateSchemaTo01H09( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20094
            CswNbtMetaDataNodeType FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Fire_Extinguisher ) );
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            CswNbtView FireExtinguisherView = _CswNbtSchemaModTrnsctn.makeView();
            FireExtinguisherView.makeNew( "All Fire Extinguishers", NbtViewVisibility.Global, null, null, null );
            FireExtinguisherView.Category = "Search";
            FireExtinguisherView.SetViewMode(NbtViewRenderingMode.Grid);
            
            CswNbtViewRelationship FExtinguisherRelationship = FireExtinguisherView.AddViewRelationship( FireExtinguisherNT, false );
            CswNbtMetaDataNodeTypeProp FEBarcodeNTP = FireExtinguisherNT.getNodeTypeProp( "Barcode" );
            CswNbtMetaDataNodeTypeProp FEDescriptionNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.DescriptionPropertyName );
            CswNbtMetaDataNodeTypeProp FEMountPointNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.MountPointPropertyName );
            CswNbtMetaDataNodeTypeProp FEStatusNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.StatusPropertyName );
            CswNbtViewProperty FEBarcodeVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEBarcodeNTP );
            FEBarcodeVP.Order = 1;
            CswNbtViewPropertyFilter FEBarcodeVPF = FireExtinguisherView.AddViewPropertyFilter( FEBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

            CswNbtViewProperty FEStatusVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEStatusNTP );
            FEStatusVP.Order = 2;
            CswNbtViewPropertyFilter FEStatusVPF = FireExtinguisherView.AddViewPropertyFilter( FEStatusVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
            
            CswNbtViewProperty FEMountPointVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEMountPointNTP );
            FEMountPointVP.Order = 3;
            CswNbtViewPropertyFilter FEMountPointVPF = FireExtinguisherView.AddViewPropertyFilter( FEMountPointVP, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

            CswNbtViewProperty FEDescriptionVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEDescriptionNTP );
            FEDescriptionVP.Order = 4;
            CswNbtViewPropertyFilter FEDescriptionVPF = FireExtinguisherView.AddViewPropertyFilter( FEDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

            CswNbtViewRelationship MPointRelationship = FireExtinguisherView.AddViewRelationship( FExtinguisherRelationship, CswNbtViewRelationship.PropOwnerType.First, FEMountPointNTP, false );
            CswNbtMetaDataNodeTypeProp MPLocationNTP = MountPointNT.getNodeTypeProp( CswNbtObjClassMountPoint.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp MPDescriptionNTP = MountPointNT.getNodeTypeProp( CswNbtObjClassMountPoint.DescriptionPropertyName );
            CswNbtViewProperty MPLocationVP = FireExtinguisherView.AddViewProperty( MPointRelationship, MPLocationNTP );
            FEBarcodeVP.Order = 5;
            CswNbtViewPropertyFilter MPLocationVPF = FireExtinguisherView.AddViewPropertyFilter( MPLocationVP, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

            CswNbtViewProperty MPDescriptionVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, MPDescriptionNTP );
            FEBarcodeVP.Order = 6;
            CswNbtViewPropertyFilter MPDescriptionVPF = FireExtinguisherView.AddViewPropertyFilter( MPDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
            
            FireExtinguisherView.save();

        } // update()

    }//class CswUpdateSchemaTo01H09

}//namespace ChemSW.Nbt.Schema


