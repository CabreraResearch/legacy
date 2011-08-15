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
            var selectedNodeTypeIds = tryParseString(o.propData.nodetype).trim();
            var selectMode = o.propData.selectmode;   // Single, Multiple, Blank

            var $CBADiv = $('<div />')
                            .appendTo($Div);

            // get data
            var data = new [];
            var d = 0;
            for (var i=0; i < optData.length; i++) {
                var thisSet = optData[i];
                for (var item in thisSet) {
                    if(thisSet.hasOwnProperty(item)) {
                        var $elm = {
                            'label': thisSet[item],
                            'key': item,
                            'values': [ isTrue(thisSet[item]) ]
                        };
                        data[d] = $elm;
                        d++;
                    }
                }
            }

            $CBADiv.CswCheckBoxArray('init', {
                'ID': o.ID + '_cba',
                'cols': [ valueCol ],
                'data': data,
                'UseRadios': (selectMode === 'Single'),
                'Required': o.Required,
                'ReadOnly': o.ReadOnly,
                'onchange': o.onchange
            });


        },
        save: function (o) { //$propdiv, $xml
            var optionData = o.propData.options;
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            for (var r = 0; r < formdata.length; r++) {
                var checkitem = formdata[r][0];
                var optItem = findObject(optionData, keyCol, checkitem.key);
                var optVal = optItem[valueCol];

                if (checkitem.checked && optVal === "False")
                    optVal = 'True';
                else if (!checkitem.checked && optVal === "True")
                    optVal = 'False');
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





