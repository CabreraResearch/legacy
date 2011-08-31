using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-12
    /// </summary>
    public class CswUpdateSchemaTo01I12 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 12 ); } }
        public CswUpdateSchemaTo01I12( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserDtp = UserOC.getObjectClassProp( CswNbtObjClassUser.DateFormatPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserDtp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, "M/d/yyyy,d-M-yyyy,yyyy/M/d,dd MMM yyyy" );
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            //Case 23130
            _CswNbtSchemaModTrnsctn.UpdateS4( "getSimultaneousUsage", @"select a.statisticsid, b.statisticsid simultaneousid
                                                                       from statistics a
                                                                       join statistics b on a.logindate <= b.logoutdate
                                                                                        and a.logoutdate >= b.logindate
                                                                                        and a.statisticsid <> b.statisticsid
                                                                      where a.logoutdate < #getbeforedate
                                                                        and a.logoutdate > #getafterdate
                                                                      order by a.statisticsid, b.statisticsid" );

            //Case 23156
            List<CswNbtView> MpViews = _CswNbtSchemaModTrnsctn.restoreViews( "Mount Points" );
            foreach( CswNbtView View in MpViews )
            {
                View.ViewName = "FE Inspection Points";
                View.save();
            }

            List<CswNbtView> MpgViews = _CswNbtSchemaModTrnsctn.restoreViews( "Mount Point Groups" );
            foreach( CswNbtView View in MpgViews )
            {
                View.ViewName = "FE Inspection Point Groups";
                View.save();
            }

        }//Update()

    }//class CswUpdateSchemaTo01I12

}//namespace ChemSW.Nbt.Schema


