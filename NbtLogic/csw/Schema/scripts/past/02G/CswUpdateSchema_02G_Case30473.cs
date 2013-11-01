using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30473_again : CswUpdateSchemaTo
    {
        public override string Title { get { return "Update GHSPicto Paths"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30473; }
        }

        public override string ScriptName
        {
            get { return "Case30473_again"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp GhsPictogramsNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms );

                CswDelimitedString PictoPaths = new CswDelimitedString( '\n' )
                    {
                        "Images/cispro/ghs/512/rondflam.jpg",
                        "Images/cispro/ghs/512/flamme.jpg",
                        "Images/cispro/ghs/512/explos.jpg",
                        "Images/cispro/ghs/512/skull.jpg",
                        "Images/cispro/ghs/512/acid.jpg",
                        "Images/cispro/ghs/512/bottle.jpg",
                        "Images/cispro/ghs/512/silhouet.jpg",
                        "Images/cispro/ghs/512/pollut.jpg",
                        "Images/cispro/ghs/512/exclam.jpg"
                    };
                GhsPictogramsNTP.ValueOptions = PictoPaths.ToString();

                foreach( CswNbtObjClassGHS GhsNode in GhsNT.getNodes( false, true ) )
                {
                    if( GhsNode.Pictograms.Value.Contains( "/ghs/600/" ) )
                    {
                        GhsNode.Pictograms.Value.Replace( "/ghs/600/", "/ghs/512/" );
                    }
                    else
                    {
                        GhsNode.Pictograms.Value.Replace( "/ghs/", "/ghs/512/" );
                    }
                    GhsNode.postChanges( false );
                }
            } // foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )

        } // update()
    } // class 
} // namespace