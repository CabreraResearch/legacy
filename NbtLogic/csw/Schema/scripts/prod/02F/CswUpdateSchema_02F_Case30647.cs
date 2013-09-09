using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30647 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30647; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp SizeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Size );

            // To be compatible with case 27330, remove 'Size' from Container layout by default
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                foreach( CswNbtMetaDataNodeTypeProp SizeNTP in SizeOCP.getNodeTypeProps() )
                {
                    SizeNTP.removeFromAllLayouts();
                }
            }

            // To resolve case 30647, clear out .Hidden flag on 'Size'
            foreach( CswNbtMetaDataNodeTypeProp SizeNTP in SizeOCP.getNodeTypeProps() )
            {
                SizeNTP.Hidden = false;
                foreach( CswNbtObjClassContainer ContainerNode in ContainerOC.getNodes( false, true ) )
                {
                    ContainerNode.Size.setHidden( false, true );
                    ContainerNode.postChanges( false );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema