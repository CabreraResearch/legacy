using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52280B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "MLM2: Add view to Manufacturing Sites Grid property"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // Create the view
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );

            CswNbtMetaDataObjectClass ManufacturerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass );
            CswNbtMetaDataObjectClassProp MaterialOCP = ManufacturerOC.getObjectClassProp( CswNbtObjClassManufacturer.PropertyName.Material );
            CswNbtMetaDataObjectClassProp ManufacturingSiteOCP = ManufacturerOC.getObjectClassProp( CswNbtObjClassManufacturer.PropertyName.ManufacturingSite );
            CswNbtMetaDataObjectClassProp QualifiedOCP = ManufacturerOC.getObjectClassProp( CswNbtObjClassManufacturer.PropertyName.Qualified );

            CswNbtView ManufacturersView = _CswNbtSchemaModTrnsctn.makeSafeView( "ManufacturingSites", CswEnumNbtViewVisibility.Property );
            ManufacturersView.ViewName = "ManufacturingSites";
            ManufacturersView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
            ManufacturersView.Root.ChildRelationships.Clear();

            CswNbtViewRelationship root = ManufacturersView.AddViewRelationship( MaterialPS, false );
            CswNbtViewRelationship relationship = ManufacturersView.AddViewRelationship( root, CswEnumNbtViewPropOwnerType.Second, MaterialOCP, true );

            ManufacturersView.AddViewProperty( relationship, ManufacturingSiteOCP );
            ManufacturersView.AddViewProperty( relationship, QualifiedOCP );

            ManufacturersView.save();

            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp GridOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ManufacturingSites );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, ManufacturersView.ToString() );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema