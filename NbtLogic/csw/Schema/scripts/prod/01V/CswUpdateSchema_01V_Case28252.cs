using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28252
    /// </summary>
    public class CswUpdateSchema_01V_Case28252 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28252; }
        }

        public override void update()
        {

            //add some helptext to Report.SQL to assist in making reports
            CswNbtMetaDataObjectClass reportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            foreach( CswNbtMetaDataNodeType reportNT in reportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp sqlNTP = reportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReport.PropertyName.Sql );
                sqlNTP.HelpText = @"To create a parameterized statement you can use the syntax: {someparamname}.
The parameter name must be one word. If the parameter is meant to be a string, you must also put your own single quotes.
An example of a valid parameterized where clause is : where ColName = '{param}'.";
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28252

}//namespace ChemSW.Nbt.Schema