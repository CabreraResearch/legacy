using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30300 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.DH; }
        }

        public override int CaseNo
        {
            get { return 30300; }
        }

        public override string ScriptName
        {
            get { return "02E_Case30300"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        public override void update()
        {
            //case 30300: nbtimporter needs:
            //unit nodetypes of each, weight and volume can't have punctuation in their nodetypename
            foreach( CswNbtMetaDataNodeType nt in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( CswEnumNbtObjectClass.UnitOfMeasureClass ) )
            {
                if( nt.NodeTypeName == "Unit (Each)" )
                {
                    nt.NodeTypeName = "Unit_Each";
                }
                if( nt.NodeTypeName == "Unit (Weight)" )
                {
                    nt.NodeTypeName = "Unit_Weight";
                }
                if( nt.NodeTypeName == "Unit (Volume)" )
                {
                    nt.NodeTypeName = "Unit_Volume";
                }
            }

            //all nodetypes of LocationClass OC need to have both their Name and their (parent) Location properties set IsCompoundUnique=true
            foreach( CswNbtMetaDataNodeType locationNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( CswEnumNbtObjectClass.LocationClass ) )
            {
                CswNbtMetaDataNodeTypeProp nameNTP = locationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Name );
                nameNTP.setIsCompoundUnique( true );
                CswNbtMetaDataNodeTypeProp locNTP = locationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                locNTP.setIsCompoundUnique( true );
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema