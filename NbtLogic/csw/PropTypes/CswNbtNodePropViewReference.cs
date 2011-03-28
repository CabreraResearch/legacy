using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropViewReference : CswNbtNodeProp
    {
        //public static char delimiter = ',';

        public CswNbtNodePropViewReference( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewReference )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropViewReference() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _ViewIdSubField = ( (CswNbtFieldTypeRuleViewReference) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ViewIdSubField;
            _CachedViewNameSubField = ( (CswNbtFieldTypeRuleViewReference) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CachedViewNameSubField;

        }//generic

        private CswNbtSubField _ViewIdSubField;
        private CswNbtSubField _CachedViewNameSubField;

        override public bool Empty
        {
            get
            {
                return ( ViewId <= 0 );
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
        public Int32 ViewId
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( _CswNbtNodePropData.GetPropRowValue( _ViewIdSubField.Column ) );
                if( ret == Int32.MinValue ) //&& NodeId != null )
                {
                    // make a new view
                    CswNbtView NewView = new CswNbtView( _CswNbtResources );
                    NewView.makeNew( PropName, NbtViewVisibility.Property, null, null, null );
                    NewView.save();
                    _CswNbtNodePropData.SetPropRowValue( _ViewIdSubField.Column, NewView.ViewId );
                    _CswNbtNodePropData.SetPropRowValue( _CachedViewNameSubField.Column, PropName );

                    // Case 20194. KLUGE Alert!!!
                    CswNbtNode node = _CswNbtResources.Nodes.GetNode( _CswNbtNodePropData.NodeId );
                    if( null != node )
                        node.postChanges( false );
                }

                return CswConvert.ToInt32( _CswNbtNodePropData.GetPropRowValue( _ViewIdSubField.Column ) );
            }
            private set
            {
                if( _CswNbtNodePropData.SetPropRowValue( _ViewIdSubField.Column, value ) )
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
            if( Int32.MinValue != ViewId )
            {
                CswNbtView View = (CswNbtView) CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                if( View != null )
                    CachedViewName = View.ViewName;
            }
            this.PendingUpdate = false;
        }

        public override void ToXml( XmlNode ParentNode )
        {
			CswNbtView View = (CswNbtView) CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
			XmlNode ViewIdNode = CswXmlDocument.AppendXmlNode( ParentNode, _ViewIdSubField.ToXmlNodeName(), ViewId.ToString() );
			XmlNode ViewModeNode = CswXmlDocument.AppendXmlNode( ParentNode, "viewmode", View.ViewMode.ToString() );
			XmlNode CachedViewNameNode = CswXmlDocument.AppendXmlNode( ParentNode, _CachedViewNameSubField.ToXmlNodeName(), CachedViewName.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ViewId = CswConvert.ToInt32( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ViewIdSubField.ToXmlNodeName() ) );
            PendingUpdate = true;
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ViewId = CswConvert.ToInt32( PropRow[_ViewIdSubField.ToXmlNodeName()].ToString() );
            PendingUpdate = true;
        }



    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
