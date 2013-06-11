indow.onerror = function (msg, url, lineNumber) {

    console.error('%s\rurl: %s\rline: %d', msg, url, lineNumber);

    return true; //true means don't propogate the error
}

window.Ext.Loader.setConfig({ enabled: true });

window.Ext.Loader.setPath('Ext', '../vendor/extJS-4.1.0');
window.Ext.state.Manager.setProvider(new Ext.state.LocalStorageProvider());

window.Ext.Error.handle = function(err) {
    console.error(err);
};