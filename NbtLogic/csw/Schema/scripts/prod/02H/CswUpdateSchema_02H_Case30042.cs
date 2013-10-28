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

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "B"; }
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

            // todo: Add the material name to the ghs name template


        } // update()

    }

}//namespace ChemSW.Nbt.Schema