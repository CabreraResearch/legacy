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

            var $OptionsXml = o.propData.children('options');
            var SelectedNodeTypeIds = o.propData.children('NodeType').text().trim();
            var SelectMode = o.propData.children('NodeType').CswAttrXml('SelectMode');   // Single, Multiple, Blank

            var $CBADiv = $('<div />')
                            .appendTo($Div);

            // get data
            var data = new Array();
            var d = 0;
            $OptionsXml.find('item').each(function () {
                var $this = $(this);
                var $elm = { 
                             'label': $this.children('column[field="' + nameCol + '"]').CswAttrXml('value'),
                             'key': $this.children('column[field="' + keyCol + '"]').CswAttrXml('value'),
                             'values': [ ($this.children('column[field="' + valueCol + '"]').CswAttrXml('value') === "True") ]
                           };
                data[d] = $elm;
                d++;
            });

            $CBADiv.CswCheckBoxArray('init', {
                'ID': o.ID + '_cba',
                'cols': [ valueCol ],
                'data': data,
                'UseRadios': (SelectMode === 'Single'),
                'Required': o.Required,
                'ReadOnly': o.ReadOnly,
                'onchange': o.onchange
            });


        },
        save: function (o) { //$propdiv, $xml
            var $OptionsXml = o.propData.children('options');
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            for (var r = 0; r < formdata.length; r++) {
                var checkitem = formdata[r][0];
                var $xmlitem = $OptionsXml.find('item:has(column[field="' + keyCol + '"][value="' + checkitem.key + '"])');
                var $xmlvaluecolumn = $xmlitem.find('column[field="' + valueCol + '"]');

                if (checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "False")
                    $xmlvaluecolumn.CswAttrXml('value', 'True');
                else if (!checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "True")
                    $xmlvaluecolumn.CswAttrXml('value', 'False');
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





