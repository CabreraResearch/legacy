///// <reference path="~/js/CswCommon-vsdoc.js" />
///// <reference path="~/js/CswNbt-vsdoc.js" />

//(function ($) { 
//    "use strict";        
//    var pluginName = 'CswQuotaImage';

//    var methods = {
//        init: function (options) {
//            var o = {
//                urlMethod: 'getQuotaPercent',
//                ID: 'action_quota_image'
//            };
//            if(options) $.extend(o, options);

//            var $Div = $(this);

//            // Quota table
//            Csw.ajax.post({
//                urlMethod: o.urlMethod,
//                data: {},
//                success: function (data) {
//                    var percentUsed = Csw.number(data.result, 0);
//                    var image = '';
//                    $Div.contents().remove();
//                    if (percentUsed > 0)
//                    {
//                        if(percentUsed <= 33) {
//                            image = "good.gif";
//                        }
//                        else if(percentUsed > 33 && percentUsed <= 66) {
//                            image = "half.gif";
//                        }
//                        else if(percentUsed > 66 ) {
//                            image = "bad.gif";
//                        }
//                        $Div.append('<img src="Images/quota/'+ image +'" title="Quota Usage: '+ percentUsed+'%" />');
//                    }
//                } // success
//            }); // ajax()

//        } // init
//    }; // methods

//    
//    // Method calling logic
//    $.fn.CswQuotaImage = function (method) {
//        
//        if ( methods[method] ) {
//          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
//        } else if ( typeof method === 'object' || ! method ) {
//          return methods.init.apply( this, arguments );
//        } else {
//          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
//        }    
//  
//    };
//})(jQuery);
