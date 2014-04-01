using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52281 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52281; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Qualified Tab on Manufacturer"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ManufacturerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass );
            foreach( CswNbtMetaDataNodeType ManufacturerNT in ManufacturerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab QualifiedNTT = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ManufacturerNT, "Qualified" );
                CswNbtMetaDataNodeTypeProp QualifiedNTP = ManufacturerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassManufacturer.PropertyName.Qualified );
                QualifiedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, QualifiedNTT.TabId );
            }
            
        } // update()
    }
}//namespace ChemSW.Nbt.Schema