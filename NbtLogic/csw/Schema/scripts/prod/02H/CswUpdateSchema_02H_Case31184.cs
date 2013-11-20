using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31184: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31184; }
        }

        public override string AppendToScriptName()
        {
            return "02H_Case" + CaseNo;
        }

        public override string Title
        {
            get { return "Fix Location Codes report"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            foreach( CswNbtObjClassReport ReportNode in ReportOC.getNodes( false, true, false, true ) )
            {
                if( "Location Codes" == ReportNode.ReportName.Text )
                {
                    ReportNode.SQL.Text = @"select lscode,location,name,Location || ' > ' || name pathname, type from (
    select barcode lscode,name,location,'building' type from building
    union
    select barcode lscode,name,location,'room' type from room
    union
    select barcode lscode,name,location,'cabinet' type from cabinet
    union
    select barcode lscode, name, location, 'floor' type from floor
    union
    select barcode lscode, name, location, 'shelf' type from shelf
    union
    select barcode lscode, name, location, 'box' type from box
    ) x 
where name is not null and location is not null and lower(location || ' > ' || name) like lower(trim('{LocationBegins}') || '%')
order by lower(location), lower(name)";

                    ReportNode.postChanges( false );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema