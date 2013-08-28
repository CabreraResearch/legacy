using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30577: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30577; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp MaterialTypeOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.MaterialType );

            

            foreach( CswNbtMetaDataNodeTypeProp MaterialTypeNTP in MaterialTypeOCP.getNodeTypeProps() )
            {
                MaterialTypeNTP.IsRequired = false;
                
            }

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MaterialTypeOCP, string.Empty );

        } // update()

    }//class CswUpdateSchema_02F_Case30577
}//namespace ChemSW.Nbt.Schema