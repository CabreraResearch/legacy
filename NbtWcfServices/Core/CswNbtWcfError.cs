using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Core
{
    public class CswNbtWcfError
    {
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources;
        public CswNbtWcfError( CswNbtWcfSessionResources CswNbtWcfSessionResources )
        {
            _CswNbtWcfSessionResources = CswNbtWcfSessionResources;
        }

        public CswNbtWebServiceErrorMessage getErrorStatus( Exception ex )
        {
            return _error( ex );
        }

        private CswNbtWebServiceErrorMessage _error( Exception ex )
        {
            CswNbtWebServiceErrorMessage Ret = new CswNbtWebServiceErrorMessage();
            if( null != _CswNbtWcfSessionResources &&
                null != _CswNbtWcfSessionResources.CswNbtResources &&
                _CswNbtWcfSessionResources.CswNbtResources.IsInitializedForDbAccess )
            {
                _CswNbtWcfSessionResources.CswNbtResources.CswLogger.reportError( ex );
                _CswNbtWcfSessionResources.CswNbtResources.Rollback();
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
            if( null != _CswNbtWcfSessionResources &&
                null != _CswNbtWcfSessionResources.CswNbtResources &&
                _CswNbtWcfSessionResources.CswNbtResources.IsInitializedForDbAccess )
            {
                if( newEx.Type == ErrorType.Warning )
                {
                    Ret.DisplayError = ( _CswNbtWcfSessionResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Ret.DisplayError = ( _CswNbtWcfSessionResources.CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Ret.ErrorType = newEx.Type;
            Ret.ErrorMessage = newEx.MsgFriendly;
            string Message = newEx.MsgEscoteric + "; ";
            if( CswConvert.ToBoolean( _CswNbtWcfSessionResources.CswNbtResources.SetupVbls[CswSetupVariableNames.ShowFullExceptions] ) )
            {
                Message += ex.StackTrace;
            }
            Ret.ErrorDetail = Message;

            return Ret;
        } // _error()
    }
}