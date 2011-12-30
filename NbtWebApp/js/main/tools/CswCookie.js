/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = 'CswCookie';

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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    

    };

})(jQuery);




