using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case31315 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31315; }
        }

        public override string Title
        {
            get { return "Remove options from PPE Chemical property"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Remove the listoptions from the PPE property of the Chemical object class
            // NOTE: We alter the table directly here because we don't want to alter the NTPs
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp PPEOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.PPE );
            CswTableUpdate OCPTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "removePPElistoptions_Case31315", "object_class_props" );
            DataTable OCPTbl = OCPTblUpdate.getTable( "where objectclassid = " + ChemicalOC.ObjectClassId + " and propname = '" + PPEOCP.PropName + "'" );
            if( OCPTbl.Rows.Count > 0 )
            {
                OCPTbl.Rows[0]["listoptions"] = "";
            }
            OCPTblUpdate.update( OCPTbl );

        } // update()
    }

}//namespace ChemSW.Nbt.Schema