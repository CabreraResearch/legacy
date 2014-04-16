using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS53381 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53381; }
        }

        public override string Title
        {
            get { return "Evil Things You Can Do in Design Mode - by Steve Salter.  Chapter 1 - Adding Invisible Properties"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignNTPOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataObjectClassProp PropNameOCP = DesignNTPOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PropNameOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
            foreach( CswNbtMetaDataNodeType DesignNTPNT in DesignNTPOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PropNameNTP = DesignNTPNT.getNodeTypePropByObjectClassProp( PropNameOCP );
                PropNameNTP.DesignNode.Required.Checked = CswEnumTristate.True;
                PropNameNTP.DesignNode.postChanges( true );
            }
        }
    }
}