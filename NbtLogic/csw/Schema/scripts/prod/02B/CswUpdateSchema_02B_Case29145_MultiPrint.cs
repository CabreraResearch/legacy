using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29145_MultiPrint : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
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

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                NodeType.HasLabel = ( SelectedNodeTypeIds.Contains( NodeType.FirstVersionNodeTypeId ) || 
                    SelectedNodeTypeIds.Contains( NodeType.NodeTypeId )  ||
                    SelectedNodeTypeIds.Contains( NodeType.getNodeTypeLatestVersion().NodeTypeId ) );
            }
        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29145; }
        }

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema