using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropMTBF : CswNbtNodeProp
    {
        public CswNbtNodePropMTBF( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _StartDateTimeSubField = ( (CswNbtFieldTypeRuleMTBF) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).StartDateTimeSubField;
            _UnitsSubField = ( (CswNbtFieldTypeRuleMTBF) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).UnitsSubField;
            _ValueSubField = ( (CswNbtFieldTypeRuleMTBF) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ValueSubField;
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

        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt


        public DateTime StartDateTime
        {
            get
            {
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _StartDateTimeSubField.Column );
                DateTime ReturnVal = DateTime.MinValue;
                if( StringVal != string.Empty )
                    ReturnVal = Convert.ToDateTime( StringVal );
                return ( ReturnVal );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _StartDateTimeSubField.Column, value );
            }
        }//StartDateTime

        public string Units
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _UnitsSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _UnitsSubField.Column, value );
            }
        }//Units

        public bool DefaultToToday
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.DateToday;
            }
        }

        public double CachedValue
        {
            get
            {
                double ret = Double.NaN;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
                if( CswTools.IsFloat( StringVal ) )
                    ret = Convert.ToDouble( StringVal );
                return ret;
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                if( value != Double.NaN )
                    _CswNbtNodePropData.Gestalt = value.ToString() + " " + Units.ToString();
                else
                    _CswNbtNodePropData.Gestalt = string.Empty;
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
                if( EndDate > DateTime.MinValue )
                    Diff = EndDate.Subtract( StartDate );
                else
                    Diff = DateTime.Now.Subtract( StartDate );

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
                CachedValue = hours;
            else
                CachedValue = days;

            this.PendingUpdate = false;

        } // CalculateMTBF()

        private Int32 _countProblems( DateTime StartDate, DateTime EndDate )
        {
            Int32 ret = 0;
            if( this.NodeId != null )
            {
                // BZ 6779
                CswNbtMetaDataObjectClass ProblemOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
                CswNbtMetaDataObjectClassProp OwnerOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.OwnerPropertyName );
                CswNbtMetaDataObjectClassProp FailureOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.FailurePropertyName );
                CswNbtMetaDataObjectClassProp DateOpenedOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.DateOpenedPropertyName );


                CswNbtView ProblemFailuresView = new CswNbtView( _CswNbtResources );
                ProblemFailuresView.ViewName = "Problem Failures";
                CswNbtViewRelationship ParentRelationship = ProblemFailuresView.AddViewRelationship( this.NodeTypeProp.NodeType, true );
                ParentRelationship.NodeIdsToFilterIn.Add( this.NodeId );
                CswNbtViewRelationship ChildRelationship = ProblemFailuresView.AddViewRelationship( ParentRelationship,
                                                                                                    CswNbtViewRelationship.PropOwnerType.Second,
                                                                                                    OwnerOCP, true );
                // BZ 10277 - Only Problems flagged Failure 
                CswNbtViewProperty FailureVP = ProblemFailuresView.AddViewProperty( ChildRelationship, FailureOCP );
                CswNbtViewPropertyFilter FailureFilter = ProblemFailuresView.AddViewPropertyFilter( FailureVP, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, Tristate.True.ToString(), false );

                // BZ 10259...within the calculation date scope
                CswNbtViewProperty DateOpenedVP = ProblemFailuresView.AddViewProperty( ChildRelationship, DateOpenedOCP );
                CswNbtViewPropertyFilter DateOpenedStartFilter = ProblemFailuresView.AddViewPropertyFilter( DateOpenedVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals, StartDate.ToString(), false );
                CswNbtViewPropertyFilter DateOpenedEndFilter = ProblemFailuresView.AddViewPropertyFilter( DateOpenedVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, EndDate.ToString(), false );

                ICswNbtTree ProblemNodesTree = _CswNbtResources.Trees.getTreeFromView( ProblemFailuresView, true, true, false, false );

                if( ProblemNodesTree.getChildNodeCount() > 0 )
                {
                    ProblemNodesTree.goToNthChild( 0 );
                    ret = ProblemNodesTree.getChildNodeCount();
                }
            }

            return ret;

        } // _countProblems()



        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode StartDateNode = CswXmlDocument.AppendXmlNode( ParentNode, _StartDateTimeSubField.ToXmlNodeName(), StartDateTime.ToString() );
            XmlNode ValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _ValueSubField.ToXmlNodeName(), CachedValue.ToString() );
            XmlNode UnitsNode = CswXmlDocument.AppendXmlNode( ParentNode, _UnitsSubField.ToXmlNodeName(), Units );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            StartDateTime = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _StartDateTimeSubField.ToXmlNodeName() );
            Units = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _UnitsSubField.ToXmlNodeName() );
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
            string StringStartDateTime = CswTools.XmlRealAttributeName( PropRow[_StartDateTimeSubField.ToXmlNodeName()].ToString() );
            if( StringStartDateTime != string.Empty )
                StartDateTime = Convert.ToDateTime( StringStartDateTime );
            Units = CswTools.XmlRealAttributeName( PropRow[_UnitsSubField.ToXmlNodeName()].ToString() );
            PendingUpdate = true;
        }




    }//CswNbtNodePropMTBF

}//namespace ChemSW.Nbt.PropTypes
