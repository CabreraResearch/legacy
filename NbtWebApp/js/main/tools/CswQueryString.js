/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

// adapted from 
// http://stackoverflow.com/questions/901115/get-querystring-values-in-javascript/2880929#2880929

// usage:    page.html?param=value&param2=value2
// var qs = $.CswQueryString();
// qs.param   -->  'value'
// qs.param2  -->  'value2'

(function ($)
{
    $.CswQueryString = function()
    {
        var urlParams = { };

        var e,
            a = /\+/g , // Regex for replacing addition symbol with a space
            r = /([^&=]+)=?([^&]*)/g ,
            d = function(s) { return decodeURIComponent(s.replace(a, " ")); },
            q = window.location.search.substring(1);

        while (e = r.exec(q))
        {
            urlParams[d(e[1])] = d(e[2]);
        }

        return urlParams;
    };

})(jQuery);
