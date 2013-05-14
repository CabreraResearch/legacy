/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _gridColumnIIFE(){

    /**
     * Private renderer column class constructor. 
     * @param dataIndex {String} Column id
     * @param width {Number} [width] Absolute width of the column
     * @param flex {Number} [flex] Relative width of the column
     * @param onRender {Function} Render method for the column
    */
    var RendererColumn = function (dataIndex, width, flex, onRender) {
        var that = Csw2.grids.columns.column({
                xtype: Csw2.constants.xtypes.gridcolumn,
                dataIndex: dataIndex
                //text: dataIndex
        });
        Csw2.property(that, 'renderer', onRender);
        if (width && width > 0) {
            Csw2.property(that, 'width', width);
        } else {
            if (flex && flex > 0) {
                Csw2.property(that, 'flex', flex);
            }
        }

        return that;
    };

    Csw2.instanceof.lift('RendererColumn', RendererColumn);

    /**
     * Create a grid column
     * @param colDef {Object} Definition of the renderer column
    */
    Csw2.grids.columns.lift('rendererColumn', function (colDef){
        if (!colDef || arguments.length === 0) {
            throw new Error('Cannot create a column without parameters');
        }
        if (!colDef.onRender) {
            throw new Error('Cannot create a render column without a render method.');
        }

        var ret = new RendererColumn(colDef.dataIndex, colDef.width, colDef.flex, colDef.onRender);
        
        return ret;
    });


    }());