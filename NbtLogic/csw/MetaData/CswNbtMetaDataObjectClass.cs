using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataObjectClass :  ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataObjectClass>
    {
        public enum NbtObjectClass
        {
            AliquotClass,
            BiologicalClass,
            ContainerClass,
            CustomerClass,
            EquipmentAssemblyClass,
            EquipmentClass,
            EquipmentTypeClass,
            FireExtinguisherClass,
            GeneratorClass,
            GenericClass,
            InventoryGroupClass,
            InspectionDesignClass,
            InspectionRouteClass,
            LocationClass,
            LocationGroupClass,
            MailReportClass,
            MaterialClass,
            MaterialSubclassClass,
            MaterialSynonymClass,
            MountPointClass,
            NotificationClass,
            PackageClass,
            PackDetailClass,
            ParameterClass,
            PrintLabelClass,
            ProblemClass,
            ReportClass,
            ResultClass,
            RoleClass,
            SampleClass,
            TaskClass,
            TestClass,
            //TestGroupClass,
            UnitOfMeasureClass,
            UserClass,
            Unknown,
            VendorClass
        };

        public static NbtObjectClass getObjectClassFromString( string ObjectClassName )
        {
            //return ( ( CswNbtMetaDataObjectClass.NbtObjectClass )Enum.Parse( typeof( CswNbtMetaDataObjectClass.NbtObjectClass ), ObjectClassName, true ) );
            NbtObjectClass ReturnVal = NbtObjectClass.Unknown;
            //bz # 7815 -- Should not care if the requested object class doesn't exist anymore
            if ( Enum.IsDefined( typeof( NbtObjectClass ), ObjectClassName ) )
            {
                ReturnVal = ( CswNbtMetaDataObjectClass.NbtObjectClass )Enum.Parse( typeof( CswNbtMetaDataObjectClass.NbtObjectClass ), ObjectClassName, true );
            }
            return ( ReturnVal );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _ObjectClassRow;

        public CswNbtMetaDataObjectClass( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            Reassign( Row );
        }

        public DataRow _DataRow
        {
            get { return _ObjectClassRow; }
            //set { _ObjectClassRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "objectclassid"; } }

        public void Reassign( DataRow NewRow )
        {
            _ObjectClassRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        public Int32 ObjectClassId
        {
            get { return Convert.ToInt32( _ObjectClassRow[ "objectclassid" ].ToString() ); }
        }
        //public string TableName
        //{
        //    get { return _ObjectClassRow["tablename"].ToString(); }
        //}
        public NbtObjectClass ObjectClass
        {
            get { return getObjectClassFromString( _ObjectClassRow[ "objectclass" ].ToString() ); }
        }
        public string IconFileName
        {
            get { return _ObjectClassRow[ "iconfilename" ].ToString(); }
        }

        public ICollection NodeTypes
        {
            get { return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypes( ObjectClassId ); }
        }

        //private Hashtable _ObjectClassPropsByPropId;
        //private SortedList _ObjectClassPropsByPropName;
        
        //public delegate void AddPropEventHandler( CswNbtMetaDataObjectClassProp NewProp );
        //public event AddPropEventHandler OnAddProp = null;

        //public void AddProp( CswNbtMetaDataObjectClassProp Prop )
        //{
        //    _ObjectClassPropsByPropId.Add( Prop.PropId, Prop );
        //    _ObjectClassPropsByPropName.Add( Prop.PropName, Prop );
        //    //if ( OnAddProp != null )
        //    //    OnAddProp( Prop );
        //}


        public ICollection ObjectClassPropIds
        {
            get
            {
                return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropIdsByObjectClass( ObjectClassId );
            }
        }
        public ICollection ObjectClassProps
        {
            get
            {
                return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByObjectClass( ObjectClassId );
            }
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassId, ObjectClassPropName );
            //if ( _ObjectClassPropsByPropName.Contains( ObjectClassPropName ) )
            //    return _ObjectClassPropsByPropName[ ObjectClassPropName ] as CswNbtMetaDataObjectClassProp;
            //else
            //    return null;
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassId, ObjectClassPropId );
            //if ( _ObjectClassPropsByPropId.Contains( ObjectClassPropId ) )
            //    return _ObjectClassPropsByPropId[ ObjectClassPropId ] as CswNbtMetaDataObjectClassProp;
            //else
            //    return null;
        }

        public CswNbtView CreateDefaultView()
        {
            CswNbtView DefaultView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
            DefaultView.ViewName = this.ObjectClass.ToString();

            CswNbtViewRelationship RelationshipToMe = DefaultView.AddViewRelationship( this, true );
            //RelationshipToMe.ArbitraryId = RelationshipToMe.SecondId.ToString();
            //DefaultView.Root.addChildRelationship( RelationshipToMe );

            return DefaultView;
        }


        #region IEquatable

        public static bool operator ==( CswNbtMetaDataObjectClass oc1, CswNbtMetaDataObjectClass oc2 )
        {
            // If both are null, or both are same instance, return true.
            if ( System.Object.ReferenceEquals( oc1, oc2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ( ( ( object )oc1 == null ) || ( ( object )oc2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if ( oc1.UniqueId == oc2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataObjectClass oc1, CswNbtMetaDataObjectClass oc2 )
        {
            return !( oc1 == oc2 );
        }

        public override bool Equals( object obj )
        {
            if ( !( obj is CswNbtMetaDataObjectClass ) )
                return false;
            return this == ( CswNbtMetaDataObjectClass )obj;
        }

        public bool Equals( CswNbtMetaDataObjectClass obj )
        {
            return this == ( CswNbtMetaDataObjectClass )obj;
        }

        public override int GetHashCode()
        {
            return this.ObjectClassId;
        }

        #endregion IEquatable
    }
}
