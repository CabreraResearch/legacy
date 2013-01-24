using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28409
    /// </summary>
    public class CswUpdateSchema_01W_Case28409 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28409; }
        }

        public override void update()
        {
            // Add 'Event Type' to Schedule Add form
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            foreach( CswNbtMetaDataNodeType GeneratorNT in GeneratorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp EventTypeNTP = GeneratorNT.getNodeTypeProp( "Event Type" );
                if( null != EventTypeNTP )
                {
                    EventTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, GeneratorNT.getNodeTypeProp( "Technician" ), true );
                }
            }
        } // update()

    }//class CswUpdateSchema_01V_Case28409

}//namespace ChemSW.Nbt.Schema