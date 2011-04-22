
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
					var ret = $.cookie(cookiename);
					if(ret == undefined)
						ret = '';
					return ret;
				},
			'set': function(cookiename, value) 
				{
					$.cookie(cookiename, value);
				},
			'clear': function (cookiename)
				{
					$.cookie(cookiename, '');
				},
			'clearAll': function() 
				{
					for(var CookieName in CswCookieName) 
					{
						if(CswCookieName.hasOwnProperty(CookieName))
						{
							$.cookie(CswCookieName[CookieName], null);
							for(var SubCookieName in CswCookieName[CookieName]) 
							{
								if( CswCookieName[CookieName].hasOwnProperty(SubCookieName))
								{
									$.cookie(CswCookieName[CookieName][SubCookieName], null);
								}
							}
						}
					}
				} // clearAll
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




