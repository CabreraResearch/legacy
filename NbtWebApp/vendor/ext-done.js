window.Ext.Loader.setConfig({ enabled: true });

window.Ext.Loader.setPath('Ext', 'vendor/extjs-4.1.0');
window.Ext.state.Manager.setProvider(new Ext.state.LocalStorageProvider());

window.Ext.require([
    'Ext.ux.grid.Printer',
    'Ext.ux.data.PagingMemoryProxy',
    'Ext.ux.grid.FiltersFeature',
    'Ext.ux.TabScrollerMenu',
    'Ext.ux.TabReorderer',
    'Ext.ux.CheckColumn',
    'Ext.tip.QuickTipManager',
    'Ext.window.Window',
    'Ext.tab.Panel',
    'Ext.selection.CellModel',
    'Ext.data.*',
    'Ext.grid.*',
    'Ext.tree.*',
    'Ext.ux.CheckColumn',
    'Ext.ux.tree.plugin.NodeDisabled'
]);


// from: http://www.sencha.com/forum/showthread.php?200248-4.1.0-button-contents-are-cut-off-when-using-scoped-CSS&highlight=scoperesetcss
// Fixes the grid pager buttons disappearing if scopeResetCSS=true
window.Ext.override(Ext.button.Button, {

    getPersistentPadding: function () {

        var me = this,
            padding = me.persistentPadding,
            btn, leftTop, btnEl, btnInnerEl;


        // Create auto-size button offscreen and measure its insides
        // Short-circuit IE as it sometimes gives false positive for padding
        if (!padding) {
            padding = me.self.prototype.persistentPadding = [0, 0, 0, 0];
            if (!window.Ext.isIE) {


                // BEGIN PATCH
                btn = window.Ext.create('Ext.button.Button', {
                    renderTo: Ext.getBody(),
                    text: 'test',
                    style: 'position:absolute;top:-999px;'
                });
                btnEl = btn.btnEl;
                // END PATCH

                btnInnerEl = btn.btnInnerEl;
                btnEl.setSize(null, null); //clear any hard dimensions on the button el to see what it does naturally
                leftTop = btnInnerEl.getOffsetsTo(btnEl);
                padding[0] = leftTop[1];
                padding[1] = btnEl.getWidth() - btnInnerEl.getWidth() - leftTop[0];
                padding[2] = btnEl.getHeight() - btnInnerEl.getHeight() - leftTop[1];
                padding[3] = leftTop[0];

                btn.destroy();
            }
        }
        return padding;
    }
});