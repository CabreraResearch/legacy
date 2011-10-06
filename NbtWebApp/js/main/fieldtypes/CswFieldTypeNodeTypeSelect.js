/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswCheckBoxArray.js" />

; (function ($) {

    var pluginName = 'CswFieldTypeNodeTypeSelect',
        nameCol = 'label',
        keyCol = 'key',
        valueCol = 'value',
        methods = {
            init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

                var $Div = $(this);

                var propVals = o.propData.values;
                var optData = propVals.options;
                var selectMode = propVals.selectmode; // Single, Multiple, Blank

                var $cbaDiv = $('<div />')
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
                                    valCol: valueCol,
                                    valColName: 'Include'
                                });

                $Div.contents().remove();
                $Div.append($cbaDiv);            
                return $Div;
            },
            save: function (o) { //$propdiv, $xml
                var attributes = { options: null };
                var $cbaDiv = o.$propdiv.children('div').first();
                var formdata = $cbaDiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
                if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                    attributes.options = formdata.data;
                } 
                preparePropJsonForSave(o.Multi, o.propData, attributes);
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





