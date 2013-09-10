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
    public class CswNbtNodePropMTBF : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropMTBF( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMTBF;
        }

        public CswNbtNodePropMTBF( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _StartDateTimeSubField = ( (CswNbtFieldTypeRuleMTBF) _FieldTypeRule ).StartDateTimeSubField;
            _UnitsSubField = ( (CswNbtFieldTypeRuleMTBF) _FieldTypeRule ).UnitsSubField;
            _ValueSubField = ( (CswNbtFieldTypeRuleMTBF) _FieldTypeRule ).ValueSubField;

            if( string.IsNullOrEmpty( Units ) )
            {
                Units = "days";
            }

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _StartDateTimeSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => StartDateTime, x => StartDateTime = CswConvert.ToDateTime( x ) ) );
            _SubFieldMethods.Add( _UnitsSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Units, x => Units = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _ValueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CachedValue, x => CachedValue = CswConvert.ToDouble( x ) ) );
        }

        private CswNbtSubField _StartDateTimeSubField;
        private CswNbtSubField _UnitsSubField;
        private CswNbtSubField _ValueSubField;

        override public bool Empty
        {
            get
            {
                return ( StartDateTime == DateTime.MinValue );
            }//
        }

        public DateTime StartDateTime
        {
            get
            {
                string StringVal = GetPropRowValue( _StartDateTimeSubField.Column );
                DateTime ReturnVal = DateTime.MinValue;
                if( StringVal != string.Empty )
                    ReturnVal = Convert.ToDateTime( StringVal );
                return ( ReturnVal );
            }
            set
            {
                SetPropRowValue( _StartDateTimeSubField.Column, value );
            }
        }//StartDateTime

        public string Units
        {
            get
            {
                return GetPropRowValue( _UnitsSubField.Column );
            }
            set
            {
                SetPropRowValue( _UnitsSubField.Column, value );
            }
        }//Units

        public bool DefaultToToday
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.DateToday;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public double CachedValue
        {
            get
            {
                double ret = Double.NaN;
                string StringVal = GetPropRowValue( _ValueSubField.Column );
                if( CswTools.IsFloat( StringVal ) )
                    ret = Convert.ToDouble( StringVal );
                return ret;
            }
            set
            {
                SetPropRowValue( _ValueSubField.Column, value );
                if( value != Double.NaN )
                    Gestalt = value.ToString() + " " + Units.ToString();
                else
                    Gestalt = string.Empty;
            }
        }

        public void RefreshCachedValue()
        {
            double Hours = Double.NaN;
            double Days = Double.NaN;
            CalculateMTBF( StartDateTime, DateTime.MinValue, ref Hours, ref Days );
        }

        public void CalculateMTBF( DateTime StartDate, DateTime EndDate, ref double hours, ref double days )
        {
            hours = 0;
            days = 0;

            if( StartDate > DateTime.MinValue )
            {
                TimeSpan Diff;
                if( DateTime.MinValue == EndDate )
                {
                    EndDate = DateTime.Now;
                }
                Diff = EndDate.Subtract( StartDate );

                days = Diff.TotalDays;
                hours = Diff.TotalHours;

                Int32 ProblemCnt = _countProblems( StartDate, EndDate );
                if( ProblemCnt > 0 )
                {
                    days = days / ProblemCnt;
                    hours = hours / ProblemCnt;
                }

                days = Math.Round( days );
                hours = Math.Round( hours );
            }

            // Set cached value
            if( Units == "hours" )
            {
                CachedValue = hours;
            }
            else
            {
                CachedValue = days;
            }
            this.PendingUpdate = false;

        } // CalculateMTBF()

        private Int32 _countProblems( DateTime StartDate, DateTime EndDate )
        {
            Int32 ret = 0;
            if( this.NodeId != null )
            {
                // BZ 6779
                CswNbtMetaDataObjectClass ProblemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ProblemClass );
                CswNbtMetaDataObjectClassProp OwnerOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.Owner );
                CswNbtMetaDataObjectClassProp FailureOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.Failure );
                CswNbtMetaDataObjectClassProp DateOpenedOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.DateOpened );


                CswNbtView ProblemFailuresView = new CswNbtView( _CswNbtResources );
                ProblemFailuresView.ViewName = "Problem Failures";
                CswNbtViewRelationship ParentRelationship = ProblemFailuresView.AddViewRelationship( this.NodeTypeProp.getNodeType(), true );
                ParentRelationship.NodeIdsToFilterIn.Add( this.NodeId );
                CswNbtViewRelationship ChildRelationship = ProblemFailuresView.AddViewRelationship( ParentRelationship,
                                                                                                    CswEnumNbtViewPropOwnerType.Second,
                                                                                                    OwnerOCP, true );
                // BZ 10277 - Only Problems flagged Failure 
                CswNbtViewProperty FailureVP = ProblemFailuresView.AddViewProperty( ChildRelationship, FailureOCP );
                CswNbtViewPropertyFilter FailureFilter = ProblemFailuresView.AddViewPropertyFilter( FailureVP, CswEnumNbtSubFieldName.Checked, CswEnumNbtFilterMode.Equals, CswEnumTristate.True.ToString(), false );

                // BZ 10259...within the calculation date scope
                CswNbtViewProperty DateOpenedVP = ProblemFailuresView.AddViewProperty( ChildRelationship, DateOpenedOCP );
                CswNbtViewPropertyFilter DateOpenedStartFilter = ProblemFailuresView.AddViewPropertyFilter( DateOpenedVP, CswEnumNbtSubFieldName.Value, CswEnumNbtFilterMode.GreaterThanOrEquals, StartDate.ToString(), false );
                CswNbtViewPropertyFilter DateOpenedEndFilter = ProblemFailuresView.AddViewPropertyFilter( DateOpenedVP, CswEnumNbtSubFieldName.Value, CswEnumNbtFilterMode.LessThanOrEquals, EndDate.ToString(), false );

                ICswNbtTree ProblemNodesTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, ProblemFailuresView, true, false, false );

                if( ProblemNodesTree.getChildNodeCount() > 0 )
                {
                    ProblemNodesTree.goToNthChild( 0 );
                    ret = ProblemNodesTree.getChildNodeCount();
                }
            }

            return ret;

        } // _countProblems()

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject[_StartDateTimeSubField.ToXmlNodeName( true )] = ( StartDateTime != DateTime.MinValue ) ? StartDateTime.ToShortDateString() : string.Empty;
            CswDateTime CswDate = new CswDateTime( _CswNbtResources, StartDateTime );
            ParentObject.Add( new JProperty( _StartDateTimeSubField.ToXmlNodeName( true ), CswDate.ToClientAsDateTimeJObject() ) );

            ParentObject[_ValueSubField.ToXmlNodeName( true )] = CachedValue.ToString();
            ParentObject[_UnitsSubField.ToXmlNodeName( true )] = Units;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string StringStartDateTime = CswTools.XmlRealAttributeName( PropRow[_StartDateTimeSubField.ToXmlNodeName()].ToString() );
            if( StringStartDateTime != string.Empty )
                StartDateTime = Convert.ToDateTime( StringStartDateTime );
            Units = CswTools.XmlRealAttributeName( PropRow[_UnitsSubField.ToXmlNodeName()].ToString() );
            PendingUpdate = true;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_StartDateTimeSubField.ToXmlNodeName( true )] )
            {
                //StartDateTime = CswConvert.ToDateTime( JObject.Property( _StartDateTimeSubField.ToXmlNodeName( true ) ).Value );
                CswDateTime CswDate = new CswDateTime( _CswNbtResources );
                CswDate.FromClientDateTimeJObject( (JObject) JObject[_StartDateTimeSubField.ToXmlNodeName( true )] );
                StartDateTime = CswDate.ToDateTime();
            }
            if( null != JObject[_UnitsSubField.ToXmlNodeName( true )] )
            {
                Units = JObject[_UnitsSubField.ToXmlNodeName( true )].ToString();
            }
            //PendingUpdate = true;
            RefreshCachedValue();
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, CachedValue.ToString() + " " + Units.ToString() );
        }

    }//CswNbtNodePropMTBF

}//namespace ChemSW.Nbt.PropTypes
