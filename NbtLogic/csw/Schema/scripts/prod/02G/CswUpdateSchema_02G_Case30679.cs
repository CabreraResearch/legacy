using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30679 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Put GHSPictos on Chemical Preview"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30679; }
        }

        public override string ScriptName
        {
            get { return "Case30679NT"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp GHSNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Jurisdiction );
                if( null != GHSNTP )
                {
                    GHSNTP.updateLayout( CswEnumNbtLayoutType.Preview, true );
                }
            }
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GHSNT in GHSOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PictosNTP = GHSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms );
                foreach( CswNbtMetaDataNodeTypeProp GHSNTP in GHSNT.getNodeTypeProps() )
                {
                    if( null != PictosNTP && GHSNTP.PropId == PictosNTP.PropId )
                    {
                        GHSNTP.updateLayout( CswEnumNbtLayoutType.Preview, true );
                    }
                    else
                    {
                        GHSNTP.removeFromLayout( CswEnumNbtLayoutType.Preview );
                    }
                }
            }
        }
    }
}