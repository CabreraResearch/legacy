/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function (n$) {

    //#region CORE 

    /**
     * Custom Errors
    */
    n$.makeSubNameSpace('errors');

    /**
     * Type checking
    */
    n$.makeSubNameSpace('is');

    /**
     * To instance check classes
    */
    n$.makeSubNameSpace('instanceOf');

    /**
     * Type conversion
    */
    n$.makeSubNameSpace('to');

    //#endregion CORE 

    //#region ACTIONS 

    /**
     * Actions
    */
    n$.makeSubNameSpace('actions');

    /**
     * Query Builder
    */
    n$.actions.makeSubNameSpace('querybuilder');

    /**
     * SQL
    */
    n$.actions.makeSubNameSpace('sql');

    //#endregion ACTIONS 

    //#region DOM 

    /**
     * The MetaData namespace. Represents the structures of nameSpaceName nodes, elements and properties.
     */
    n$.makeSubNameSpace('metadata');

    /**
     * The node namespace. Represents an nameSpaceName Node and its properties.
     * [1]: This class is responsible for constructing the DOM getters (properties on this object which reference Nodes in the DOM tree)
     * [2]: This class exposes helper methods which can get/set properties on this instance of the node.
     * [3]: This class validates the execution of these methods (e.g. Is the node still in the DOM; has it been GC'd behind our backs)
     * [4]: Maintaining an im-memory representation of tree with children/parents
     */
    n$.makeSubNameSpace('node');

    //#endregion DOM

    //#region EXT



    /**
     * Models
    */
    n$.makeSubNameSpace('dataModels');

    /**
     *Grids
    */
    n$.makeSubNameSpace('grids');

    /**
     * Grids Columns
    */
    n$.grids.makeSubNameSpace('columns');

    /**
     * Grids Subscribers
    */
    n$.grids.makeSubNameSpace('subscribers');

    /**
     * Stores
    */
    n$.makeSubNameSpace('stores');

    /**
     * Panels
    */
    n$.makeSubNameSpace('panels');

    /**
     * Panel Subscribers
    */
    n$.panels.makeSubNameSpace('subscribers');

    /**
     * Trees
    */
    n$.makeSubNameSpace('trees');

    /**
     * Tree Subscribers
    */
    n$.trees.makeSubNameSpace('subscribers');

    /**
     * Windows.
     * Aside: Since 'window' cannot be used _and_ since few synonyms of the word conjurre the same meaning, use the Russian: sheet (window), sheets (windows)
    */
    n$.makeSubNameSpace('sheets');

    /**
     * Window subscribers
    */
    n$.sheets.makeSubNameSpace('subscribers');


    //#endregion EXT
    
    n$.makeSubNameSpace('ajax');
    n$.makeSubNameSpace('ajaxCore');
    n$.makeSubNameSpace('ajaxWcf');
    n$.makeSubNameSpace('browserCompatibility');
    n$.makeSubNameSpace('clientChanges');
    n$.makeSubNameSpace('clientSession');
    n$.makeSubNameSpace('clientState');
    n$.makeSubNameSpace('clientDb');
    n$.makeSubNameSpace('currentUser');
    n$.makeSubNameSpace('composites');
    n$.makeSubNameSpace('cookie');
    n$.makeSubNameSpace('db');
    n$.db.makeSubNameSpace('index');
    n$.db.makeSubNameSpace('select');
    n$.db.makeSubNameSpace('table');
    n$.makeSubNameSpace('dialogs');
    n$.makeSubNameSpace('enums');
    n$.makeSubNameSpace('error');
    n$.makeSubNameSpace('fun');
    n$.makeSubNameSpace('layouts');
    n$.makeSubNameSpace('literals');
    n$.makeSubNameSpace('main');
    n$.makeSubNameSpace('nbt');
    n$.makeSubNameSpace('promises');
    n$.makeSubNameSpace('properties');
    n$.makeSubNameSpace('reports');
    n$.makeSubNameSpace('window');
    n$.makeSubNameSpace('wizard');
    n$.makeSubNameSpace('workers');

    n$.register('isFunction', function (obj) {
        'use strict';
        /// <summary> Returns true if the object is a function</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isFunction(obj));
        return ret;
    });

    n$.register('tryExec', function (func) {
        'use strict';
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        var that = this;
        var ret = false;
        try {
            if (Csw.isFunction(func)) {
                ret = func.apply(that, Array.prototype.slice.call(arguments, 1));
            }
        } catch (exception) {
            if ((exception.name !== 'TypeError' ||
                exception.type !== 'called_non_callable') &&
                exception.type !== 'non_object_property_load') { /* ignore errors failing to exec self-executing functions */
                Ext.resumeLayouts(true);
                Csw.error.catchException(exception);
            }
        } finally {
            // In JavaScript, finally executes after return. http://www.2ality.com/2013/03/try-finally.htmls
            // return true;
        }
        return ret;
    });


    n$.register('method', function (func) {
        'use strict';
        return function () {
            var that = this;
            var args = Array.prototype.slice.call(arguments, 0);
            args.unshift(func);
            return Csw.tryExec.apply(that, args);
        };
    });

}(window.$nameSpace$));
