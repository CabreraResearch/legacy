using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31041 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31041; }
        }

        public override string Title
        {
            get { return "Make Fire Class Exempt Set Required"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtMetaDataObjectClass FireExemptSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
            if( null != ControlZoneOC & null != FireExemptSetOC )
            {
                CswNbtMetaDataObjectClassProp FireClassSetOCP = ControlZoneOC.getObjectClassProp( CswNbtObjClassControlZone.PropertyName.FireClassSetName );

                //make fire exempt set required
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireClassSetOCP, CswEnumNbtObjectClassPropAttributes.isrequired, CswConvert.ToDbVal( true ) );
                
                //find the default fire exempt set
                CswNbtObjClassFireClassExemptAmountSet DefaultExemptSet = null;
                foreach( CswNbtObjClassFireClassExemptAmountSet FireExemptSet in FireExemptSetOC.getNodes( true, false, false, true ) )
                {
                    if( FireExemptSet.NodeName == "Default" )
                    {
                        DefaultExemptSet = FireExemptSet;
                        break;
                    }
                }
                if( null == DefaultExemptSet )
                {
                    DefaultExemptSet = FireExemptSetOC.getNodes( true, false, false, true ).FirstOrDefault();
                }

                //set "default" as the default exempt set on control zones
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue(
                    FireClassSetOCP, 
                    DefaultExemptSet.NodeId.PrimaryKey, 
                    CswEnumNbtSubFieldName.NodeID 
                );

                //set the default for all control zones that currently have nothing set
                foreach( CswNbtObjClassControlZone ControlZone in ControlZoneOC.getNodes( true, false, false, true ) )
                {
                    ControlZone.FireClassSetName.RelatedNodeId = ControlZone.FireClassSetName.RelatedNodeId ?? DefaultExemptSet.NodeId;
                    ControlZone.postChanges( false );
                }

            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema