using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27230
    /// </summary>
    public class CswUpdateSchemaCase27230 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType containerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != containerNT )
            {
                CswNbtMetaDataNodeTypeProp docsNTP = containerNT.getNodeTypeProp( "Documents" );
                if( null != docsNTP )
                {
                    CswNbtView docsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( docsNTP.ViewId );
                    foreach( CswNbtViewProperty viewProp in docsView.getOrderedViewProps( false ) )
                    {
                        if( viewProp.Name.Equals( "Disposed" ) && viewProp.Filters.Count > 0 )
                        {
                            viewProp.removeFilter( (CswNbtViewPropertyFilter) viewProp.Filters[0] );
                            docsView.save();
                        }
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27230

}//namespace ChemSW.Nbt.Schema