using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29039
    /// </summary>
    public class CswUpdateSchema_01Y_Case29039_Generators: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29039; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp TargetTypeOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );

            foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TargetType = GeneratorNt.getNodeTypePropByObjectClassProp( TargetTypeOcp.ObjectClassPropId );
                TargetType.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }
        } //Update()

    }//class CswUpdateSchema_01Y_CaseXXXXX

}//namespace ChemSW.Nbt.Schema