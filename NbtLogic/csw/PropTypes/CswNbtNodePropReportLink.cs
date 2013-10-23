using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropReportLink : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropReportLink( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsReportLink;
        }

        public CswNbtNodePropReportLink( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            CswNbtFieldTypeRuleReportLink FieldTypeRule = (CswNbtFieldTypeRuleReportLink) _FieldTypeRule;
        }

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST
            ParentObject["reportid"] = new CswPrimaryKey( "nodes", this.NodeTypeProp.FKValue ).ToString();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing        
        }

        public override void SyncGestalt()
        {

        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
