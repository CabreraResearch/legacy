using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31315 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 31315; }
        }

        public override string Title
        {
            get { return "Update PPE listoptions on Chemical NT NTPs"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // Update the PPE list options on all Chemical NTs
            const string UpdatedPPEOptions = "Goggles,Gloves,Clothing,Fume Hood,Respirator,Dust Mask,Face Shield,Isolation Lab,Lab Coat,Safety Cabinet";

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PPENTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.PPE );
                if( null != PPENTP )
                {
                    PPENTP.ListOptions = UpdatedPPEOptions;
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema