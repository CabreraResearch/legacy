using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29211
    /// </summary>
    public class CswUpdateSchema_02B_Case29211 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29211; }
        }

        public override void update()
        {
            // Add help text
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            if( null != UnitOfMeasureOC )
            {
                foreach( CswNbtMetaDataNodeType UnitOfMeasureNT in UnitOfMeasureOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp AliasesNTP = UnitOfMeasureNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );
                    AliasesNTP.HelpText = "Add aliases separated by a comma.";
                }
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29211

}//namespace ChemSW.Nbt.Schema