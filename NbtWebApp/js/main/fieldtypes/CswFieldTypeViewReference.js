/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeViewReference';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var viewId = Csw.string(propVals.viewid).trim();
            var viewMode = Csw.string(propVals.viewmode).trim().toLowerCase();
            /* var viewName = Csw.string(propVals.name).trim(); */
            var state = Csw.clientState();
            var $table = $Div.CswTable('init', {'ID': o.ID + '_tbl'});

            if (o.EditMode !== EditMode.AddInPopup.name && false === o.Multi) {
                $table.CswTable('cell', 1, 1).CswViewContentTree({
                    viewid: viewId
                });


                $table.CswTable('cell', 1, 2).CswImageButton({
                    ID: o.ID + '_view',
                    ButtonType: CswImageButton_ButtonType.View,
                    AlternateText: 'View',
                    Required: o.Required,
                    onClick: function () {
                        state.setCurrentView(viewId, viewMode);
                        var cswCookie = Csw.cookie();
                        // case 20958 - so that it doesn't treat the view as a Grid Property view
                        cswCookie.clear(cswCookie.cookieNames.CurrentNodeId);
                        cswCookie.clear(cswCookie.cookieNames.CurrentNodeKey);

                        window.location = "Main.html";
                        return CswImageButton_ButtonType.None;
                    }
                });
                if (false === o.ReadOnly) {
                    $table.CswTable('cell', 1, 3).CswImageButton({
                        ID: o.ID + '_edit',
                        ButtonType: CswImageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        Required: o.Required,
                        onClick: function () {
                            o.onEditView(viewId);
                            return CswImageButton_ButtonType.None;
                        }
                    });
                }
            } // if(o.EditMode != EditMode.AddInPopup.name)
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeViewReference = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
