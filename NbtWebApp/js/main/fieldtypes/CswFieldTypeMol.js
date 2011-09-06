/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />

    var pluginName = 'CswFieldTypeMol';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            //var href = tryParseString(propVals.href).trim();
            var width = 100; //tryParseString(propVals.width);
            //var height = tryParseString(propVals.height);
            //var fileName = tryParseString(propVals.name).trim();
            var mol = tryParseString(propVals.mol).trim();
            var col = tryParseString(propVals.column).trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1).CswAttrDom('colspan', '3');
            var $cell21 = $table.CswTable('cell', 2, 1).CswAttrDom('width', width - 36);
            var $cell22 = $table.CswTable('cell', 2, 2).CswAttrDom('align', 'right');
            var $cell23 = $table.CswTable('cell', 2, 3).CswAttrDom('align', 'right');

            if (mol !== '') {
                var $TextBox = $('<pre>' + mol + '</pre>')
									.appendTo($cell11);
                //$cell21.append('<a href="' + href + '" target="_blank">' + fileName + '</a>');
            }

            if (!o.ReadOnly && o.EditMode != EditMode.AddInPopup.name) {
                var $editButton = $('<div/>')
                    .appendTo($cell22)
                    .CswImageButton({
                        ButtonType: CswImageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        onClick: function ($ImageDiv) {

                            $.CswDialog('FileUploadDialog', {
                                url: '/NbtWebApp/wsNBT.asmx/fileForProp',
                                params: {
                                    'PropId': o.propData.id,
                                    'Column': col
                                },
                                onSuccess: function () {
                                    o.onReload();
                                }
                            });
                            return CswImageButton_ButtonType.None;
                        }
                    });
                var $clearButton = $('<div/>')
                    .appendTo($cell23)
                    .CswImageButton({
                        ButtonType: CswImageButton_ButtonType.Clear,
                        AlternateText: 'Clear',
                        ID: o.ID + '_clr',
                        onClick: function ($ImageDiv) {

                            if (confirm("Are you sure you want to clear this structure?")) {
                                var dataJson = {
                                    PropId: o.propData.id,
                                    IncludeBlob: true
                                };

                                CswAjaxJson({
                                    url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                    data: dataJson,
                                    success: function () { o.onReload(); }
                                });
                            }
                            return CswImageButton_ButtonType.None;
                        }
                    });
            }

        },
        save: function (o) { //$propdiv, o.propData
            // nothing to do here
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMol = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
