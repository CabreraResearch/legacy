using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30473 : CswUpdateSchemaTo
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
            get { return "Case30473"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp GhsPictogramsNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms );

                CswDelimitedString PictoPaths = new CswDelimitedString( '\n' )
                    {
                        "Images/cispro/ghs/600/rondflam.jpg",
                        "Images/cispro/ghs/600/flamme.jpg",
                        "Images/cispro/ghs/600/explos.jpg",
                        "Images/cispro/ghs/600/skull.jpg",
                        "Images/cispro/ghs/600/acid.jpg",
                        "Images/cispro/ghs/600/bottle.jpg",
                        "Images/cispro/ghs/600/silhouet.jpg",
                        "Images/cispro/ghs/600/pollut.jpg",
                        "Images/cispro/ghs/600/exclam.jpg"
                    };
                GhsPictogramsNTP.ValueOptions = PictoPaths.ToString();

                foreach( CswNbtObjClassGHS GhsNode in GhsNT.getNodes( false, true ) )
                {
                    GhsNode.Pictograms.Value.Replace( "/ghs/", "/ghs/600/" );
                    GhsNode.postChanges( false );
                }
            } // foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )

        } // update()
    } // class 
} // namespace