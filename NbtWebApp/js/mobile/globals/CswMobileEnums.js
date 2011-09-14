var CswMobileCssClasses = {
    listview: { name: 'csw_listview' },
    select: { name: 'csw_prop_select' },
    fieldset: { name: 'csw_fieldset' },
    answer: { name: 'csw_answer' },
    collapsible: { name: 'csw_collapsible' },
    proplabel: { name: 'csw_prop_label' },
    OOC: { name: 'OOC' },
    onlineStatus: { name: 'onlineStatus' },
    pendingChanges: { name: 'pendingChanges' }
};
var CswMobileHeaderButtons = {
    back: {name: 'back', 
            ID: 'back',
            text: 'Back',
            cssClass: 'ui-btn-left',
            dataDir: 'reverse',
            dataIcon: 'arrow-l',
            dataRelationship: 'back'
    },
    search: {name: 'search',
             ID: 'search',
             text: 'Search',
             cssClass: 'ui-btn-right'
    }
};
var CswMobileFooterButtons = {
    online: {name: 'online',
             ID: 'online',
             text: 'Online',
             cssClass: CswMobileCssClasses.onlineStatus.name + ' ui-btn-active ',
             dataIcon: 'gear'
    },
    refresh: {name: 'refresh',
              ID: 'refresh',
              text: 'Refresh',
              cssClass: 'refresh',
              dataIcon: 'refresh'
    
    },
    fullsite: {name: 'main',
               ID: 'main',
               text: 'Full Site',
               href: 'Main.html',
               rel: 'external',
               dataIcon: 'home'
    },
    help: {name: 'help',
           ID: 'help',
           text: 'Help',
           dataIcon: 'info'
    }
};
var CswMobilePage_Type = {
    login: { name: 'login', id: 'logindiv', title: 'ChemSW Live' },
    help: { name: 'help', id: 'helpdiv', title: 'Help' },
    nodes: { name: 'nodes', id: 'nodesdiv', title: 'Nodes' },
    offline: {name: 'offline', id: 'offlinediv', title: 'Sorry Charlie!' },
    online: { name: 'online', id: 'onlinediv', title: 'Sync Status' },
    props: { name: 'props', id: 'propsdiv', title: 'Properties' },
    search: { name: 'search', id: 'searchdiv', title: 'Search' },
    tabs: { name: 'tabs', id: 'tabsdiv', title: 'Tabs' },
    views: { name: 'views', id: 'viewsdiv', title: 'Views' }
};
var CswMobileGlobal_Config = {
    theme: 'b',
    storedViews: 'storedviews'
};