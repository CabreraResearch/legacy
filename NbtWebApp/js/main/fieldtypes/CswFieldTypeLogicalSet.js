/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswCheckBoxArray.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeLogicalSet';
    var nameCol = 'name';
    var keyCol = 'key';
  

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var logicalSetJson = o.propData.logicalsetjson;
            
            var $cbaDiv = $('<div />')
                            .appendTo($Div)
                            .CswCheckBoxArray('transmorgify', {
                                dataAry: logicalSetJson,
			                    nameCol: nameCol,
			                    keyCol: keyCol
                            })
                            .CswCheckBoxArray('init', {
                                ID: o.ID + '_cba',
                                onchange: o.onchange,
								ReadOnly: o.ReadOnly
                            });
            return $Div;

        },
        save: function(o) { //$propdiv, $xml
                var logicalSetJson = o.propData.logicalsetxml;
                var $CBADiv = o.$propdiv.children('div').first();
                var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
                for( var r = 0; r < formdata.length; r++)
                {
                    for( var c = 0; c < formdata[r].length; c++)
                    {
                        var checkitem = formdata[r][c];
                        var jsonItem = findObject(logicalSetJson, keyCol, checkitem.key);
                        var itemColumn = findObject(jsonItem, checkitem.collabel);
                    
                        if (checkitem.checked && itemColumn === "False") {
                            itemColumn = 'True';
                        }
                        else if (!checkitem.checked && itemColumn === "True") {
                            itemColumn = 'False';
                        }
                    } // for( var c = 0; c < formdata.length; c++)
                } // for( var r = 0; r < formdata.length; r++)
            } // save()
    };
    

    // Method calling logic
    $.fn.CswFieldTypeLogicalSet = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);





