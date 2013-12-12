using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31308B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31308; }
        }
        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Format SqlScript property"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            foreach( CswNbtMetaDataNodeType PrintLabelNT in PrintLabelOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SqlScriptNTP = PrintLabelNT.getNodeTypePropByObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.SqlScript );
                CswNbtMetaDataNodeTypeProp NodeTypesNTP = PrintLabelNT.getNodeTypePropByObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );
                SqlScriptNTP.updateLayout( CswEnumNbtLayoutType.Edit, NodeTypesNTP, true );
                SqlScriptNTP.updateLayout( CswEnumNbtLayoutType.Add, NodeTypesNTP, true );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema