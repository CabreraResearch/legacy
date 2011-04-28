; (function ($) {

    var PluginName = 'CswFieldTypeNodeTypeSelect';
    var NameCol = "NodeTypeName";
    var KeyCol = "nodetypeid";
    var ValueCol = "Include";

    var methods = {
        init: function (o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var $OptionsXml = o.$propxml.children('options');
            var SelectedNodeTypeIds = o.$propxml.children('NodeType').text().trim();
            var SelectMode = o.$propxml.children('NodeType').attr('SelectMode');   // Single, Multiple, Blank

            var $CBADiv = $('<div />')
                            .appendTo($Div);

            // get data
            var data = new Array();
            var d = 0;
            $OptionsXml.find('item').each(function () {
                var $this = $(this);
                var $elm = { 
                             'label': $this.children('column[field="' + NameCol + '"]').attr('value'),
                             'key': $this.children('column[field="' + KeyCol + '"]').attr('value'),
                             'values': [ ($this.children('column[field="' + ValueCol + '"]').attr('value') === "True") ]
                           };
                data[d] = $elm;
                d++;
            });

            $CBADiv.CswCheckBoxArray('init', {
                'ID': o.ID + '_cba',
                'cols': [ ValueCol ],
                'data': data,
                'UseRadios': (SelectMode === 'Single'),
                'Required': o.Required,
                'onchange': o.onchange
            });


        },
        save: function (o) { //$propdiv, $xml
            var $OptionsXml = o.$propxml.children('options');
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            for (var r = 0; r < formdata.length; r++) {
                var checkitem = formdata[r][0];
                var $xmlitem = $OptionsXml.find('item:has(column[field="' + KeyCol + '"][value="' + checkitem.key + '"])');
                var $xmlvaluecolumn = $xmlitem.find('column[field="' + ValueCol + '"]');

                if (checkitem.checked && $xmlvaluecolumn.attr('value') === "False")
                    $xmlvaluecolumn.attr('value', 'True');
                else if (!checkitem.checked && $xmlvaluecolumn.attr('value') === "True")
                    $xmlvaluecolumn.attr('value', 'False');
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
            $.error('Method ' + method + ' does not exist on ' + PluginName);
        }

    };
})(jQuery);





