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

            var optData = o.propData.options;
            var selectedNodeTypeIds = tryParseString(o.propData.nodetype).trim().split(',');
            var selectMode = o.propData.selectmode; // Single, Multiple, Blank

            var $cbaDiv = $('<div />')
                    .appendTo($Div)
                    .CswCheckBoxArray('transmorgify', {
                        dataAry: optData,
			            nameCol: nameCol,
			            keyCol: keyCol,
                        valCol: valueCol
                    })  
                    .CswCheckBoxArray('init', {
                        ID: o.ID + '_cba',
                        UseRadios: (selectMode === 'Single'),
                        Required: o.Required,
                        ReadOnly: o.ReadOnly,
                        onchange: o.onchange
                    });

            return $Div;
        },
        save: function (o) { //$propdiv, $xml
            var optionData = o.propData.options;
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            for (var r = 0; r < formdata.length; r++) {
                var checkitem = formdata[r][0];
                var objHelper = new ObjectHelper(optionData);
                var optItem = objHelper.find(keyCol, checkitem.key);
                var optVal = optItem[valueCol];

                if (checkitem.checked && optVal === "False")
                    optItem[valueCol] = 'True';
                else if (!checkitem.checked && optVal === "True")
                    optItem[valueCol] = 'False';
            } // for( var r = 0; r < formdata.length; r++)
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





