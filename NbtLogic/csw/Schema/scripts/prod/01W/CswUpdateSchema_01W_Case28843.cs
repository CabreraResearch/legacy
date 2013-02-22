using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28843
    /// </summary>
    public class CswUpdateSchema_01W_Case28843 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28843; }
        }

        public override void update()
        {

            //change the Request NT name to '{Name} {Date Submitted}' and change the submitted date to unclude the time
            CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            foreach( CswNbtMetaDataNodeType requestNT in requestOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp nameNTP = requestNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Name );
                CswNbtMetaDataNodeTypeProp dateSubmittedNTP = requestNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );

                dateSubmittedNTP.Extended = CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString();

                string newNameTemplate = CswNbtMetaData.MakeTemplateEntry( nameNTP.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( dateSubmittedNTP.PropName );
                requestNT.setNameTemplateText( newNameTemplate );
            }


        } //Update()

    }//class CswUpdateSchema_01V_Case28843

}//namespace ChemSW.Nbt.Schema