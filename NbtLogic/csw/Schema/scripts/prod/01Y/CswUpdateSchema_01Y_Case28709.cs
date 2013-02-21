using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28709
    /// </summary>
    public class CswUpdateSchema_01Y_Case28709 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28709; }
        }

        public override void update()
        {

            //Make the Container.Missing checkbox prop readonly (so only admins can edit it)
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( int containerNTId in containerOC.getNodeTypeIds() )
            {
                CswNbtMetaDataNodeTypeProp missingNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( containerNTId, CswNbtObjClassContainer.PropertyName.Missing );
                missingNTP.ReadOnly = true;
            }

        } //Update()

    }//class CswUpdateSchema_01Y_Case28709

}//namespace ChemSW.Nbt.Schema