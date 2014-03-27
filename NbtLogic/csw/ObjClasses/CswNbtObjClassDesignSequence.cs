using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignSequence: CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Pre = "Pre";
            public const string Post = "Post";
            public const string Pad = "Pad";
            public const string NextValue = "Next Value";
        }

        public CswNbtObjClassDesignSequence( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
        }

        //ctor()

        /// <summary>
        /// This is the object class that OWNS this property (DesignNodeType)
        /// If you want the object class property value, look for ObjectClassProperty
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeType
        /// </summary>
        public static implicit operator CswNbtObjClassDesignSequence( CswNbtNode Node )
        {
            CswNbtObjClassDesignSequence ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignSequenceClass ) )
            {
                ret = (CswNbtObjClassDesignSequence) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( Creating )
            {
                string DbName = getDbName();
                if( false == _CswNbtResources.doesUniqueSequenceExist( DbName ) )
                {
                    _CswNbtResources.makeUniqueSequenceForProperty( DbName, 1 );
                }
            }
        }

        protected override void afterPopulateProps()
        {
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                NextValue.SetOnBeforeRender( delegate( CswNbtNodeProp prop )
                    {
                        NextValue.Text = getCurrent();
                    } );
            }
        } //afterPopulateProps()

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                /*Do Something*/
            }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText Pre { get { return ( _CswNbtNode.Properties[PropertyName.Pre] ); } }
        public CswNbtNodePropText Post { get { return ( _CswNbtNode.Properties[PropertyName.Post] ); } }
        public CswNbtNodePropNumber Pad { get { return ( _CswNbtNode.Properties[PropertyName.Pad] ); } }
        public CswNbtNodePropText NextValue { get { return ( _CswNbtNode.Properties[PropertyName.NextValue] ); } }
        
        #endregion

        #region Sequence Functions

        public Int32 SequenceId
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( CswTools.IsPrimaryKey( RelationalId ) )
                {
                    ret = RelationalId.PrimaryKey;
                }
                return ret;
            }
        }

        public string getDbName()
        {
            return Name.Text.Replace( " ", string.Empty );
        }

        public string formatSequence( Int32 RawSequenceVal )
        {
            string ret = "";
            ret = RawSequenceVal.ToString();
            if( Pad.Value > 0 )
            {
                while( ret.Length < Pad.Value )
                {
                    ret = "0" + ret;
                }
            }
            ret = Pre.Text + ret + Post.Text;
            return ret;
        } // formatSequence()


        public Int32 deformatSequence( string FormattedSequenceVal )
        {
            Int32 ret = Int32.MinValue;
            string PrepVal = Pre.Text;
            string PostVal = Post.Text;
            if( FormattedSequenceVal.Length > ( PrepVal.Length + PostVal.Length ) )
            {
                if( FormattedSequenceVal.Substring( 0, PrepVal.Length ) == PrepVal &&
                    FormattedSequenceVal.Substring( FormattedSequenceVal.Length - PostVal.Length, PostVal.Length ) == PostVal )
                {
                    string RawSequenceVal = FormattedSequenceVal.Substring( PrepVal.Length, ( FormattedSequenceVal.Length - PrepVal.Length - PostVal.Length ) );
                    if( CswTools.IsInteger( RawSequenceVal ) )
                    {
                        ret = CswConvert.ToInt32( RawSequenceVal );
                    }
                }
            }
            return ret;
        } // deformatSequence()


        public string getNext()
        {
            string DbName = getDbName();
            if( false == _CswNbtResources.doesUniqueSequenceExist( DbName ) )
            {
                //_CswNbtResources.makeUniqueSequenceForProperty( DbName, 1 );
            }
            Int32 RawSequenceVal = _CswNbtResources.getNextUniqueSequenceVal( DbName );
            return formatSequence( RawSequenceVal );
        }

        public string getCurrent()
        {
            string ret = string.Empty;
            string DbName = getDbName();
            if( _CswNbtResources.doesUniqueSequenceExist( DbName ) )
            {
                // Do not create the sequence if it's missing here, or else you create race conditions.  See case 31584.
                Int32 RawSequenceVal = _CswNbtResources.getCurrentUniqueSequenceVal( DbName );
                ret = formatSequence( RawSequenceVal );
            }
            return ret;
        }

        /// <summary>
        /// Resets next sequence value based on maximum existing value in the database.
        /// </summary>
        public void reSync( CswEnumNbtPropColumn Column )
        {
            reSync( Column, Int32.MinValue );
        }

        /// <summary>
        /// Resets next sequence value based on newest entry and existing values in the database.
        /// </summary>
        public void reSync( CswEnumNbtPropColumn Column, Int32 NewSeqVal )
        {
            string SelectText = @"select j." + Column.ToString() + @"
                                    from sequences s
                                    join nodetype_props p on ( p.sequenceid = s.sequenceid ) 
                                    join jct_nodes_props j on ( p.nodetypepropid = j.nodetypepropid and j.nodeid is not null )
                                   where s.sequenceid = :sequenceid ";

            CswArbitrarySelect SeqValueSelect = _CswNbtResources.makeCswArbitrarySelect( "syncSequence_maxvalue_select", SelectText );
            SeqValueSelect.addParameter( "sequenceid", SequenceId.ToString() );
            DataTable SeqValueTable = SeqValueSelect.getTable();

            Int32 MaxSeqVal = NewSeqVal;
            foreach( DataRow SeqValueRow in SeqValueTable.Rows )
            {
                Int32 ThisSeqVal = CswConvert.ToInt32( SeqValueRow[Column.ToString()] );
                if( ThisSeqVal > MaxSeqVal )
                {
                    MaxSeqVal = ThisSeqVal;
                }
            } // foreach( DataRow SeqValueRow in SeqValueTable.Rows )
            //_CswNbtResources.resetUniqueSequenceVal( getDbName(), MaxSeqVal + 1 );
        } // reSync()

        public static CswNbtObjClassDesignSequence getSequence( CswNbtResources CswNbtResources, string SequenceName )
        {
            CswNbtMetaDataObjectClass SequenceOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
            return SequenceOC.getNodes( forceReInit : false, includeSystemNodes : true )
                             .FirstOrDefault( seq => ( (CswNbtObjClassDesignSequence) seq ).Name.Text.ToLower() == SequenceName.ToLower() );
        }

        #endregion Sequence Functions

    } // CswNbtObjClassDesignSequence

}//namespace ChemSW.Nbt.ObjClasses
