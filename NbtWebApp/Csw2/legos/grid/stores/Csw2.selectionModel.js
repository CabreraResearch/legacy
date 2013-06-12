/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _selectionModelClassIIFE(nameSpace) {

    var gridSelectionMode = Object.create(null);
    gridSelectionMode.simple = 'SIMPLE';
    gridSelectionMode.single = 'SINGLE';
    gridSelectionMode.multi = 'MULTI';
    nameSpace.constant(nameSpace.grids, 'selectionMode', gridSelectionMode);

    /**
     * Internal class to define a Proxy. This class cannot be directly instanced.
     */
    var SelectionModel = function (mode, checkOnly, onSelect, onDeselect) {
        'use strict';
        if (!(nameSpace.grids.constants.selectionMode.has(mode))) {
            throw new Error('Grid selection model does not support mode "' + mode + '".');
        }
        var that = this;
        nameSpace.property(that, 'mode', mode);
        nameSpace.property(that, 'checkOnly', checkOnly);

        //Until we need more listeners on the Selection Model, let's define them ad hoc.
        //This'll be right until it isn't.
        if (onSelect || onDeselect) {
            nameSpace.property(that, 'listeners', {});
            if (onSelect) {
                nameSpace.property(that.listeners, 'select', onSelect);
            }
            if (onDeselect) {
                nameSpace.property(that.listeners, 'deselect', onDeselect);
            }
        }

        nameSpace.property(that, 'ExtSelModel', Ext.create('Ext.selection.CheckboxModel', that));

        return that;
    };

    nameSpace.instanceOf.lift('SelectionModel', SelectionModel);

    nameSpace.stores.lift('selectionModel',
        /**
         * Instance a new Selection Model. Selection Models are the constraints upon which elements from grids can be selected.
         * @param selDef {Object} Object describing the model
         */
        function selectionModel(selDef) {
            'use strict';
            if (!selDef) {
                throw new Error('Cannot create a selection model without a definition.');
            }
            selDef.mode = selDef.mode || nameSpace.grids.constants.selectionMode.simple;
            var ret = new SelectionModel(selDef.mode, selDef.checkOnly, selDef.onSelect, selDef.onDeselect);

            return ret;
        });

}(window.$om$));