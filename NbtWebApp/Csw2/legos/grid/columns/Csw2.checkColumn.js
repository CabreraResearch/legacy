/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _checkColumnIIFE(){

    /**
     * Internal check column class
     * @param text {String} Name of the column
    */
    var CheckColumn = function (text) {
        var that = Csw2.grids.columns.column({
                xtype: Csw2.grids.constants.xtypes.checkcolumn,
                flex: 0.075,
                text: text
            });
        that.align = 'center';

        return that;
    };

    Csw2.instanceOf.lift('CheckColumn', CheckColumn);

    /**
     * Create a check column
     * @param sortable {Boolean} [sortable=true] Is Column Sortable
     * @param text {String} Column Name
     * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu Disabled
    */
    Csw2.grids.columns.lift('checkColumn', function (sortable, text, menuDisabled){
        if(arguments.length === 0) {
            throw new Error('Cannot create a column without parameters');
        }

        var ret = new CheckColumn(text);
        ret.menuDisabled = menuDisabled;
        ret.sortable = sortable;

        return ret;
    });


    }());