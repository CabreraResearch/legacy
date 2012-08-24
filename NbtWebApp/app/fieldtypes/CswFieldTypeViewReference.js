/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeViewReference';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var viewId = Csw.string(propVals.viewid).trim();
            var viewMode = Csw.string(propVals.viewmode).trim().toLowerCase();
            /* var viewName = Csw.string(propVals.name).trim(); */
            var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });

            if (o.EditMode !== Csw.enums.editMode.Add && false === o.Multi) {
                table.cell(1, 1).$.CswViewContentTree({
                    viewid: viewId
                });

                table.cell(1, 2).imageButton({
                    ID: o.ID + '_view',
                    ButtonType: Csw.enums.imageButton_ButtonType.View,
                    AlternateText: 'View',
                    Required: o.Required,
                    onClick: function () {
                        Csw.clientState.setCurrentView(viewId, viewMode);
                        /* case 20958 - so that it doesn't treat the view as a Grid Property view */
                        Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeId);
                        Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeKey);

                        Csw.window.location(Csw.getGlobalProp('homeUrl'));
                    }
                });
                if (false === o.ReadOnly) {
                    table.cell(1, 3).imageButton({
                        ID: o.ID + '_edit',
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        Required: o.Required,
                        onClick: function () {
                            o.onEditView(viewId);
                        }
                    });
                }
            } /* if(o.EditMode != Csw.enums.editMode.Add) */
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };
    
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
