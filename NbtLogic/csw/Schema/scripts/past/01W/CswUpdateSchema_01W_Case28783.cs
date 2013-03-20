using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28783
    /// </summary>
    public class CswUpdateSchema_01W_Case28783 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28783; }
        }

        public override void update()
        {
            // Add storage compatibility to location preview
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp StorageCompatOCP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                StorageCompatOCP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, false );
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28783

}//namespace ChemSW.Nbt.Schema