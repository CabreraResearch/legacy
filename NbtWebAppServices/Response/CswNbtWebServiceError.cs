using System;
using ChemSW.Exceptions;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public class CswNbtWebServiceError
    {
        private CswNbtSessionResources _CswNbtSessionResources;
        public CswNbtWebServiceError( CswNbtSessionResources CswNbtSessionResources )
        {
            _CswNbtSessionResources = CswNbtSessionResources;
        }

        public CswNbtWebServiceStatus getErrorStatus( Exception ex )
        {
            return _error( ex );
        }

        private CswNbtWebServiceStatus _error( Exception ex )
        {
            CswNbtWebServiceStatus Ret = new CswNbtWebServiceStatus();
            if( null != _CswNbtSessionResources &&
                null != _CswNbtSessionResources.CswNbtResources )
            {
                _CswNbtSessionResources.CswNbtResources.CswLogger.reportError( ex );
                _CswNbtSessionResources.CswNbtResources.Rollback();
            }

            CswDniException newEx = null;
            if( ex is CswDniException )
            {
                newEx = (CswDniException) ex;
            }
            else
            {
                newEx = new CswDniException( ex.Message, ex );
            }

            Ret.DisplayError = true;
            if( null != _CswNbtSessionResources &&
                null != _CswNbtSessionResources.CswNbtResources )
            {
                if( newEx.Type == ErrorType.Warning )
                {
                    Ret.DisplayError = ( _CswNbtSessionResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Ret.DisplayError = ( _CswNbtSessionResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Ret.ErrorType = newEx.Type;
            Ret.ErrorMessage = newEx.MsgFriendly;
            Ret.ErrorDetail = newEx.MsgEscoteric + "; " + ex.StackTrace;

            return Ret;
        } // _error()
    }
}