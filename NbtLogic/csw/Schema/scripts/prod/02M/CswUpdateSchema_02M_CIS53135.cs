using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53135 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53135; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ": Set value for Location FullPath property"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            // For each Location Node type that isn't Site, the Full Path should be Path + > + Name
            // and for Sites, the Full Path should be Name.

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp FullPathNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.FullPath );
                CswNbtMetaDataNodeTypeProp LocationNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                CswNbtMetaDataNodeTypeProp NameNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Name );
                if( null != FullPathNTP )
                {
                    // Set the value of the property
                    if( LocationNT.NodeTypeName != "Site" )
                    {
                        FullPathNTP.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Template].AsComposite.TemplateValue = "{" + LocationNTP.PropId + "} > " + "{" + NameNTP.PropId + "}";
                    }
                    else
                    {
                        FullPathNTP.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Template].AsComposite.TemplateValue = "{" + NameNTP.PropId + "}";
                    }

                    // Set the layout position and remove from the add layout
                    FullPathNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    FullPathNTP.updateLayout( CswEnumNbtLayoutType.Edit, LocationNTP, true );
                }
            }
        }
    }
}