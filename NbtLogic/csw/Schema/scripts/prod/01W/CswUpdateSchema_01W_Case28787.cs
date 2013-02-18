using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28787
    /// </summary>
    public class CswUpdateSchema_01W_Case28787 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28787; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp documentNTP = materialNT.getNodeTypeProp( "Documents" );
                if( null != documentNTP )
                {
                    CswNbtView documentsPropView = _CswNbtSchemaModTrnsctn.restoreView( documentNTP.ViewId );
                    foreach( CswNbtViewProperty viewProp in documentsPropView.getOrderedViewProps( true ) )
                    {
                        if( viewProp.Name.Equals( CswNbtObjClassDocument.PropertyName.Archived ) )
                        {
                            documentsPropView.AddViewPropertyFilter( viewProp,
                                Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.Or,
                                SubFieldName: CswNbtSubField.SubFieldName.Checked,
                                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                Value: CswConvert.ToDbVal( true ).ToString()
                            );
                        }
                    }
                    documentsPropView.save();

                } // if( null != documentNTP )
            } //foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )

        } //Update()

    }//class CswUpdateSchema_01W_Case28787

}//namespace ChemSW.Nbt.Schema