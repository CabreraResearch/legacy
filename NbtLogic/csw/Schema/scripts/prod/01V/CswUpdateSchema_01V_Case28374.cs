using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28374
    /// </summary>
    public class CswUpdateSchema_01V_Case28374 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28374; }
        }

        public override void update()
        {
            //9 - rename MSDS to SDS everywhere
            CswNbtMetaDataObjectClass DocumentClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            foreach(CswNbtMetaDataNodeType DocumentNT in DocumentClass.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeProp DocumentClassNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
                DocumentClassNTP.ListOptions = DocumentClassNTP.ListOptions.Replace( "MSDS", "SDS" );
                DocumentClassNTP.DefaultValue.AsList.Value = DocumentClassNTP.DefaultValue.AsList.Value.Replace( "MSDS", "SDS" );
                DocumentClassNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LanguageNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                LanguageNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                LanguageNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp FormatNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                FormatNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                FormatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp IssueDateNTP = DocumentNT.getNodeTypeProp( "Issue Date" );                
                if( null != IssueDateNTP )
                {
                    IssueDateNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                    IssueDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                }                                
            }
            foreach( CswNbtObjClassDocument DocumentNode in DocumentClass.getNodes( false, false ) )
            {
                if( DocumentNode.DocumentClass.Value == "MSDS" )
                {
                    DocumentNode.DocumentClass.Value = "SDS";
                    DocumentNode.postChanges( false );
                }
            }
            CswNbtView MSDSView = _CswNbtSchemaModTrnsctn.restoreView( "MSDS Expiring Next Month" );
            if( null != MSDSView )
            {
                MSDSView.ViewName = "SDS Expiring Next Month";
                MSDSView.save();
            }

        }//Update()

    }//class CswUpdateSchemaCase_01V_28374

}//namespace ChemSW.Nbt.Schema