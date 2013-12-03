using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31233 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Fix existing GHSPicto Paths"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31233; }
        }

        public override void update()
        {
            // This fixes what CswUpdateSchema_02G_Case30473 failed to do correctly

            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )
            {
                foreach( CswNbtObjClassGHS GhsNode in GhsNT.getNodes( false, true ) )
                {
                    CswDelimitedString oldVals = GhsNode.Pictograms.Value;
                    CswDelimitedString newVals = new CswDelimitedString( CswNbtNodePropImageList.Delimiter );
                    foreach( string oldVal in oldVals )
                    {
                        if( oldVal.IndexOf( "/ghs/" ) >= 0 )
                        {
                            char testChar = oldVal[( oldVal.IndexOf( "/ghs/" ) + "/ghs/".Length )];
                            string newVal;
                            if( CswTools.IsNumeric( testChar ) )
                            {
                                newVal = oldVal.Replace( "/ghs/600/", "/ghs/512/" );
                            }
                            else
                            {
                                newVal = oldVal.Replace( "/ghs/", "/ghs/512/" );
                            }
                            newVals.Add( newVal );
                        }
                    }
                    GhsNode.Pictograms.Value = newVals;
                    GhsNode.postChanges( false );
                } // foreach( CswNbtObjClassGHS GhsNode in GhsNT.getNodes( false, true ) )
            } // foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )

        } // update()
    } // class 
} // namespace