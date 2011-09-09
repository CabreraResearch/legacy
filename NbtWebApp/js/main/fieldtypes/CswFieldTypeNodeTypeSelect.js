/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {

    var pluginName = 'CswFieldTypeNodeTypeSelect';
    var nameCol = "NodeTypeName";
    var keyCol = "nodetypeid";
    var valueCol = "Include";

    var methods = {
        init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var optData = propVals.options;
            var selectMode = propVals.selectmode; // Single, Multiple, Blank

            var $cbaDiv = $('<div />')
                    .appendTo($Div)
                    .CswCheckBoxArray('init', {
                        ID: o.ID + '_cba',
                        UseRadios: (selectMode === 'Single'),
                        Required: o.Required,
                        ReadOnly: o.ReadOnly,
                        Multi: o.Multi,
                        onchange: o.onchange,
                        dataAry: optData,
			            nameCol: nameCol,
			            keyCol: keyCol,
                        valCol: valueCol
                    });

            return $Div;
        },
        save: function (o) { //$propdiv, $xml
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                o.propData.values.options = formdata;
                o.wasmodified = true;
            } 
            return $(this);
        } // save()
    };


    // Method calling logic
    $.fn.CswFieldTypeNodeTypeSelect = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);





