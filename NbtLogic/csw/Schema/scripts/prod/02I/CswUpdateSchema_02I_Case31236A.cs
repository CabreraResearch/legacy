using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31236A: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add missing option to Hazard Categories"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31236; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp HazCats = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardCategories );

            CswCommaDelimitedString ListOpts = new CswCommaDelimitedString( HazCats.ListOptions );
            ListOpts.Add( "C = Chronic" );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( HazCats, CswEnumNbtObjectClassPropAttributes.listoptions, ListOpts.ToString() );

        }

    }
}