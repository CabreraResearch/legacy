using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52266: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52266; }
        }

        public override string Title
        {
            get { return "Unhide all container dispose/undispose buttons"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp UndisposeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Undispose );
            CswNbtMetaDataObjectClassProp DisposeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Dispose );

            CswCommaDelimitedString PropIds = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp UndisposeNTP in UndisposeOCP.getNodeTypeProps() )
            {
                PropIds.Add( UndisposeNTP.PropId.ToString() );
            }
            foreach( CswNbtMetaDataNodeTypeProp DisposeNTP in DisposeOCP.getNodeTypeProps() )
            {
                PropIds.Add( DisposeNTP.PropId.ToString() );
            }

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update jct_nodes_props jnp set hidden = 0 where nodetypepropid in (" + PropIds + ")" );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema