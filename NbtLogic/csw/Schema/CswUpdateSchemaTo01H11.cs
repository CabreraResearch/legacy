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
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-11
    /// </summary>
    public class CswUpdateSchemaTo01H11 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 11 ); } }
        public CswUpdateSchemaTo01H11( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // Case 20083 - set MobileSearch for nodetypeprops
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.FE_Inspection_Point ) );
            CswNbtMetaDataNodeTypeProp MPBarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
            MPBarcodeNTP.MobileSearch = true;
            CswNbtMetaDataNodeTypeProp MPLocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
            MPLocationNTP.MobileSearch = true;
            
            CswNbtMetaDataNodeType PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            CswNbtMetaDataNodeTypeProp PITargetNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            PITargetNTP.MobileSearch = true;

        } // update()

    }//class CswUpdateSchemaTo01H11

}//namespace ChemSW.Nbt.Schema


