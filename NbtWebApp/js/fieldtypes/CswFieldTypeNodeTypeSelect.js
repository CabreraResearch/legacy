; (function ($) {

    var PluginName = 'CswFieldTypeNodeTypeSelect';
    var NameCol = "NodeTypeName";
    var KeyCol = "nodetypeid";
    var ValueCol = "Include";

    var methods = {
        init: function (nodepk, $xml) {

            var $Div = $(this);
            $Div.children().remove();

            var ID = $xml.attr('id');
            var Required = ($xml.attr('required') == "true");
            var ReadOnly = ($xml.attr('readonly') == "true");

            var $OptionsXml = $xml.children('options');
            var SelectedNodeTypeIds = $xml.children('NodeType').text();
            var SelectMode = $xml.children('NodeType').attr('SelectMode');   // Single, Multiple, Blank

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
                             'values': [ ($this.children('column[field="' + ValueCol + '"]').attr('value') == "True") ]
                           };
                data[d] = $elm;
                d++;
            });

            $CBADiv.CswCheckBoxArray('init', {
                'ID': ID + '_cba',
                'cols': [ ValueCol ],
                'data': data,
                'UseRadios': (SelectMode == 'Single'),
                'Required': Required
            });


        },
        save: function ($propdiv, $xml) {
            var $OptionsXml = $xml.children('options');
            var $CBADiv = $propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': $xml.attr('id') + '_cba' } );
            for (var r = 0; r < formdata.length; r++) {
                var checkitem = formdata[r][0];
                var $xmlitem = $OptionsXml.find('item:has(column[field="' + KeyCol + '"][value="' + checkitem.key + '"])');
                var $xmlvaluecolumn = $xmlitem.find('column[field="' + ValueCol + '"]');

                if (checkitem.checked && $xmlvaluecolumn.attr('value') == "False")
                    $xmlvaluecolumn.attr('value', 'True');
                else if (!checkitem.checked && $xmlvaluecolumn.attr('value') == "True")
                    $xmlvaluecolumn.attr('value', 'False');
            } // for( var r = 0; r < formdata.length; r++)
            console.log($OptionsXml);
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





