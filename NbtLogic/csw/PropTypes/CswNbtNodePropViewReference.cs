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

    public class CswNbtNodePropViewReference : CswNbtNodeProp
    {
        //public static char delimiter = ',';

        public static implicit operator CswNbtNodePropViewReference( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsViewReference;
        }

        public CswNbtNodePropViewReference( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewReference )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropViewReference() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}
            _FieldTypeRule = (CswNbtFieldTypeRuleViewReference) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _ViewIdSubField = _FieldTypeRule.ViewIdSubField;
            _CachedViewNameSubField = _FieldTypeRule.CachedViewNameSubField;

        }
        private CswNbtFieldTypeRuleViewReference _FieldTypeRule;
        private CswNbtSubField _ViewIdSubField;
        private CswNbtSubField _CachedViewNameSubField;

        override public bool Empty
        {
            get
            {
                return ( false == ViewId.isSet() );
            }
        }//Empty


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }//Gestalt

        /// <summary>
        /// ViewId for referenced view
        /// </summary>
        public CswNbtViewId ViewId
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( _CswNbtNodePropData.GetPropRowValue( _ViewIdSubField.Column ) );
                if( ret == Int32.MinValue ) //&& NodeId != null )
                {
                    // make a new view
                    CswNbtView NewView = new CswNbtView( _CswNbtResources );
                    NewView.saveNew( PropName, NbtViewVisibility.Property, null, null, null );
                    //NewView.save();
                    _CswNbtNodePropData.SetPropRowValue( _ViewIdSubField.Column, NewView.ViewId.get() );
                    _CswNbtNodePropData.SetPropRowValue( _CachedViewNameSubField.Column, PropName );

                    // Case 20194. KLUGE Alert!!!
                    CswNbtNode node = _CswNbtResources.Nodes.GetNode( _CswNbtNodePropData.NodeId );
                    if( null != node )
                        node.postChanges( false );
                }

                return new CswNbtViewId( CswConvert.ToInt32( _CswNbtNodePropData.GetPropRowValue( _ViewIdSubField.Column ) ) );
            }
            private set
            {
                if( _CswNbtNodePropData.SetPropRowValue( _ViewIdSubField.Column, value.get() ) )
                    PendingUpdate = true;
            }
        }

        /// <summary>
        /// String name of view
        /// </summary>
        public string CachedViewName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _CachedViewNameSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _CachedViewNameSubField.Column, value );
            }
        }

        /// <summary>
        /// Refreshes the cached view name
        /// </summary>
        public void RefreshViewName()
        {
            //bz # 8758
            CachedViewName = string.Empty;
            if( ViewId.isSet() )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                if( View != null )
                    CachedViewName = View.ViewName;
            }
            this.PendingUpdate = false;
        }

        public override void ToJSON( JObject ParentObject )
        {
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
            if( null != View )
            {
                ParentObject[_ViewIdSubField.ToXmlNodeName( true )] = ViewId.ToString();
                ParentObject["viewmode"] = View.ViewMode.ToString();
                ParentObject[_CachedViewNameSubField.ToXmlNodeName( true )] = CachedViewName.ToString();
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ViewId = new CswNbtViewId( CswConvert.ToInt32( PropRow[_ViewIdSubField.ToXmlNodeName()].ToString() ) );
            PendingUpdate = true;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //ViewId = new CswNbtViewId( CswConvert.ToInt32( JObject[_ViewIdSubField.ToXmlNodeName()] ) );
            //PendingUpdate = true;
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
