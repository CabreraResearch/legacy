using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31893 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31893; }
        }

        public override string AppendToScriptName()
        {
            return "G";
        }

        public override string Title
        {
            get { return "Remove Legacy Material Id property from all layouts"; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass objectClass in MaterialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType nodeType in objectClass.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp LegacyMaterialIdNTP = nodeType.getNodeTypeProp( CswNbtPropertySetMaterial.PropertyName.LegacyMaterialId );
                    if( null != LegacyMaterialIdNTP )
                    {
                        LegacyMaterialIdNTP.removeFromAllLayouts();
                    }
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema