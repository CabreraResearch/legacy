/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypeFile';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();

            if(o.Multi) {
                $Div.append(Csw.enums.multiEditDefaultValue);
            } else {

                var propVals = o.propData.values;

                var href = Csw.string(propVals.href).trim();
                var fileName = Csw.string(propVals.name).trim();

                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell13 = $table.CswTable('cell', 1, 3);

                $cell11.append('<a href="' + href + '" target="_blank">' + fileName + '</a>');

                if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    //Edit button
                    $('<div/>')
                        .appendTo($cell12)
                        .CswImageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                                AlternateText: 'Edit',
                                ID: o.ID + '_edit',
                                onClick: function () {
                                    $.CswDialog('FileUploadDialog', {
                                        url: '/NbtWebApp/wsNBT.asmx/fileForProp',
                                        params: {
                                            PropId: o.propData.id
                                        },
                                        onSuccess: function () {
                                            o.onReload();
                                        }
                                    });
                                    return Csw.enums.imageButton_ButtonType.None;
                                }
                            });
                    //Clear button
                    $('<div/>')
                        .appendTo($cell13)
                        .CswImageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Clear,
                                AlternateText: 'Clear',
                                ID: o.ID + '_clr',
                                onClick: function () {
                                    /* remember: confirm is globally blocking call */
                                    if (confirm("Are you sure you want to clear this file?")) {
                                        var dataJson = {
                                            PropId: o.propData.id,
                                            IncludeBlob: true
                                        };

                                        Csw.ajax.post({
                                                url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                                data: dataJson,
                                                success: function () { o.onReload(); }
                                            });
                                    }
                                    return Csw.enums.imageButton_ButtonType.None;
                                }
                            });
                }
            }
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeFile = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
