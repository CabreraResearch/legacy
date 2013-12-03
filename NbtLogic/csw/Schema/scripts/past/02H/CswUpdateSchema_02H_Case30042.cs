using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30042 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30042; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Hide CAF only Chemical prop; add prop to GHS name template"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NodetypeProp = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.AddLabelCodes );
                //NodetypeProp.Hidden = true;
                NodetypeProp.removeFromAllLayouts();
            }

            // Add material to name template for ghs nodetypes
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GHSNT in GHSOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp JurisdictionNTP = GHSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
                CswNbtMetaDataNodeTypeProp MaterialNTP = GHSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                GHSNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( JurisdictionNTP.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( MaterialNTP.PropName ) );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema