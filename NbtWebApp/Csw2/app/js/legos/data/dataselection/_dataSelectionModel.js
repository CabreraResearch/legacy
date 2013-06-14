/// <reference path="../release/nsApp-vsdoc.js" />
/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _selectionModelClassIIFE(n$) {

    //Init the nameSpace
    n$.makeSubNameSpace('dataSelections');

    var selectionMode = n$.object();
    selectionMode.simple = 'SIMPLE';
    selectionMode.single = 'SINGLE';
    selectionMode.multi = 'MULTI';
    n$.constant(n$.dataSelections, 'selectionMode', selectionMode);
    
    var selectionModelProperties = n$.object();
    selectionModelProperties.allowDeselect = 'allowDeselect';
    selectionModelProperties.mode = 'mode';
    selectionModelProperties.pruneRemoved = 'pruneRemoved';
    n$.constant(n$.dataSelections, 'properties', selectionModelProperties);

    /**
     * Internal class to define a Selection Mode. This class cannot be directly instanced.
     * @param name {String} A name for the store class
     * @param extend {String} [extend=Ext.selection.Model] The Ext extension to use
     * @param mode {String} [mode=SIMPLE] The selection mode (SIMPLE; SINGLE; MULTI)
     * @param onDefine {Function} [onDefine] An optional callba
     */
    var _SelectionModel = function(name, extend, mode, onDefine) {
        if (!(n$.dataSelections.constants.selectionMode.has(mode))) {
            throw new Error('Grid selection model does not support mode "' + mode + '".');
        }
        var that = n$.classDefinition({
            name: name,
            namespace: 'dataSelections',
            onDefine: function(classDef) {
                delete classDef.initComponent;
                n$.property(classDef, 'mode', mode);
                if (onDefine) {
                    onDefine(classDef);
                }
            }
        });
        
        return that;
    };

    n$.instanceOf.register('_SelectionModel', _SelectionModel);

    /**
     * Instance a new Selection Model. Selection Models are the constraints upon which elements from ui structuress can be selected.
     * @param selDef {Object} Object describing the model
     * @param selDef.name {String} A name for the store class
     * @param selDef.extend {String} [extend=Ext.selection.Model] The Ext extension to use
     * @param selDef.mode {String} [mode=SIMPLE] The selection mode (SIMPLE; SINGLE; MULTI)
     * @param selDef.onDefine {Function} [onDefine] An optional callba
     */
    n$.dataSelections.register('_selectionModel', function (selDef) {
        if (!selDef) {
            throw new Error('Cannot create a selection model without a definition.');
        }
        selDef.mode = selDef.mode || n$.dataSelections.constants.selectionMode.simple;
        var ret = new _SelectionModel(selDef.name, selDef.extend, selDef.mode, selDef.onDefine);
        
        return ret;
    });

}(window.$nameSpace$));
