using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29431: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29431; }
        }
        
        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                //Case 29431
                CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
                CswNbtObjClassInventoryGroup IgNode = null;
                foreach( CswNbtNode Node in InventoryGroupOc.getNodes( forceReInit: true, IncludeDefaultFilters: false, IncludeHiddenNodes: false, includeSystemNodes: false )
                    .Where( Node => Node.NodeName.ToLower().Trim() == "default inventory group" ) )
                {
                    IgNode = Node;
                    break;
                }
                if( null != IgNode )
                {
                    CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                    CswNbtMetaDataObjectClassProp InGroupOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( InGroupOcp, IgNode.NodeId.PrimaryKey, SubFieldName : CswEnumNbtSubFieldName.NodeID );
                }

                //Requested from Case 29436: Set Label Format default value
                CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
                CswNbtObjClassPrintLabel PlNode = null;
                foreach( CswNbtNode Node in PrintLabelOc.getNodes( forceReInit : true, IncludeDefaultFilters : false, IncludeHiddenNodes : false, includeSystemNodes : false )
                    .Where( Node => Node.NodeName.ToLower().Trim() == "default barcode label" ) )
                {
                    PlNode = Node;
                    break;
                }
                if( null != PlNode )
                {
                    CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                    CswNbtMetaDataObjectClassProp PrintLabelOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue(PrintLabelOcp, PlNode.NodeId.PrimaryKey, SubFieldName: CswEnumNbtSubFieldName.NodeID );
                }

            }
        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema