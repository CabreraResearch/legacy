using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02M_CIS53197 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 53197; }
        }

        public override string Title
        {
            get { return "Fix design node sync"; }
        }

        public override void update()
        {
            // Repeat edits in Larch to object class props

            // From CswUpdateMetaData_02L_Case31750
            CswNbtMetaDataObjectClass UnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp NameOCP = UnitOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOCP, CswEnumNbtObjectClassPropAttributes.isglobalunique, true );

            // From CswUpdateMetaData_02L_Case52821
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtMetaDataObjectClassProp CzNameOCP = ControlZoneOC.getObjectClassProp( CswNbtObjClassControlZone.PropertyName.ControlZoneName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CzNameOCP, CswEnumNbtObjectClassPropAttributes.propname, "Control Zone Name" );

            // update the name template, too
            foreach( CswNbtMetaDataNodeType ControlZoneNT in ControlZoneOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ControlZoneNameNTP = ControlZoneNT.getNodeTypePropByObjectClassProp( CswNbtObjClassControlZone.PropertyName.ControlZoneName );
                ControlZoneNT.DesignNode.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( ControlZoneNameNTP.PropName );
            }

            // From CswUpdateMetaData_02L_Case52284
            CswNbtMetaDataObjectClass MEPOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass );
            CswNbtMetaDataObjectClass ManufacturerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass );
            if( null != ManufacturerOC && null != MEPOC )
            {
                CswNbtMetaDataObjectClassProp ManufacturerOCP = MEPOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
                if( null != ManufacturerOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.fktype, CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.fkvalue, ManufacturerOC.ObjectClassId.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
                    CswNbtMetaDataObjectClassProp EPOCP = MEPOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EPOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
                    foreach( CswNbtMetaDataNodeType MEPNT in MEPOC.getNodeTypes() )
                    {
                        CswNbtMetaDataNodeTypeProp ManufacturerNTP = MEPNT.getNodeTypePropByObjectClassProp( ManufacturerOCP );
                        ManufacturerNTP.DesignNode.syncFromObjectClassProp();
                        CswNbtMetaDataNodeTypeProp EPNTP = MEPNT.getNodeTypePropByObjectClassProp( EPOCP );
                        EPNTP.DesignNode.syncFromObjectClassProp();
                    }
                }
            }
        } // update()

    }//class

}//namespace ChemSW.Nbt.Schema