using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-12
    /// </summary>
    public class CswUpdateSchemaTo01I12 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 12 ); } }

		public override void update()
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

            //Case 23132
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "session_data", "keepinquicklaunch" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "session_data", "keepinquicklaunch", "Determines whether to keep the item in User's Quick Launch", true, false );
            }
        }//Update()

    }//class CswUpdateSchemaTo01I12

}//namespace ChemSW.Nbt.Schema


