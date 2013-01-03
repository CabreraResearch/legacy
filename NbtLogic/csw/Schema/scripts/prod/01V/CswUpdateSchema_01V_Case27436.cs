using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27436
    /// </summary>
    public class CswUpdateSchema_01V_Case27436 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27436; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GHSClass );
            CswNbtMetaDataNodeType GhsNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS" );
            if( null != GhsNT )
            {
                CswNbtMetaDataNodeTypeProp GhsPhraseNTP = GhsNT.getNodeTypeProp( "GHS Phrase" );
                CswNbtMetaDataNodeTypeProp GhsTypeNTP = GhsNT.getNodeTypeProp( "Type" );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( GhsTypeNTP );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( GhsPhraseNTP );

                _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( GhsNT, GhsOC );
            } // if( null != GhsNT )
        } //Update()

    }//class CswUpdateSchema_01V_Case27436

}//namespace ChemSW.Nbt.Schema