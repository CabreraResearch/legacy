using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 21701
    /// </summary>
    public class CswUpdateSchema_01S_PrintLabel_Case21701 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            Collection<Int32> SelectedNodeTypeIds = new Collection<Int32>();
            foreach( CswNbtObjClassPrintLabel Node in PrintLabelOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
            {
                if( null != Node &&
                    null != Node.NodeTypes.SelectedNodeTypeIds &&
                    Node.NodeTypes.SelectedNodeTypeIds.Count > 0 )
                {
                    foreach( Int32 SelectedNodeTypeid in Node.NodeTypes.SelectedNodeTypeIds.ToIntCollection() )
                    {
                        if( false == SelectedNodeTypeIds.Contains( SelectedNodeTypeid ) )
                        {
                            SelectedNodeTypeIds.Add( SelectedNodeTypeid );
                        }
                    }
                }
            }

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypesLatestVersion() )
            {
                NodeType.HasLabel = ( SelectedNodeTypeIds.Contains( NodeType.FirstVersionNodeTypeId ) || SelectedNodeTypeIds.Contains( NodeType.NodeTypeId ) );
            }
        }//Update()

    }

}//namespace ChemSW.Nbt.Schema