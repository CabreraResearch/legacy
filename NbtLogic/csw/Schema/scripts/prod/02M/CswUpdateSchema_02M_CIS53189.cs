using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53189 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 53189; }
        }

        public override string Title
        {
            get { return "Fix Biological properties"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            // Biological NTP Design Nodes have the wrong value for Original Name -- they point to NonChemical, not Biological
            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );
            foreach( CswNbtMetaDataObjectClassProp propOCP in BiologicalOC.getObjectClassProps() )
            {
                foreach( CswNbtMetaDataNodeTypeProp propNTP in propOCP.getNodeTypeProps() )
                {
                    ( (CswNbtObjClassDesignNodeTypeProp) propNTP.DesignNode ).ObjectClassPropName.Value = propOCP.ObjectClassPropId.ToString();
                }
            }
        } // update()

    }
}