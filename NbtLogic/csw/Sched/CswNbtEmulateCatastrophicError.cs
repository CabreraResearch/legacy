using System;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// bz # 9787: this class illustrates what happens when an exection is not handled at the lower
    /// layers. It's the easiest way to produce an exception that, prior to the 9787 control 
    /// of flow changes, would attempt to croass the thread boundary and thus bring the service
    /// to a crashing halt with no explanation
    /// </summary>
    public class CswNbtEmulateCatastrophicError : CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;

        public enum CatastrophicErrorType { UnCaughtInRun, UnCaughtInDoesRunNow };
        private CatastrophicErrorType _CatastrophicErrorType;
        public override void reset()
        {
            _Succeeded = true;
            _StatusMessage = string.Empty;
        }//


        public CswNbtEmulateCatastrophicError( CswNbtResources CswNbtResources, CatastrophicErrorType CatastrophicErrorType )
        {
            _CatastrophicErrorType = CatastrophicErrorType;
            _CswNbtResources = CswNbtResources;
        }//ctor

        override public bool doesItemRunNow()
        {

            if( CatastrophicErrorType.UnCaughtInDoesRunNow == _CatastrophicErrorType )
                throw ( new CswDniException( "Deliberate exception thrown to emulate unhandled exception; catastrophic error type is " + CatastrophicErrorType.UnCaughtInDoesRunNow.ToString()  ) ); 

            return ( true );
        }//runNow()

        override public void run()
        {

            if ( CatastrophicErrorType.UnCaughtInRun == _CatastrophicErrorType )
                throw ( new CswDniException( "Deliberate exception thrown to emulate unhandled exception; catastrophic error type is " + CatastrophicErrorType.UnCaughtInDoesRunNow.ToString() ) ); 


        }//run()

        override public string Name
        {
            get
            {
                return ( "Update Property Values" );
            }
        }

        private bool _Succeeded = true;
        override public bool Succeeded
        {
            get
            {
                return ( _Succeeded );
            }

        }//Succeeded

        private string _StatusMessage = "";
        override public string StatusMessage
        {
            get
            {
                return ( _StatusMessage );
            }

        }//StatusMessage

    }//CswNbtEmulateCatastrophicError

}//namespace ChemSW.Nbt.Sched
