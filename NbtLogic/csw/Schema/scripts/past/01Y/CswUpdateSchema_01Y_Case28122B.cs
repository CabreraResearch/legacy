using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28122B
    /// </summary>
    public class CswUpdateSchema_01Y_Case28122B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28122; }
        }

        public override void update()
        {
            //Add Legacy Id NTP to all NTs and make sure it's hidden

            string LegacyIdPropName = "Legacy Id";
            CswNbtMetaDataFieldType numberFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Number );

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp legacyIdNTP = NodeType.getNodeTypeProp( LegacyIdPropName );
                if( null == legacyIdNTP )
                {
                    legacyIdNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( NodeType, numberFT, LegacyIdPropName ) );
                    legacyIdNTP.ServerManaged = true;
                }
                legacyIdNTP.removeFromAllLayouts();
            }

        } //Update()

    }//class CswUpdateSchema_01Y_Case28122B

}//namespace ChemSW.Nbt.Schema