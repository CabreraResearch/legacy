using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{


    public class CswNbtNodePropWrapper//: IComparable
    {

        private CswNbtNodeProp _CswNbtNodeProp = null;
        private CswNbtNodePropData _CswNbtNodePropData = null;

        public CswNbtNodePropWrapper( CswNbtNodeProp CswNbtNodeProp, CswNbtNodePropData CswNbtNodePropData )
        {
            _CswNbtNodeProp = CswNbtNodeProp;
            _CswNbtNodePropData = CswNbtNodePropData;
        }//ctor

        //bz # 8287: rearranged a few things
        public void refresh( DataRow NewValueRow )
        {
            _CswNbtNodePropData.refresh( NewValueRow );

        }//refresh

        public void clearModifiedFlag()
        {
            _CswNbtNodeProp.clearModifiedFlag();
        }//clearModifiedFlag()


        //public int CompareTo( object obj )
        //{
        //    if( !( obj is CswNbtNodePropWrapper ) )
        //        throw new ArgumentException( "object is not a CswNbtNodePropWrapper" );


        //    CswNbtNodePropWrapper CswNbtNodePropWrapper = ( CswNbtNodePropWrapper ) obj;

        //    //return ( string.Compare( FieldType.FieldType.ToString(), CswNbtNodePropWrapper.FieldType.FieldType.ToString() ) );

        //    return ( string.Compare( FieldType.FieldType.ToString(), CswNbtNodePropWrapper.FieldType.FieldType.ToString() ) );

        //}//CompareTo() 

        public bool WasModified
        {
            get
            {
                return ( _CswNbtNodeProp.WasModified );
            }//

        }//WasModified

        public bool SuspendModifyTracking
        {
            set
            {
                _CswNbtNodeProp.SuspendModifyTracking = value;
            }
            get
            {
                return ( _CswNbtNodeProp.SuspendModifyTracking );
            }//
        }//SuspendModifyTracking

        //public void ensureEmptyVal()
        //{
        //    _CswNbtNodePropData.ensureEmptyVal();
        //}

        public string Gestalt { get { return ( _CswNbtNodeProp.Gestalt ); } }
        public string ClobData { get { return ( _CswNbtNodePropData.ClobData ); } }

        public bool Empty { get { return ( _CswNbtNodeProp.Empty ); } }
        //        public abstract CswNbtNodePropData Attributes { get; }


        public CswNbtMetaDataFieldType FieldType { get { return ( _CswNbtNodeProp.FieldType ); } }
        public CswNbtMetaDataNodeTypeProp NodeTypeProp { get { return ( _CswNbtNodeProp.NodeTypeProp ); } }

        public string GetPropRowValue( CswNbtSubField.PropColumn Column ) { return _CswNbtNodePropData.GetPropRowValue( Column ); }
        public string GetOriginalPropRowValue( CswNbtSubField.PropColumn Column ) { return _CswNbtNodePropData.GetOriginalPropRowValue( Column ); }
        public void SetPropRowValue( CswNbtSubField.PropColumn Column, object value ) { _CswNbtNodePropData.SetPropRowValue( Column, value ); }
        public void makePropRow() { _CswNbtNodePropData.makePropRow(); }

        public string PropName { get { return ( _CswNbtNodeProp.PropName ); } }
        public Int32 JctNodePropId { get { return ( _CswNbtNodeProp.JctNodePropId ); } }
        public Int32 NodeTypePropId { get { return ( _CswNbtNodeProp.NodeTypePropId ); } }
        public Int32 ObjectClassPropId { get { return ( _CswNbtNodeProp.ObjectClassPropId ); } }
        public string ObjectClassPropName { get { return ( _CswNbtNodeProp.ObjectClassPropName ); } }
        public CswPrimaryKey NodeId { get { return ( _CswNbtNodePropData.NodeId ); } set { _CswNbtNodePropData.NodeId = value; } }
        public bool ReadOnly { get { return ( _CswNbtNodePropData.ReadOnly ); } set { _CswNbtNodePropData.ReadOnly = value; } }
        public bool Hidden { get { return ( _CswNbtNodePropData.Hidden ); } set { _CswNbtNodePropData.Hidden = value; } }
        public string Field1 { get { return ( _CswNbtNodePropData.Field1 ); } set { _CswNbtNodePropData.Field1 = value; } }
        public string Field2 { get { return ( _CswNbtNodePropData.Field2 ); } set { _CswNbtNodePropData.Field2 = value; } }
        public string Field3 { get { return ( _CswNbtNodePropData.Field3 ); } set { _CswNbtNodePropData.Field3 = value; } }
        public string Field4 { get { return ( _CswNbtNodePropData.Field4 ); } set { _CswNbtNodePropData.Field4 = value; } }
        public string Field5 { get { return ( _CswNbtNodePropData.Field5 ); } set { _CswNbtNodePropData.Field5 = value; } }
        //public bool IsPropRow( DataRow DataRow ) { return ( _CswNbtNodeProp.IsPropRow( DataRow ) ); }
        public bool Required { get { return ( _CswNbtNodeProp.Required ); } }
        /// <summary>
        /// Determines whether to treat the property as required, temporarily
        /// </summary>
        public bool TemporarilyRequired { get { return _CswNbtNodePropData.TemporarilyRequired; } set { _CswNbtNodePropData.TemporarilyRequired = value; } }
        public CswNbtNodePropWrapper DefaultValue { get { return ( _CswNbtNodeProp.DefaultValue ); } }
        public bool HasDefaultValue() { return ( _CswNbtNodeProp.HasDefaultValue() ); }

        public bool PendingUpdate { get { return ( _CswNbtNodePropData.PendingUpdate ); } set { _CswNbtNodePropData.PendingUpdate = value; } }

        public void ClearValue() { _CswNbtNodePropData.ClearValue(); }
        public void ClearBlob() { _CswNbtNodePropData.ClearBlob(); }

		public void onBeforeUpdateNodePropRow( bool IsCopy, bool OverrideUniqueValidation ) { _CswNbtNodeProp.onBeforeUpdateNodePropRow( IsCopy, OverrideUniqueValidation ); }
        public void onNodePropRowFilled() { _CswNbtNodeProp.onNodePropRowFilled(); }

        //public bool IsNodeReference( XmlNode PropertyValueNode ) { return _CswNbtNodeProp.IsNodeReference( PropertyValueNode ); }
        //public bool IsNodeTypeReference( XmlNode PropertyValueNode ) { return _CswNbtNodeProp.IsNodeTypeReference( PropertyValueNode ); }

        public bool AuditChanged { get { return _CswNbtNodePropData.AuditChanged; } }

        // case 21809
        private string _HelpText = string.Empty;
        public string HelpText
        {
            get
            {
                string ret = NodeTypeProp.HelpText;
                if( _HelpText != string.Empty && NodeTypeProp.HelpText != string.Empty )
                {
                    ret += " ";
                }
                if( _HelpText != string.Empty )
                {
                    ret += _HelpText;
                }
                return ret;
            }
            set { _HelpText = value; }
        }




        /// <summary>
        /// Returns defined Field Type attributes/subfields as XmlDocument class XmlNode
        /// </summary>
        /// <param name="Parent">XmlDocument class XmlNode</param>
        public void ToXml( XmlNode Parent )
        {
            _CswNbtNodeProp.ToXml( Parent );
        }
        /// <summary>
        /// Parses defined Field Type attributes/subfields into a XmlDocument class object
        /// </summary>
        /// <param name="Node">XmlDocument class XmlNode</param>
        public void ReadXml( XmlNode Node, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _CswNbtNodeProp.ReadXml( Node, NodeMap, NodeTypeMap );
        }

        /// <summary>
        /// Returns defined Field Type attributes/subfields as JToken class JObject
        /// </summary>
        /// <param name="JObject">JToken class JObject</param>
        public void ToJSON( JObject JObject )
        {
            JObject Values = new JObject();
            JObject["values"] = Values;
            _CswNbtNodeProp.ToJSON( Values );
        }

        /// <summary>
        /// Parses defined Field Type attributes/subfields into a JToken class JObject
        /// </summary>
        public void ReadJSON( JObject Object, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != Object.Property( "values" ) )
            {
                JObject Values = (JObject) Object["values"];
                _CswNbtNodeProp.ReadJSON( Values, NodeMap, NodeTypeMap );
            }
        }

        /// <summary>
        /// Returns defined Field Type attributes/subfields as XContainer class XElement
        /// </summary>
        /// <param name="Parent">XContainer class XElement</param>
        public void ToXElement( XElement Parent )
        {
            _CswNbtNodeProp.ToXElement( Parent );
        }
        /// <summary>
        /// Parses defined Field Type attributes/subfields into a XContainer class object
        /// </summary>
        /// <param name="Node">XContainer class XElement</param>
        public void ReadXElement( XElement Node, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _CswNbtNodeProp.ReadXElement( Node, NodeMap, NodeTypeMap );
        }
        public void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _CswNbtNodeProp.ReadDataRow( PropRow, NodeMap, NodeTypeMap );
        }

        /// <summary>
        /// Copy property value
        /// This triggers field-type-specific copying
        /// </summary>
        public void copy( CswNbtNodePropWrapper Source )
        {
            _CswNbtNodeProp.Copy( Source._CswNbtNodePropData );
        }

        /// <summary>
        /// Copy property value
        /// This doesn't triggers field-type-specific copying!
        /// For copying Object Class Prop Default Values Only!  See BZ 5073
        /// </summary>
        public void copy( CswNbtNodePropData Source )
        {
            _CswNbtNodePropData.copy( Source );
        }

        private string _makeTypeErrorMessage( Type CurrentType )
        {
            return ( "Current class is not of type " + CurrentType.ToString() );
        }

        /// <summary>
        /// Set the default value specified by the nodetype prop
        /// </summary>
        public void SetDefaultValue()
        {
            bool DoCopy = false;
            switch( this.FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    CswNbtNodePropDateTime PropAsDate = this.AsDateTime;
                    if( PropAsDate.DefaultToToday )
                    {
                        PropAsDate.DateTimeValue = DateTime.Now;
                    }
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    CswNbtNodePropMTBF PropAsMTBF = this.AsMTBF;
                    if( PropAsMTBF.DefaultToToday )
                    {
                        PropAsMTBF.StartDateTime = DateTime.Now;
                    }
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    // This will default to Top.  Setting the Parent might change this later.
                    this.AsLocation.SelectedNodeId = null;
                    DoCopy = true;
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    if( this.DefaultValue.AsBarcode.Barcode != CswNbtNodePropBarcode.AutoSignal )
                    {
                        DoCopy = true;
                    }
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                    if( this.DefaultValue.AsSequence.Sequence != CswNbtNodePropBarcode.AutoSignal )
                    {
                        DoCopy = true;
                    }
                    break;
                default:
                    DoCopy = true;
                    break;
            } // switch( Prop.FieldType.FieldType )

            if( DoCopy && this.HasDefaultValue() )
            {
                this.copy( this.DefaultValue );
            }

        } // SetDefaultValue()



        public CswNbtNodePropBarcode AsBarcode
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropBarcode ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropBarcode ) ) ) );
                return ( (CswNbtNodePropBarcode) _CswNbtNodeProp );
            }
        }//Barcode

        public CswNbtNodePropButton AsButton
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropButton ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropButton ) ) ) );
                return ( (CswNbtNodePropButton) _CswNbtNodeProp );
            }
        }//Button

        public CswNbtNodePropBlob AsBlob
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropBlob ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropBlob ) ) ) );
                return ( (CswNbtNodePropBlob) _CswNbtNodeProp );
            }
        }//Blob

        public CswNbtNodePropComposite AsComposite
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropComposite ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropComposite ) ) ) );
                return ( (CswNbtNodePropComposite) _CswNbtNodeProp );
            }
        }//Composite

        public CswNbtNodePropDateTime AsDateTime
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropDateTime ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropDateTime ) ) ) );
                return ( (CswNbtNodePropDateTime) _CswNbtNodeProp );
            }
        }//DateTime

        public CswNbtNodePropGrid AsGrid
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropGrid ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropGrid ) ) ) );
                return ( (CswNbtNodePropGrid) _CswNbtNodeProp );
            }
        }//Grid

        public CswNbtNodePropImage AsImage
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropImage ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropImage ) ) ) );
                return ( (CswNbtNodePropImage) _CswNbtNodeProp );
            }
        }//Image

        public CswNbtNodePropImageList AsImageList
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropImageList ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropImageList ) ) ) );
                return ( (CswNbtNodePropImageList) _CswNbtNodeProp );
            }
        }//ImageList

        public CswNbtNodePropLink AsLink
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropLink ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropLink ) ) ) );
                return ( (CswNbtNodePropLink) _CswNbtNodeProp );
            }
        }//Link

        public CswNbtNodePropList AsList
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropList ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropList ) ) ) );
                return ( (CswNbtNodePropList) _CswNbtNodeProp );
            }
        }//List

        public CswNbtNodePropLocation AsLocation
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropLocation ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropLocation ) ) ) );
                return ( (CswNbtNodePropLocation) _CswNbtNodeProp );
            }
        }//Location

        public CswNbtNodePropLocationContents AsLocationContents
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropLocationContents ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropLocationContents ) ) ) );
                return ( (CswNbtNodePropLocationContents) _CswNbtNodeProp );
            }
        }//LocationContents

        public CswNbtNodePropLogical AsLogical
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropLogical ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropLogical ) ) ) );
                return ( (CswNbtNodePropLogical) _CswNbtNodeProp );
            }
        }//Logical

        public CswNbtNodePropLogicalSet AsLogicalSet
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropLogicalSet ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropLogicalSet ) ) ) );
                return ( (CswNbtNodePropLogicalSet) _CswNbtNodeProp );
            }
        }//LogicalSet

        public CswNbtNodePropMemo AsMemo
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropMemo ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropMemo ) ) ) );
                return ( (CswNbtNodePropMemo) _CswNbtNodeProp );
            }

        }//AsMemo

        public CswNbtNodePropMol AsMol
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropMol ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropMol ) ) ) );
                return ( (CswNbtNodePropMol) _CswNbtNodeProp );
            }

        }//AsMol

        public CswNbtNodePropMTBF AsMTBF
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropMTBF ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropMTBF ) ) ) );
                return ( (CswNbtNodePropMTBF) _CswNbtNodeProp );
            }

        }//AsMTBF

        //public CswNbtNodePropMultiRelationship AsMultiRelationship
        //{
        //    get
        //    {
        //        if( !( _CswNbtNodeProp is CswNbtNodePropMultiRelationship ) )
        //            throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropMultiRelationship ) ) ) );
        //        return ( (CswNbtNodePropMultiRelationship) _CswNbtNodeProp );
        //    }

        //}//AsMultiRelationship

        public CswNbtNodePropMultiList AsMultiList
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropMultiList ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropMultiList ) ) ) );
                return ( (CswNbtNodePropMultiList) _CswNbtNodeProp );
            }

        }//AsMultiList

        public CswNbtNodePropNodeTypeSelect AsNodeTypeSelect
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropNodeTypeSelect ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropNodeTypeSelect ) ) ) );
                return ( (CswNbtNodePropNodeTypeSelect) _CswNbtNodeProp );
            }
        }//AsNodeTypeSelect

        public CswNbtNodePropNFPA AsNFPA
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropNFPA ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropNFPA ) ) ) );
                return ( (CswNbtNodePropNFPA) _CswNbtNodeProp );
            }
        }//AsNFPA

        public CswNbtNodePropNumber AsNumber
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropNumber ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropNumber ) ) ) );
                return ( (CswNbtNodePropNumber) _CswNbtNodeProp );
            }
        }//Number

        public CswNbtNodePropPassword AsPassword
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropPassword ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropPassword ) ) ) );
                return ( (CswNbtNodePropPassword) _CswNbtNodeProp );
            }
        }//Password

        public CswNbtNodePropPropertyReference AsPropertyReference
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropPropertyReference ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropPropertyReference ) ) ) );
                return ( (CswNbtNodePropPropertyReference) _CswNbtNodeProp );
            }
        }//PropertyReference

        public CswNbtNodePropQuantity AsQuantity
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropQuantity ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropQuantity ) ) ) );
                return ( (CswNbtNodePropQuantity) _CswNbtNodeProp );
            }
        }//Quantity

        public CswNbtNodePropQuestion AsQuestion
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropQuestion ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropQuestion ) ) ) );
                return ( (CswNbtNodePropQuestion) _CswNbtNodeProp );
            }
        }//Question

        public CswNbtNodePropRelationship AsRelationship
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropRelationship ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropRelationship ) ) ) );
                return ( (CswNbtNodePropRelationship) _CswNbtNodeProp );
            }
        }//Relationship


        public CswNbtNodePropScientific AsScientific
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropScientific ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropScientific ) ) ) );
                return ( (CswNbtNodePropScientific) _CswNbtNodeProp );
            }
        }//Scientific

        public CswNbtNodePropSequence AsSequence
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropSequence ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropSequence ) ) ) );
                return ( (CswNbtNodePropSequence) _CswNbtNodeProp );
            }
        }//Sequence

        public CswNbtNodePropStatic AsStatic
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropStatic ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropStatic ) ) ) );
                return ( (CswNbtNodePropStatic) _CswNbtNodeProp );
            }
        }//Static

        public CswNbtNodePropText AsText
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropText ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropText ) ) ) );
                return ( (CswNbtNodePropText) _CswNbtNodeProp );
            }
        }//Text

        //public CswNbtNodePropDateTime AsTime
        //{
        //    get
        //    {
        //        if( !( _CswNbtNodeProp is CswNbtNodePropTime ) )
        //            throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropTime ) ) ) );
        //        return ( (CswNbtNodePropTime) _CswNbtNodeProp );
        //    }
        //}//Time

        public CswNbtNodePropTimeInterval AsTimeInterval
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropTimeInterval ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropTimeInterval ) ) ) );
                return ( (CswNbtNodePropTimeInterval) _CswNbtNodeProp );
            }
        }//TimeInterval

        public CswNbtNodePropUserSelect AsUserSelect
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropUserSelect ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropUserSelect ) ) ) );
                return ( (CswNbtNodePropUserSelect) _CswNbtNodeProp );
            }
        }//UserSelect

        public CswNbtNodePropViewPickList AsViewPickList
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropViewPickList ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropViewPickList ) ) ) );
                return ( (CswNbtNodePropViewPickList) _CswNbtNodeProp );
            }
        }//ViewPickList

        public CswNbtNodePropViewReference AsViewReference
        {
            get
            {
                if( !( _CswNbtNodeProp is CswNbtNodePropViewReference ) )
                    throw ( new CswDniException( _makeTypeErrorMessage( typeof( CswNbtNodePropViewReference ) ) ) );
                return ( (CswNbtNodePropViewReference) _CswNbtNodeProp );
            }
        }//View



    }//CswNbtNodePropWrapper

}//namespace ChemSW.Nbt.PropTypes
