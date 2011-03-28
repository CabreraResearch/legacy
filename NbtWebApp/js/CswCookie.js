
var CswCookieName = {
	SessionId: 'CswSessionId',
	Username: 'csw_username',
	CurrentView: { 
		ViewId: 'csw_currentviewid',
		ViewMode: 'csw_currentviewmode'
	},
	CurrentNode: { 
		NodeId: 'csw_currentnodeid',
		NodeKey	: 'csw_currentnodekey'
	},
	CurrentTabId: 'csw_currenttabid'
};


(function ($) {
	
	var PluginName = 'CswCookie';

    $.CswCookie = function (method) {
		var methods = {
			'get': function(cookiename) 
				{
					return $.cookie(cookiename);
				},
			'set': function(cookiename, value) 
				{
					$.cookie(cookiename, value);
				},
			'clear': function(cookiename) 
				{
					$.cookie(cookiename, null);
				}
		};

		// Method calling logic
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    

    };

})(jQuery);




