using System;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.TreeEvents
{

    public class CswNbtTreeFactory : ICswNbtTreeFactory
	{

        private string _SchemaPath = "";
        public CswNbtTreeFactory(  string SchemaPath )
        {
            _CswNbtResources = CswNbtResources;
            _SchemaPath = SchemaPath;
        }//ctor


        //public CswNbtNodes _CswNbtNodes = null;
        //public CswNbtNodes Nodes
        //{
        //    set
        //    {
        //        _CswNbtNodes = value;
        //    }

        //    get
        //    {
        //        if( null == _CswNbtNodes )
        //            throw( new CswDniException( "Internal error" , "CswNbtNodes property is null" ) );

        //        return( _CswNbtNodes );
        //    }
        //}//

        private CswNbtNodeCollection _CswNbtNodeCollection = null;
        public CswNbtNodeCollection CswNbtNodeCollection
        {
            get
            {
                if (_CswNbtNodeCollection == null)
					throw new CswDniException( "CswNbtTreeFactory.CswNbtNodeCollection is null" );
                return _CswNbtNodeCollection;
            }
            set { _CswNbtNodeCollection = value; }
        }

//        CswNbtNodeWriterFactory _CswNbtNodeWriterFactory = null;
        CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {
            set
            {
                _CswNbtResources = value;
                //_CswNbtNodeWriterFactory = new CswNbtNodeWriterFactory();
            }//set

            get
            {
                return( _CswNbtResources );
            }
        }//

        public ICswNbtTree makeTree(TreeMode TreeMode, CswNbtView View, bool IsFullyPopulated) //, CswNbtTreeKey CswNbtTreeKey)
        {
            ICswNbtTree ReturnVal = null;

            //CswNbtNodeWriter CswNbtNodeWriter = _CswNbtNodeWriterFactory.makeNodeWriter( _CswNbtResources );
            CswNbtNodeWriter CswNbtNodeWriter = new CswNbtNodeWriter(_CswNbtResources);
            CswNbtNodeReader CswNbtNodeReader = new CswNbtNodeReader(_CswNbtResources);


            //if( TreeMode.NLevelDs == TreeMode )
            //{
            //    ReturnVal = new CswNbtTreeNLevelDs( _CswNbtResources, _SchemaPath, CswNbtNodeWriter );
            //    CswNbtTreeNLevelDs CswNbtTreeNLevelDs = new CswNbtTreeNLevelDs(  _CswNbtResources, _SchemaPath, CswNbtNodeWriter );

            //    CswNbtTreeNLevelDs.onBeforeInsertNode += new CswNbtTreeNLevelDs.CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode );
            //    CswNbtTreeNLevelDs.onAfterInsertNode += new CswNbtTreeNLevelDs.CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode );

            //    ReturnVal = CswNbtTreeNLevelDs;
            //}
            //else if( TreeMode.DomProxy == TreeMode )
            //{
            CswNbtTreeDomProxy CswNbtTreeDomProxy = new CswNbtTreeDomProxy(//CswNbtTreeKey, 
																			View, _CswNbtResources, CswNbtNodeWriter, CswNbtNodeCollection, IsFullyPopulated );

            CswNbtTreeDomProxy.onBeforeInsertNode += new CswNbtTreeDomProxy.CswNbtTreeModificationHandler(CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode);
            CswNbtTreeDomProxy.onAfterInsertNode += new CswNbtTreeDomProxy.CswNbtTreeModificationHandler(CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode);

            ReturnVal = CswNbtTreeDomProxy;
            //}//        

            return (ReturnVal);

        }//makeTree()

    }//class CswNbtTreeFactory

}//namespace ChemSW.Nbt.TableEvents


